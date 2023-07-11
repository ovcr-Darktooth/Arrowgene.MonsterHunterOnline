﻿using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class CsCmdBattleActorStopMoveHandler : ICsProtoHandler
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(CsCmdBattleActorStopMoveHandler));


    public CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_ACTOR_STOPMOVE;


    public void Handle(Client client, CsProtoPacket packet)
    {
        CSActorStopmove req = new CSActorStopmove();
        req.Read(packet.NewBuffer());
      
        //Logger.Info($"stopmove:{client.State._spawnPlayer.NetObjId} sync:{req.SyncTime} loc:{req.Location} actorrot:{req.ActorRot}");
        client.SendCsPacket(NewCsPacket.ActorStopmoveNtf(new CSActorStopmoveNtf()
        {
            NetObjId = client.State._spawnPlayer.NetObjId,
            ActorStopmove = req
        }));
    }
}