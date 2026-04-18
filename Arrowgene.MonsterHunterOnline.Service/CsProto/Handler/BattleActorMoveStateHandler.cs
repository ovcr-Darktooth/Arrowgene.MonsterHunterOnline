using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using System;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class BattleActorMoveStateHandler : CsProtoStructureHandler<ActorMoveState>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(CreateRoleReqHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_ACTOR_MOVESTATE;


    public override void Handle(Client client, ActorMoveState req)
    {
        // Copy values to avoid sharing the same CSVec3 instance (prevents aliasing/overwrite issues)
        client.State.Position = new CSVec3(req.Location.x, req.Location.y, req.Location.z);
        
        //Logger.Info($"Pos X:{req.Location.x} Y:{req.Location.y} Z:{req.Location.z} from {client.Identity}");

        // Forward movement to other connections for the same character (different port/identity)
        try
        {
            var ntf = CsProtoResponse.ActorMoveStateNtf;
            ntf.Structure.NetObjId = client.Character?.Id ?? 0;
            ntf.Structure.ActorMoveState = req;

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
                        Logger.Error($"Send move ntf to {other.Identity}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Forward move ntf error: {ex.Message}");
        }

        /*CsCsProtoStructurePacket<ActorMoveStateNtf> actorMoveStateNtf = CsProtoResponse.ActorMoveStateNtf;
        actorMoveStateNtf.Structure.NetObjId = client.Character.Id;
        actorMoveStateNtf.Structure.ActorMoveState = req;
        // client.SendCsProtoStructurePacket(actorMoveStateNtf);

        // construire la notification (existant dans ton code)
        var ntf = CsProtoResponse.ActorMoveStateNtf;
        ntf.Structure.NetObjId = client.Character.Id;
        ntf.Structure.ActorMoveState = req;

        // envoyer à tous les autres clients
        foreach (var other in PlayerState.Server.ClientManager.GetAll())
        {
            if (other == client) continue;
            try { other.SendCsProtoStructurePacket(ntf); }
            catch (Exception ex) { Logger.Error($"Send move ntf to {other.Identity}: {ex.Message}"); }
        }

        PlayerState.Server?.MonsterAI?.PlayerMoved(client);
        */

        PlayerState.Server?.MonsterAI?.PlayerMoved(client);
    }
}