using System;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System;
using Arrowgene.MonsterHunterOnline.Service.System.CharacterSystem;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

/// <summary>
/// Handles client request to settle battle/instance results (CS_CMD_BATTLE_RESULT_REQ).
/// Responds with CSInstanceResultRsp containing minimal result information.
/// </summary>
public class BattleResultReqHandler : CsProtoStructureHandler<CSInstanceResultReq>
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(BattleResultReqHandler));

    private readonly CharacterManager _characterManager;

    public BattleResultReqHandler(CharacterManager characterManager)
    {
        _characterManager = characterManager;
    }

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_RESULT_REQ;

    public override void Handle(Client client, CSInstanceResultReq req)
    {
        try
        {
            Logger.Info(client, $"Received battle result request: PlayerID={req.PlayerID} RewardFlag={req.RewardFlag}");

            // Basic validation: ensure client has character
            if (client.Character == null)
            {
                Logger.Error(client, "BattleResultReq: no character attached to client");
                return;
            }

            // TODO: perform reward calculation / persistence here based on req.RewardFlag

            // Build a minimal result response
            var rsp = new CSInstanceResultRsp
            {
                LevelID = client.State?.levelId ?? 0,
                GameMode = (int)GameMode.Story,
                HuntingMode = 0,
            };

            try
            {
                
            }
            catch { }

            // Send response back to the requesting client
            var packet = new CsCsProtoStructurePacket<CSInstanceResultRsp>(CS_CMD_ID.CS_CMD_BATTLE_RESULT_RSP);
            packet.Structure = rsp;
            client.SendCsProtoStructurePacket(packet);

            Logger.Info(client, "Sent CSInstanceResultRsp (battle result)");
        }
        catch (Exception ex)
        {
            Logger.Exception(client, ex);
        }
    }
}
