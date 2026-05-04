using System;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class BattleActorIdleMoveHandler : CsProtoStructureHandler<ActorIdleMove>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(BattleActorIdleMoveHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_ACTOR_IDLEMOVE;

    public override void Handle(Client client, ActorIdleMove req)
    {
        client.State.Position = new CSVec3(req.Location.x, req.Location.y, req.Location.z);

        try
        {
            var ntf = CsProtoResponse.ActorIdleMoveNtf;
            ntf.Structure.NetObjId = client.Character?.Id ?? 0;
            ntf.Structure.ActorIdleMove = req;

            var clients = PlayerState.Server?.ClientManager?.GetAll();
            if (clients != null)
            {
                foreach (var other in clients)
                {
                    if (other == null) continue;
                    if (other == client) continue;
                    if (other.Character == null) continue;
                    if (client.Character == null) continue;
                    if (other.Character.Id != client.Character.Id) continue;

                    other.State.Position = new CSVec3(req.Location.x, req.Location.y, req.Location.z);
                    try
                    {
                        other.SendCsProtoStructurePacket(ntf);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Send idlemove ntf to {other.Identity}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Forward idlemove ntf error: {ex.Message}");
        }

        PlayerState.Server?.MonsterAI?.PlayerMoved(client);
    }
}
