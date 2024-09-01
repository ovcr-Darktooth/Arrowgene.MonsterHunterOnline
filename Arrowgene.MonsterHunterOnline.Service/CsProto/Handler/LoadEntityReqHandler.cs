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
        monsterAppearNtf.Structure.NetId = (int)415;
        monsterAppearNtf.Structure.SpawnType = (short)1;
        monsterAppearNtf.Structure.MonsterInfoId = 60030; //268495486
        monsterAppearNtf.Structure.Name = "Em003";
        monsterAppearNtf.Structure.Class = "EmCommon";
        monsterAppearNtf.Structure.EntGuid = 415; //415
        monsterAppearNtf.Structure.ParentGuid = 1;
        //monsterAppearNtf.Structure.Pose.t = client.State.Position;
        monsterAppearNtf.Structure.Pose.q = new CSQuat()
        {
            v = new CSVec3()
            {
                x = 300f,
                y = 255f,
                z = 358f
            },
            w = 10f,
        };

        monsterAppearNtf.Structure.Pose.t = new CSVec3()
        {
            x = 300f,
            y = 255f,
            z = 358f
        };

        monsterAppearNtf.Structure.LcmState.SteeringEnabled = 1;
        monsterAppearNtf.Structure.LcmState.AnimSeqName = "Idle";
        monsterAppearNtf.Structure.LcmState.MonsterID = 60030;
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
    }
}