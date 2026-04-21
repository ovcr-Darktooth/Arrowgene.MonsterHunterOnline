using System;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.Data;
using Arrowgene.MonsterHunterOnline.Service.System;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

/// <summary>
/// Monster → player hit packet. The client runs HitCol sweeps locally and reports
/// the hit to the server. Server resolves <see cref="CSBattleDMG.hashAttacker"/>
/// against the AttackData table, applies the damage to the reporting client's HP,
/// and broadcasts the new HP to all clients.
/// </summary>
public class BattleDmgWithoutVerifyHandler : CsProtoStructureHandler<CSDMGContext>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(BattleDmgWithoutVerifyHandler));

    private readonly AttackDataTable _attackData;
    private readonly MonsterAIManager _monsterAi;
    private readonly ClientManager _clientManager;

    public BattleDmgWithoutVerifyHandler(
        AttackDataTable attackData,
        MonsterAIManager monsterAi,
        ClientManager clientManager)
    {
        _attackData = attackData;
        _monsterAi = monsterAi;
        _clientManager = clientManager;
    }

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_DMG_WITHOUT_VERIFY;

    // Fallback damage used when we can't resolve the attacker hash. Keeps gameplay flowing
    // while the hash algorithm / source strings are still being reverse-engineered.
    private const int FallbackDamage = 10;

    public override void Handle(Client client, CSDMGContext req)
    {
        CSBattleDMG hit = req.HitInfo;
        if (hit == null)
        {
            Logger.Error(client, "[709] null HitInfo");
            return;
        }

        if (client.Character == null || client.Attr == null)
        {
            Logger.Error(client, "[709] client has no character/attr; dropping");
            return;
        }

        // Client-authoritative: the sender is the victim. The client reports CryEngine runtime
        // entity IDs (0xFFxx range), not our Character.Id / MonsterAI netId, so we can't
        // cross-check identities server-side. We rely on hash resolution + damage cap.
        int dmg;
        string resolved;
        if (_attackData.TryGetIdByHash(hit.hashAttacker, out int attackDataId)
            && _attackData.TryGet(attackDataId, out AttackData data))
        {
            dmg = data.DamagePower;
            resolved = $"attackData={attackDataId} ({data.AttackName})";
        }
        else
        {
            dmg = FallbackDamage;
            resolved = $"fallback (HASH-MISS hashAttacker=0x{hit.hashAttacker:X8} hashWeaponClass=0x{hit.hashWeaponClass:X8} hashFireMode=0x{hit.hashFireMode:X8} hashCurEvent=0x{hit.hashCurEvent:X8} skillSeq={hit.skillSeq})";
        }

        int maxHp = client.Attr.CharMaxHP != null && client.Attr.CharMaxHP.Length > 0
            ? client.Attr.CharMaxHP[0]
            : 100;
        if (maxHp <= 0) maxHp = 100;

        if (dmg < 0) dmg = 0;
        int dmgCap = 2 * maxHp;
        if (dmg > dmgCap)
        {
            Logger.Info(client, $"[709] damage clamped from {dmg} to {dmgCap} (maxHp={maxHp})");
            dmg = dmgCap;
        }

        int oldHp = client.Attr.CharHP;
        int newHp = Math.Clamp(oldHp - dmg, 0, maxHp);
        client.Attr.CharHP = newHp;
        if (newHp <= 0) client.Attr.Death = 1;

        Logger.Info(client, $"[709] apply dmg={dmg} {resolved} shooterId={hit.shooterId} hp:{oldHp}->{newHp}/{maxHp}");

        BroadcastHealth(client, newHp, maxHp);
    }

    private void BroadcastHealth(Client target, int curHp, int maxHp)
    {
        float ratio = (float)curHp / maxHp;
        if (ratio < 0f) ratio = 0f;
        if (ratio > 1f) ratio = 1f;

        foreach (Client c in _clientManager.GetAll())
        {
            try
            {
                CsCsProtoStructurePacket<HealthSyncNtf> packet = CsProtoResponse.HealthSyncNtf;
                packet.Structure.NetID = (int)target.Character.Id;
                packet.Structure.Health = ratio;
                packet.Structure.HealthRecover = ratio;
                c.SendCsProtoStructurePacket(packet);
            }
            catch (Exception ex)
            {
                Logger.Error($"[709] BroadcastHealth to {c.Identity}: {ex.Message}");
            }
        }
    }
}
