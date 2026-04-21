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
/// Player → monster hit packet. Mirror of cmd 709 for the opposite direction:
/// the client runs HitCol sweeps on the player's weapon locally and reports
/// hits against monsters. Server resolves attacker hash against AttackData,
/// decrements MonsterAI.CurrentHp, broadcasts health, and despawns on death.
/// </summary>
public class BattleDMGHandler : CsProtoStructureHandler<BattleDMG>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(BattleDMGHandler));

    private readonly AttackDataTable _attackData;
    private readonly MonsterAIManager _monsterAi;
    private readonly ClientManager _clientManager;

    public BattleDMGHandler(
        AttackDataTable attackData,
        MonsterAIManager monsterAi,
        ClientManager clientManager)
    {
        _attackData = attackData;
        _monsterAi = monsterAi;
        _clientManager = clientManager;
    }

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_DMG_VERIFY;

    private const int FallbackDamage = 200;

    public override void Handle(Client client, BattleDMG req)
    {
        // Target lookup: client reports CryEngine runtime entity IDs which don't match
        // our server-assigned NetId (same quirk as cmd 709). Strict-match first, then
        // fall back to "nearest live monster to the player" so gameplay keeps flowing.
        MonsterAI monster;
        if (_monsterAi.TryGet(req.targetId, out monster))
        {
            // direct hit by netId
        }
        else
        {
            CSVec3 playerPos = client.State?.Position ?? client.State?.InitSpawnPos;
            monster = playerPos != null ? _monsterAi.FindNearestLiveMonster(playerPos) : null;
            if (monster == null)
            {
                Logger.Info(client, $"[705] no live monster (targetId={req.targetId} shooterId={req.shooterId}); dropping");
                EchoDmg(client, req);
                return;
            }
            Logger.Info(client, $"[705] targetId=0x{req.targetId:X8} unmapped; fallback to nearest monster netId={monster.NetId}");
        }

        if (monster.CurrentHp <= 0)
        {
            Logger.Info(client, $"[705] monster {monster.NetId} already dead; ignoring hit");
            EchoDmg(client, req);
            return;
        }

        int dmg;
        string resolved;
        if (_attackData.TryGetIdByHash(req.hashAttacker, out int attackDataId)
            && _attackData.TryGet(attackDataId, out AttackData data))
        {
            dmg = data.DamagePower;
            resolved = $"attackData={attackDataId} ({data.AttackName})";
        }
        else
        {
            dmg = FallbackDamage;
            resolved = $"fallback (HASH-MISS hashAttacker=0x{req.hashAttacker:X8} hashWeaponClass=0x{req.hashWeaponClass:X8} hashFireMode=0x{req.hashFireMode:X8} hashCurEvent=0x{req.hashCurEvent:X8} skillSeq={req.skillSeq})";
        }

        if (dmg < 0) dmg = 0;
        int dmgCap = 2 * monster.MaxHp;
        if (dmg > dmgCap)
        {
            Logger.Info(client, $"[705] damage clamped from {dmg} to {dmgCap} (monsterMaxHp={monster.MaxHp})");
            dmg = dmgCap;
        }

        int oldHp = monster.CurrentHp;
        int newHp = Math.Clamp(oldHp - dmg, 0, monster.MaxHp);
        monster.CurrentHp = newHp;

        Logger.Info(client, $"[705] apply dmg={dmg} {resolved} monsterNetId={monster.NetId} hp:{oldHp}->{newHp}/{monster.MaxHp}");

        BroadcastMonsterHealth(monster);
        EchoDmg(client, req);

        if (newHp <= 0)
        {
            Logger.Info(client, $"[705] monster {monster.NetId} died from player hit");
            _monsterAi.BroadcastMonsterActiveState(monster.NetId, 0, monster.Position, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            //_monsterAi.Despawn(monster.NetId);
            monster.makeDie();
        }
    }

    private void BroadcastMonsterHealth(MonsterAI monster)
    {
        float ratio = (float)monster.CurrentHp / monster.MaxHp;
        if (ratio < 0f) ratio = 0f;
        if (ratio > 1f) ratio = 1f;

        foreach (Client c in _clientManager.GetAll())
        {
            try
            {
                CsCsProtoStructurePacket<HealthSyncNtf> packet = CsProtoResponse.HealthSyncNtf;
                packet.Structure.NetID = (int)monster.NetId;
                packet.Structure.Health = ratio;
                packet.Structure.HealthRecover = ratio;
                c.SendCsProtoStructurePacket(packet);
            }
            catch (Exception ex)
            {
                Logger.Error($"[705] BroadcastMonsterHealth to {c.Identity}: {ex.Message}");
            }
        }
    }

    private static void EchoDmg(Client client, BattleDMG req)
    {
        // Legacy behavior: echo the damage packet back so the client renders its own hit VFX.
        CsCsProtoStructurePacket<BattleDMG> dmgInfo = CsProtoResponse.BattleDMG;
        dmgInfo.Structure.shooterId = req.shooterId;
        dmgInfo.Structure.targetId = req.targetId;
        dmgInfo.Structure.weaponId = req.weaponId;
        dmgInfo.Structure.projectileId = req.projectileId;
        dmgInfo.Structure.material = req.material;
        dmgInfo.Structure.type = req.type;
        dmgInfo.Structure.bulletType = req.bulletType;
        dmgInfo.Structure.damageMin = req.damageMin;
        dmgInfo.Structure.pierce = req.pierce;
        dmgInfo.Structure.partId = req.partId;
        dmgInfo.Structure.pos = req.pos;
        dmgInfo.Structure.lpos = req.lpos;
        dmgInfo.Structure.dir = req.dir;
        dmgInfo.Structure.normal = req.normal;
        dmgInfo.Structure.lnorm = req.lnorm;
        dmgInfo.Structure.attackDir = req.attackDir;
        dmgInfo.Structure.tScarDir = req.tScarDir;
        dmgInfo.Structure.tPos = req.tPos;
        dmgInfo.Structure.tUp = req.tUp;
        dmgInfo.Structure.tNormal = req.tNormal;
        dmgInfo.Structure.localnormangle = req.localnormangle;
        dmgInfo.Structure.shakeStrength = req.shakeStrength;
        dmgInfo.Structure.shakeDurationTime = req.shakeDurationTime;
        dmgInfo.Structure.shakeStillTime = req.shakeStillTime;
        dmgInfo.Structure.projectileClassId = req.projectileClassId;
        dmgInfo.Structure.weaponClassId = req.weaponClassId;
        dmgInfo.Structure.remote = req.remote;
        dmgInfo.Structure.damageLevel = req.damageLevel;
        dmgInfo.Structure.attackType = req.attackType;
        dmgInfo.Structure.hitType = req.hitType;
        dmgInfo.Structure.defenseResult = req.defenseResult;
        dmgInfo.Structure.HitIndex = req.HitIndex;
        dmgInfo.Structure.shooterSrvId = req.shooterSrvId;
        dmgInfo.Structure.targetSrvId = req.targetSrvId;
        dmgInfo.Structure.weaponSrvId = req.weaponSrvId;
        dmgInfo.Structure.projectileSrvId = req.projectileSrvId;
        dmgInfo.Structure.hashWeaponClass = req.hashWeaponClass;
        dmgInfo.Structure.hashFireMode = req.hashFireMode;
        dmgInfo.Structure.hashAttacker = req.hashAttacker;
        dmgInfo.Structure.hashMeleeParams = req.hashMeleeParams;
        dmgInfo.Structure.hashCurEvent = req.hashCurEvent;
        dmgInfo.Structure.skillResID = req.skillResID;
        dmgInfo.Structure.skillSeq = req.skillSeq;
        dmgInfo.Structure.curStamina = req.curStamina;
        client.SendCsProtoStructurePacket(dmgInfo);
    }
}
