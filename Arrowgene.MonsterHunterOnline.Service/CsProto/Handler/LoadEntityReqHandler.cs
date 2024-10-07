using System;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.System;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class LoadEntityReqHandler : CsProtoStructureHandler<LoadEntityReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(LoadEntityReqHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_LOAD_ENTITY_REQ;


    public LoadEntityReqHandler()
    {
    }

    public override void Handle(Client client, LoadEntityReq req)
    {
        Logger.Debug(req.JsonDump());


        LogicEntityId id = new LogicEntityId();
        id.Id = req.LogicEntityId[0];
        /*Logger.Debug($"{req.LogicEntityId[0]}");
        Logger.Debug($"{req.LogicEntityType[0]}"); 
        Logger.Debug($"{(LogicEntityType)req.LogicEntityType[0]}");*/

        id.Type = (LogicEntityType)req.LogicEntityType[0];
        /*Logger.Debug($"{(int)id.Type}");*/

        /*
            NetId = 0;
            SpawnType = 0;
            MonsterInfoId = 0;
            EntGuid = 0;
            Name = "";
            Class = "";
            Pose = new CSQuatT();
            Faction = 0;
            BTState = "";
            BBVars = new CSBBVarList();
            Dead = 0;
            LcmState = new CSMonsterLocomotion();
            AttrInit = new List<CSAttrData>();
            ProjIds = new List<CSAmmoInfo>();
            Buff = new List<byte>();
            ParentGuid = 0;
            LastChildId = 0;
        */

        CsCsProtoStructurePacket <MonsterAppearNtf> monsterAppearNtf = CsProtoResponse.MonsterAppearNtf;
        monsterAppearNtf.Structure.NetId = (int)233; // idk, EntityId from mission_mission0.xml ? 
        monsterAppearNtf.Structure.SpawnType = (short)1; //monster type from monsterdata.dat_monsters ?
        monsterAppearNtf.Structure.MonsterInfoId = 60030; //monster id from monsterdata.dat_monsters ?
        monsterAppearNtf.Structure.Name = "Em003"; //entity name from monsterdata.dat_monsters ?
        monsterAppearNtf.Structure.Class = "EmCommon"; //entity class from monsterdata.dat_monsters ?
        monsterAppearNtf.Structure.EntGuid = Convert.ToUInt64("0x475A9E689F8C9FEE", 16); // uuid from mission_mission0.xml ?
        //monsterAppearNtf.Structure.EntGuid = (ulong)"4EF80F039B52F72C"; //415
        monsterAppearNtf.Structure.ParentGuid = 233; // idk, EntityId from mission_mission0.xml ? 
        monsterAppearNtf.Structure.Dead = 1; // 1 is dead ? from a dev POV, 1 is dead
        //monsterAppearNtf.Structure.Pose.t = client.State.Position;
        monsterAppearNtf.Structure.Pose.q = new CSQuat()
        {
            //Rotation
            /*v = new CSVec3()
            {
                x = 300f,
                y = 255f,
                z = 358f
            },*/
            v = new CSVec3()
            {
                x = 286.36642f,
                y = 227.33673f,
                z = 358f
            },
            w = 10f,
        };

        monsterAppearNtf.Structure.Pose.t = new CSVec3()
        {
            //Position
            /*x = 300f,
            y = 255f,
            z = 358f*/
            x = 286.36642f,
            y = 227.33673f,
            z = 358f
        };

        //With a not touched LcmState it still crashes
        monsterAppearNtf.Structure.LcmState.SteeringEnabled = 0; // idk, maybe in pause mode it can be "easier"
        monsterAppearNtf.Structure.LcmState.AnimSeqName = "Idle"; // idk where tf I found this
        monsterAppearNtf.Structure.LcmState.MonsterID = 60030; //monster id from monsterdata.dat_monsters ?
        //monsterAppearNtf.Structure.LcmState.MonsterPos = client.State.Position;
        
        monsterAppearNtf.Structure.LcmState.MonsterPos = new CSVec3()
        {
            x = 300f,
            y = 255f,
            z = 358f
        };
        monsterAppearNtf.Structure.LcmState.TargetID = 3;
        monsterAppearNtf.Structure.LcmState.TargetSrvID = 1;//7310;
        monsterAppearNtf.Structure.LcmState.SyncTime = 1;
        monsterAppearNtf.Structure.LcmState.SyncFlag = 1;

        //   Logger.Debug(monsterAppearNtf.Structure.JsonDump());
        CsCsProtoStructurePacket<MonsterAppearNtfList> monsterAppearNtfList = CsProtoResponse.MonsterAppearNtfList;
        monsterAppearNtfList.Structure.Appear.Add(monsterAppearNtf.Structure);
        client.SendCsProtoStructurePacket(monsterAppearNtfList);


        /*CsCsProtoStructurePacket<MonsterAppearNtf> monsterAppearNtf = CsProtoResponse.MonsterAppearNtf;
        monsterAppearNtf.Structure.NetId = (int)158;
        monsterAppearNtf.Structure.SpawnType = (short)1;
        monsterAppearNtf.Structure.MonsterInfoId = 60251; //268495486
        monsterAppearNtf.Structure.Name = "Em025_u01";
        monsterAppearNtf.Structure.Class = "EmCommon";//"easy" ? "Easy" ?
        monsterAppearNtf.Structure.EntGuid = Convert.ToUInt64("0x475A9E689F8C9FEE", 16);
        //monsterAppearNtf.Structure.EntGuid = (ulong)"4EF80F039B52F72C"; //415
        monsterAppearNtf.Structure.ParentGuid = 158;
        monsterAppearNtf.Structure.Dead = 1;
        //monsterAppearNtf.Structure.Pose.t = client.State.Position;
        monsterAppearNtf.Structure.Pose.q = new CSQuat()
        {
            v = new CSVec3()
            {
                x = 0.95105654f,
                y = 0f,
                z = 0f
            },
            w = 0.309017f,
        };

        monsterAppearNtf.Structure.Pose.t = new CSVec3()
        {
            x = 1411.0615f,
            y = 738.8645f,
            z = 30.007999f
        };

        
        monsterAppearNtf.Structure.LcmState.SteeringEnabled = 0;
        monsterAppearNtf.Structure.LcmState.AnimSeqName = "Idle"; 
        monsterAppearNtf.Structure.LcmState.MonsterID = 60251;
        //monsterAppearNtf.Structure.LcmState.MonsterPos = client.State.Position;

        monsterAppearNtf.Structure.LcmState.MonsterPos = new CSVec3()
        {
            x = 300f,
            y = 255f,
            z = 358f
        };
        monsterAppearNtf.Structure.LcmState.TargetID = 3;
        monsterAppearNtf.Structure.LcmState.TargetSrvID = 1;//7310;
        monsterAppearNtf.Structure.LcmState.SyncTime = 1;
        monsterAppearNtf.Structure.LcmState.SyncFlag = 1;

        //   Logger.Debug(monsterAppearNtf.Structure.JsonDump());
        CsCsProtoStructurePacket<MonsterAppearNtfList> monsterAppearNtfList = CsProtoResponse.MonsterAppearNtfList;
        monsterAppearNtfList.Structure.Appear.Add(monsterAppearNtf.Structure);
        client.SendCsProtoStructurePacket(monsterAppearNtfList);*/
    }
}