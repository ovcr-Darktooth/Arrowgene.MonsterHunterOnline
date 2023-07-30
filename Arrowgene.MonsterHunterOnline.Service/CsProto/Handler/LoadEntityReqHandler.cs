using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class LoadEntityReqHandler : CsProtoStructureHandler<CSLoadEntityReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(LoadEntityReqHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_LOAD_ENTITY_REQ;


    public override void Handle(Client client, CSLoadEntityReq req)
    {
        Logger.Info($"Entity spawned clientside ?");


        Logger.Info($"id size= {req.LogicEntityID.Count}\ntype size= {req.LogicEntityType.Count}");
        Logger.Info($"id= {req.LogicEntityID[0]}\ntype= {req.LogicEntityType[0]}");
        CsProtoStructurePacket<CSSpawnSrvEntList> sendEntity = CsProtoResponse.SpawnSrvEntList;


        sendEntity.Structure.InitMode = 0; // default = 0 ?
        sendEntity.Structure.EntList = new List<CSSpawnSrvEnt>();

        CSSpawnSrvEnt leNPC = new CSSpawnSrvEnt();
        leNPC.Name = "黑心的肯";
        leNPC.NetObjID = 63;
        leNPC.Position = new XYZPosition()
        { 
            x= 408.59229f,
            y= 366.01309f,
            z= 88.239403f,
        };// 408.59229,366.01309,88.239403
        leNPC.Rotation = new Quaternion()
        {
            x = 0,
            y = 0,
            z = 0,
            w = 0
        };//0.99984771,0,0,-0.017452469
        leNPC.Scale = 1.0f;

        sendEntity.Structure.EntList.Add(leNPC);

       
        //isn't a client to server cmd ? useless here then ? 
        client.SendCsProtoStructurePacket(sendEntity);

        /*for (int i = 30; i < 50; i++)
        {
            sendEntity.Structure.InitMode = (byte)i;
            client.SendCsProtoStructurePacket(sendEntity);
        }*/

    }
}
