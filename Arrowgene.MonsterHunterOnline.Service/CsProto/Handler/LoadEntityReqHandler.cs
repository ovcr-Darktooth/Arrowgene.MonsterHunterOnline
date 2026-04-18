using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.ExtraStructures;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class LoadEntityReqHandler : CsProtoStructureHandler<LoadEntityReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(LoadEntityReqHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_LOAD_ENTITY_REQ;

    private MonsterAIManager _monsterAI;

    public LoadEntityReqHandler(MonsterAIManager monsterAI)
    {
        _monsterAI = monsterAI;
    }

    public override void Handle(Client client, LoadEntityReq req)
    {
        Logger.Debug(req.JsonDump());

        // Phase 3 of the 3-phase spawn protocol:
        // Client sent CMD 534 (LoadEntityReq) requesting full entity data
        // after we sent CMD 533 (EntityAppearNtfIdList) in Phase 1.
        //
        // Two-entity model (confirmed via IDA analysis of CryGame.dll):
        //   CMD 662 → CMonsterSpawner::SpawnMonsters → type-1 CMonster_Derived (logic: AI, hitboxes, locomotion)
        //   CMD 663 → sub_112A3AC0 → type-8 entity (render shell: mesh, animations)
        // Both MUST share the same NetId so CMD 641 locomotion updates reach the visible mesh.
        // CMD 663 with NetId=0 creates a static render entity that never receives locomotion updates.

        CSVec3 spawnPos = client.State.PendingMonsterSpawnPos ?? client.State.Position;

        for (int i = 0; i < req.LogicEntityId.Count; i++)
        {
            uint netId = req.LogicEntityId[i];

            // CMD 662 → type-1 CMonster_Derived: AI, hitboxes, locomotion receiver
            CsCsProtoStructurePacket<MonsterAppearNtf> monsterAppearNtf = CsProtoResponse.MonsterAppearNtf;
            monsterAppearNtf.Structure.NetId = (int)netId;
            monsterAppearNtf.Structure.SpawnType = 1;
            monsterAppearNtf.Structure.MonsterInfoId = 50080;
            monsterAppearNtf.Structure.EntGuid = 12345;
            monsterAppearNtf.Structure.Name = "M008_RaptorCrimson";
            monsterAppearNtf.Structure.Class = "EmCommon";
            monsterAppearNtf.Structure.Pose = new CSQuatT(spawnPos, new CSQuat(1.0f, 0, 0, 0));
            monsterAppearNtf.Structure.Faction = 2;
            monsterAppearNtf.Structure.BTState = "Idle";
            monsterAppearNtf.Structure.BBVars.Vars.Add(new CSBBVar("IsMonster",            new CSBBBool(true)));
            monsterAppearNtf.Structure.BBVars.Vars.Add(new CSBBVar("MaxHealth",            new CSBBInt { value = 5000 }));
            monsterAppearNtf.Structure.BBVars.Vars.Add(new CSBBVar("TargetSrvID",          new CSBBInt { value = 0 }));
            monsterAppearNtf.Structure.BBVars.Vars.Add(new CSBBVar("TargetID",             new CSBBInt { value = 0 }));
            monsterAppearNtf.Structure.BBVars.Vars.Add(new CSBBVar("Flag_Invulnerability", new CSBBBool(false)));
            monsterAppearNtf.Structure.BBVars.Vars.Add(new CSBBVar("RegionTimeRecord",     new CSBBInt { value = 0 }));
            monsterAppearNtf.Structure.Dead = 0;
            monsterAppearNtf.Structure.ParentGuid = 0;
            monsterAppearNtf.Structure.LastChildId = -1;
            monsterAppearNtf.Structure.LcmState.MonsterID = netId;
            monsterAppearNtf.Structure.LcmState.AnimSeqName = "Idle";
            monsterAppearNtf.Structure.LcmState.MonsterPos = spawnPos;
            monsterAppearNtf.Structure.LcmState.MonsterRot = new CSQuat(1.0f, 0, 0, 0);
            monsterAppearNtf.Structure.LcmState.TargetSrvID = 0;
            client.SendCsProtoStructurePacket(monsterAppearNtf);

            // CMD 663 → type-8 render shell (NetId=0 is the only safe value — any real netId crashes).
            // ParentGuid=12345 links this render entity to the CMD 662 logic entity (EntGuid=12345)
            // via CryEngine's entity hierarchy: the visual shell follows the parent's transform
            // automatically, without needing its own CMD 641 locomotion packets.
            CsCsProtoStructurePacket<MonsterAppearNtfList> renderSpawn = CsProtoResponse.MonsterAppearNtfList;
            renderSpawn.Structure.Appear.Add(new MonsterAppearNtf()
            {
                NetId = 0,
                SpawnType = 1,
                MonsterInfoId = 50080,
                Pose = new CSQuatT(spawnPos, new CSQuat(1.0f, 0, 0, 0)),
                ParentGuid = 12345,
            });
            client.SendCsProtoStructurePacket(renderSpawn);

            // CMD 528: activate the type-1 logic entity
            CsCsProtoStructurePacket<MonsterActiveState> activeState = CsProtoResponse.MonsterActiveState;
            activeState.Structure.SyncTime = 0;
            activeState.Structure.ActiveState = 1;
            activeState.Structure.MonsterId = netId;
            activeState.Structure.Position = new XYZPosition() { x = spawnPos.x, y = spawnPos.y, z = spawnPos.z };
            activeState.Structure.Rotation = new Quaternion() { x = 0, y = 0, z = 0, w = 1 };
            client.SendCsProtoStructurePacket(activeState);

            _monsterAI.Spawn(netId, 0, 50080, spawnPos);
        }
    }
}