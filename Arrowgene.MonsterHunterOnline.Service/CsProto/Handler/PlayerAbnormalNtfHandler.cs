using System;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

/// <summary>
/// Client-authoritative knockdown/ragdoll state upload. Follows a 709 when the hit
/// triggers a reaction. We store it on the session, log it to build a buff-ID catalog,
/// and rebroadcast so other clients see the downed animation.
/// </summary>
public class PlayerAbnormalNtfHandler : CsProtoStructureHandler<CSPlayerAbnormalNtf>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(PlayerAbnormalNtfHandler));

    private readonly ClientManager _clientManager;

    public PlayerAbnormalNtfHandler(ClientManager clientManager)
    {
        _clientManager = clientManager;
    }

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_PLAYER_ABNORMAL_NTF;

    public override void Handle(Client client, CSPlayerAbnormalNtf req)
    {
        if (client.Character == null)
        {
            Logger.Error(client, "[614] abnormal ntf from client without character");
            return;
        }

        Logger.Info(client, $"[614] abnormal first={req.FirstBuffId} firstOwner={req.FirstBuffOwner} second={req.SecondBuffId} secondOwner={req.SecondBuffOwner} hittype={req.hittype} result={req.result} param1={req.param1} change={req.change}");

        client.LastAbnormal = req;

        foreach (Client c in _clientManager.GetAll())
        {
            if (c == client) continue;
            try
            {
                c.SendCsProtoStructure(CS_CMD_ID.CS_CMD_PLAYER_ABNORMAL_NTF, req);
            }
            catch (Exception ex)
            {
                Logger.Error($"[614] rebroadcast to {c.Identity}: {ex.Message}");
            }
        }
    }
}
