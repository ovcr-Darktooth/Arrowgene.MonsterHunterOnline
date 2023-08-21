using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using System.Collections.Generic;
using System.Security.Policy;
using System.Globalization;
using static System.Formats.Asn1.AsnWriter;
using System;

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
        //client.SendCsProtoStructurePacket(sendEntity);

        /*for (int i = 30; i < 50; i++)
        {
            sendEntity.Structure.InitMode = (byte)i;
            client.SendCsProtoStructurePacket(sendEntity);
        }*/


        //Looks a bit more interesing way to spawn a entity
        CsProtoStructurePacket<CSSceneObjAppearNtfList> entityAppear = CsProtoResponse.SceneObjAppearNtfList;
        CSSceneObjAppearNtf firstEntity = new CSSceneObjAppearNtf();

        firstEntity.NetID = 63; //63, should be ok for no conflict, if it's logic see playerstate : 9 / 3 / 10 / 12 /13
        firstEntity.EntityName = "UncleMerchant"; //npcdatanew.dat_NPCData
        //firstEntity.ClassName = "EmCommon"; //npcdatanew.dat_NPCData
        firstEntity.ClassName = "MHMonsterSpawnPoint"; //level/hub_001
        firstEntity.Pose = new CSQuatT()
        {
            q = new CSQuat()
            {
                v = new CSVec3()
                {
                    x = 10,
                    y = 10,
                    z = 10
                },
                w = 10
            },
            t = new CSVec3()
            {
                x = 408.59229f,
                y = 366.01309f,
                z = 88.239403f,
            }
        };

        //scene obj type id, defined by each scene obj
        firstEntity.SubTypeID = 63; // is it region specific entity id ? from mission0_NPCs.csv or leveldata_NPCs.csv
        firstEntity.Sync2CE = 0; // google translate : Whether to synchronously generate to CE, 0 is no, others are required
        firstEntity.SpawnType = 0; // bone related spawn, 0 is absolute, 1 is bone pos
        firstEntity.Bone = 0;
        firstEntity.Holder = 0; // dependent obj netid ?
        firstEntity.Owner = 0; //object ownernetid ?
        firstEntity.Faction = 3; // scripts/ai/faction.xml civilan is the 3rd so 3 ?
        firstEntity.RegionId = -1; //common/leveldata/hub_001 for the entity 63/NPC_39002 regionid is -1
        firstEntity.UsrData = new List<byte>(); //user data, idk what suposed to go here
        firstEntity.EntGUID = HexStringToULong("44454658350C7105"); //common/leveldata/hub_001 for the entity 63/NPC_39002 EntGUID is 44454658350C7105
        firstEntity.PropertityFile = "npc_common.xml"; //npcdatanew.dat_NPCData
        firstEntity.MHSpawnType = 6; // idk
        firstEntity.BTState = "SFJSFSF"; //idk, found it in : scripts\ai\behaviortree\behaviortree.lua
        firstEntity.BBVars = new CSBBVarList(); // don't tell me it's : scripts\ai\behaviortree\npc\common\monsterblackboard.xml
        //note : scripts/ai/logic/blackboard.lua : nil = unset the var, so having an empty list looks "good" for me
        firstEntity.Buff = new List<byte>();
        firstEntity.ParentID = 0;
        firstEntity.ParentGUID = 0;

        entityAppear.Structure.Appear.Add(firstEntity);


        client.SendCsProtoStructurePacket(entityAppear);

        client.SendCsProtoStructurePacket(sendEntity);

    }

    public static ulong HexStringToULong(string hexString)
    {
        ulong result = 0;
        foreach (char c in hexString)
        {
            result <<= 4;
            if (c >= '0' && c <= '9')
                result += (ulong)(c - '0');
            else if (c >= 'A' && c <= 'F')
                result += (ulong)(c - 'A' + 10);
            else if (c >= 'a' && c <= 'f')
                result += (ulong)(c - 'a' + 10);
            else
                throw new ArgumentException("Invalid character in the input string.");
        }
        return result;
    }
}
