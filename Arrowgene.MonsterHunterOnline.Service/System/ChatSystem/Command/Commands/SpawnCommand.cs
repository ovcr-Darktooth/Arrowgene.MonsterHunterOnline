using System.Collections.Generic;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.ExtraStructures;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.System.UnlockSystem;
using Arrowgene.MonsterHunterOnline.Service.CsProto;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.System.ChatSystem.Command.Commands
{

    /// <summary>
    /// Spawns entities
    /// </summary>
    public class SpawnCommand : ChatCommand
    {

        private static readonly ServiceLogger Logger =
            LogProvider.Logger<ServiceLogger>(typeof(SpawnCommand));

        //public override AccountType Account => AccountType.Admin;
        public override AccountType Account => AccountType.User;
        public override string Key => "spawn";
        public override string HelpText => "usage: /spawn [id] [monster|battle|playerlist|disappear] [spawnType] [name]";

        public override void Execute(string[] command, Client client, ChatMessage message, List<ChatMessage> responses)
        {
            int spawnId = 39002;
            if (command.Length > 0 && !int.TryParse(command[0], out spawnId))
                spawnId = 39002;

            string type = "monster";
            if (command.Length > 1)
                type = command[1];

            short spawnType = 1;
            if (command.Length > 2 && !short.TryParse(command[2], out spawnType))
                spawnType = 1;
            string name = "name";
            if (command.Length > 1)
                name = command[3];
            CsCsProtoStructurePacket<MonsterAppearNtfList> monsterAppearNtfList = CsProtoResponse.MonsterAppearNtfList;

            switch (type)
            {
                case "battle":

                    CsCsProtoStructurePacket<CtrledMonsterAppearNtf> battle = CsProtoResponse.CtrledMonsterAppearNtf;
                    battle.Structure.BaseInfo = new MonsterAppearNtf()
                    {
                        NetId = 0,
                        SpawnType = spawnType,
                        MonsterInfoId = spawnId,
                        EntGuid = 0,
                        Name = "",
                        Class = "",
                        Pose = new CSQuatT(client.State.Position, new CSQuat()),
                        Faction = 0,
                        Dead = 0,
                        ParentGuid = 0,
                        LastChildId = 0,
                        LcmState = new CSMonsterLocomotion() { AnimSeqName = "Attack_HeavyTail" },

                        BTState = "Em025\\BB_Knowledge.xml",
                        BBVars = new CSBBVarList() { Vars = new List<CSBBVar>() { new CSBBVar("ExFlag", new CSBBBool(true)) } },
                    };
                    battle.Structure.OwnerId = 1;
                    battle.Structure.Type = 1;
                    battle.Structure.Duration = 0.0f;
                    client.SendCsProtoStructurePacket(battle);
                    break;
                case "disapear":
                    CSEntityDisappearNtfList entityDisappearNtfList = new();
                    entityDisappearNtfList.Disappear.Add(new CSEntityDisappearNtf() { NetID = spawnId, Reason = 0 });
                    // entityDisappearNtfList.Disappear.Add(new CSEntityDisappearNtf() { NetID = 300, Reason = 0 });
                    client.SendCsPacket(NewCsPacket.EntityDisappearNtfList(entityDisappearNtfList));
                    break;
                case "battleani":
                    CsCsProtoStructurePacket<MonsterSequenceState> seq = CsProtoResponse.MonsterSequenceState;
                    seq.Structure.AnimSeqName = "Attack_HeavyTail";
                    seq.Structure.MonsterID = (uint)spawnId;
                    seq.Structure.CurTime = 1.0F;
                    seq.Structure.Location = client.State.Position;
                    client.SendCsProtoStructurePacket(seq);

                    break;


                case "playerlist":

                    List<AttrSync> attrs = new List<AttrSync>();

                    AttrSync sync;

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 1;
                    sync.BonusId = 1;
                    sync.Data.Int = (int)1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 2;
                    sync.BonusId = 0;
                    sync.Data.String = name;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 3;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 16;
                    sync.BonusId = 1;
                    sync.Data.Int = 100; // max hp
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 20;
                    sync.BonusId = 0;
                    sync.Data.Int = 100; // stamina (needed to move around)
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 21;
                    sync.BonusId = 0;
                    sync.Data.Int = 100; // max stamina (not needed to move around)
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 25;
                    sync.BonusId = 1;
                    sync.Data.Int = 1; // str
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 26;
                    sync.BonusId = 1;
                    sync.Data.Int = 1; // bst
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 27;
                    sync.BonusId = 1;
                    sync.Data.Int = 1; // lck
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 28;
                    sync.BonusId = 1;
                    sync.Data.Int = 1; // vir
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 73;
                    sync.BonusId = 1;
                    sync.Data.Int = 100; // speed
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 107;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 108;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 172;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 200;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 201;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 202;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 203;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 204;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 215;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 216;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 222;
                    sync.BonusId = 0;
                    sync.Data.Bool = true;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 223;
                    sync.BonusId = 0;
                    sync.Data.Bool = false;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 224;
                    sync.BonusId = 0;
                    sync.Data.Bool = false;
                    attrs.Add(sync);

                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 317;
                    sync.BonusId = 0;
                    sync.Data.Int = 1;
                    attrs.Add(sync);

                    uint faceAttrId = 247;
                    // TODO verify this logic
                    for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
                    {
                        sync = new AttrSync();
                        sync.EntityId = (uint)spawnId;
                        sync.AttrId = faceAttrId;
                        sync.BonusId = 0;
                        sync.Data.Int = 1;
                        attrs.Add(sync);
                        faceAttrId++;
                        if (faceAttrId == 272)
                        {
                            faceAttrId = 324;
                        }
                    }

                    //System unlock data
                    sync = new AttrSync();
                    sync.EntityId = (uint)spawnId;
                    sync.AttrId = 236;
                    sync.BonusId = 0;
                    sync.Data.UInt64 = SystemUnlock.GetForLevel(1).ToUInt64();
                    //sync.Data.UInt64 = 8796093022207; //Debug value, everything unlocked
                    //Logger.Info($"System unlock = {sync.Data.UInt64}\n binary : {Convert.ToString((long)sync.Data.UInt64, 2)}");
                    attrs.Add(sync);



                    CSPlayerAppearNtfList playerAppearNtfList = new();
                    playerAppearNtfList.Appear.Add(new CSPlayerAppearNtf()
                    {


                        NetID = spawnId,
                        SessionID = 0,
                        Name = name,
                        Gender = client.Character.Gender,
                        Pose = new CSQuatT(client.State.Position, new CSQuat()),
                        AvatarSetID = 100, //client.Character.AvatarSetId,
                        Health = 100.0f,
                        HealthRecover = 10.0f,
                        Faction = 0,
                        Weapon = 1,
                        State = client.Character.RoleState,
                        HasTeam = 1,
                        TeamHasPwd = 0,
                        RandSeed = 1,
                        Equip = new List<AvatarItem>() {
                    new() { BreakLevel = 1, ColorIndex = 1, EnhanceLevel = 1, ItemType = 1, GemId = 1, Rank = 1, EnhanceRule = 1, SoltCount = 1, WakeLevel = 1, PosIndex = 1, FakeShow = 1 },
                    new() { BreakLevel = 1, ColorIndex = 1, EnhanceLevel = 1, ItemType = 1, GemId = 1, Rank = 1, EnhanceRule = 1, SoltCount = 1, WakeLevel = 1, PosIndex = 1, FakeShow = 1 },
                    new() { BreakLevel = 1, ColorIndex = 1, EnhanceLevel = 1, ItemType = 1, GemId = 1, Rank = 1, EnhanceRule = 1, SoltCount = 1, WakeLevel = 1, PosIndex = 1, FakeShow = 1 },
                    new() { BreakLevel = 1, ColorIndex = 1, EnhanceLevel = 1, ItemType = 1, GemId = 1, Rank = 1, EnhanceRule = 1, SoltCount = 1, WakeLevel = 1, PosIndex = 1, FakeShow = 1 },
                    new() { BreakLevel = 1, ColorIndex = 1, EnhanceLevel = 1, ItemType = 1, GemId = 1, Rank = 1, EnhanceRule = 1, SoltCount = 1, WakeLevel = 1, PosIndex = 1, FakeShow = 1 },
                    new() { BreakLevel = 1, ColorIndex = 1, EnhanceLevel = 1, ItemType = 1, GemId = 1, Rank = 1, EnhanceRule = 1, SoltCount = 1, WakeLevel = 1, PosIndex = 1, FakeShow = 1 },
                },
                        Attr = new List<byte>(),
                        Buff = new List<byte>(),
                        ProjIds = new List<CSAmmoInfo>(),
                        ParentEntityGUID = 0,
                        Type = 1,
                        state1 = 1,
                        state2 = 1,
                        state3 = 1,
                        state4 = 1,
                        AGState = 1,
                        SubState = 1,
                        Guilder = new CSGuilderOutline() { GuildName = "Guild Name" },
                        StarLevel = "2",
                        FacialInfo = client.Character.FacialInfo,
                        //FacialInfo = new short[CsProtoConstant.CS_MAX_FACIALINFO_COUNT] { 5, 5,5,5,5,5,5,5,5,5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,5,5,5,5,5,5 },
                        VehicleID = 1,
                        GrowHighDay = 1,
                        GrowHeight = 1
                    });
                    client.SendCsPacket(NewCsPacket.PlayerAppearNtfList(playerAppearNtfList));







                    List<List<AttrSync>> attrChunks = Util.Chunk(attrs, CsProtoConstant.CS_ATTR_SYNC_LIST_MAX);

                    foreach (List<AttrSync> attrChunk in attrChunks)
                    {
                        if (attrChunk.Count > CsProtoConstant.CS_ATTR_SYNC_LIST_MAX)
                        {
                            Logger.Error(client, "Chunk error");
                        }

                        CsCsProtoStructurePacket<AttrSyncList> attrSyncList = CsProtoResponse.AttrSyncList;
                        for (int i = 0; i < attrChunk.Count; i++)
                        {
                            attrSyncList.Structure.Attr.Add(attrChunk[i]);
                        }

                        client.SendCsProtoStructurePacket(attrSyncList);
                    }
                    break;
                case "monster":
                    // MonsterAppearNtf appear1 = new()
                    // {
                    //     NetId = 0x11,
                    //     SpawnType = 0x22,
                    //     MonsterInfoId = 0x333,
                    //     EntGuid = 0x4444444444444444,
                    //     Name = "Npc_39002",
                    //     Class = "MHMonsterSpawnPoint",
                    //     Pose = new CSQuatT(408.59229f, 366.01309f, 88.239403f, 0.99984771f, 0f, 0f, -0.017452469f),
                    //     Faction = 0x88,
                    //     BTState = "SomeState",
                    //     Dead = 0x99,
                    //     ParentGuid = 0xAA,
                    //     LastChildId = 0xBB,
                    //     LcmState = new CSMonsterLocomotion()
                    //     {
                    //         SteeringEnabled = 1,
                    //         SyncTime = 2,
                    //         MonsterID = 3,
                    //         AnimSeqName = "3",
                    //         PartBoneName = "4",
                    //         SkillID = 5,
                    //         SkillLv = 6,
                    //         SyncFlag = 7,
                    //         TargetID = 8,
                    //         TargetSrvID = 9,
                    //         Flag = 10,
                    //         TargetDis = new CSVec3(1, 2, 3),
                    //         MoveSpeed = new CSVec3(4, 5, 6),
                    //         TargetRot = new CSVec3(7, 8, 9),
                    //         RotSpeed = new CSVec3(1, 2, 3),
                    //         RotSpeedByAnim = 11,
                    //         MonsterPos = new CSVec3(4, 5, 6),
                    //         MonsterRot = new CSQuat(7, 8, 9, 10),
                    //         SkillSpeed = 12.0f,
                    //         RestartAnim = 13,
                    //         RotFlag = 14,
                    //         TargetMultiAttackPos = new List<CSVec3>() { new CSVec3(11, 12, 13) },
                    //         TargetAttackPos = new CSVec3(14, 15, 16),
                    //         NeedTargetAttackPos = 15,
                    //         AckFlag = 16,
                    //         UserParam1 = 17,
                    //         UserParam2 = 18,
                    //         SetRotate = 19,
                    //         SetPos = 20,
                    //         NoTransferCorrection = 21,
                    //         NeedMoveSpeedAcc = 22,
                    //         MoveSpeedAccelerate = new CSVec3(17, 18, 19),
                    //         MoveSpeedAccStart = 23.0f,
                    //         MoveSpeedAccEnd = 24.0f,
                    //         MoveSplineScale = new CSVec3(20, 21, 22)
                    //     },

                    // };
                    // appear1.BBVars.Vars.Add(new CSBBVar("BBVarName", new CSBBInt()));
                    // appear1.AttrInit.Add(new CSAttrData(123, new CSAttrBaseData(new CSVec3(11, 22, 33))));
                    // appear1.ProjIds.Add(new CSAmmoInfo() { NetID = 1, TypeID = 2 });
                    // appear1.Buff.Add(0xAA);
                    // //monsterAppearNtfIdList.Structure.Appear.Add(appear1);

                    monsterAppearNtfList.Structure.Appear.Add(new MonsterAppearNtf()
                    {
                        NetId = 0,
                        SpawnType = spawnType,
                        MonsterInfoId = spawnId,
                        EntGuid = 0,
                        Name = "",
                        Class = "",
                        Pose = new CSQuatT(client.State.Position, new CSQuat()),
                        Faction = 0,
                        Dead = 0,
                        ParentGuid = 0,
                        LastChildId = 0,
                        LcmState = new CSMonsterLocomotion() { AnimSeqName = "Attack_HeavyTail" },
                        //BTState = "BTClientEventFile",
                        BBVars = new CSBBVarList() { Vars = new List<CSBBVar>() { new CSBBVar("Sleep", new CSBBBool(true)) } },
                    });
                    client.SendCsProtoStructurePacket(monsterAppearNtfList);

                    break;
                default:
                    Logger.Info($"Unknown spawn type: {type}");
                    break;
            }
            //CsCsProtoStructurePacket<MonsterAppearNtfList> monsterAppearNtfList = CsProtoResponse.MonsterAppearNtfList;
            //monsterAppearNtfIdList.Structure.Appear.Add(new MonsterAppearNtf()
            //{
            //    NetId = 70,
            //    SpawnType = 0,
            //    MonsterInfoId = 39004,
            //    EntGuid = 0x40C9BC380C0430DE,
            //    Name = "SupplyBox",
            //    Class = "EmCommon",
            //    Pose = new CSQuatT(352.48267f, 1158.7018f, 178.10593f, 0.7009092f, 0f, 0f, 0.71325052f),
            //});
            //monsterAppearNtfIdList.Structure.Appear.Add(new MonsterAppearNtf()
            //{
            //    NetId = 60,
            //    SpawnType = 0,
            //    MonsterInfoId = 39902,
            //    EntGuid = 0x4951325F4A43337B,
            //    Name = "supplyBox",
            //    Class = "EmCommon",
            //    Pose = new CSQuatT(410.97061f, 364.21097f, 88.695702f, 0.98901588f, 0f, 0f, 0.14780943f),
            //});
            //monsterAppearNtfIdList.Structure.Appear.Add(new MonsterAppearNtf()
            //{
            //    NetId = 60,
            //    SpawnType = 0,
            //    MonsterInfoId = 39902,
            //    EntGuid = 0x4951325F4A43337B,
            //    Name = "MHSpawnPoint4",
            //    Class = "MHMonsterSpawnPoint",
            //    Pose = new CSQuatT(410.97061f, 364.21097f, 88.695702f, 0.98901588f, 0f, 0f, 0.14780943f)
            //});
            //monsterAppearNtfList.Structure.Appear.Add(new MonsterAppearNtf()
            //{
            //    NetId = 63,
            //    SpawnType = 1,
            //    MonsterInfoId = 39002,
            //    EntGuid = 0x44454658350C7105,
            //    Name = "Npc_39002",
            //    Class = "MHMonsterSpawnPoint",
            //    Pose = new CSQuatT(408.59229f, 366.01309f, 88.239403f, 0.99984771f, 0f, 0f, -0.017452469f),
            //    Faction = 0,
            //    BTState = "",
            //    Dead = 0,
            //    ParentGuid = 0,
            //    LastChildId = 0
            //});

            //call_handler: CMD:663 CS_CMD_MONSTER_APPEAR_NTF_LIST handler_fn:11DBA1E0, unk:40291264, call_fn:112A4220
            //client_log: $6[Warning][RenderProxy::LoadCharacter] Called with empty filename, Entity: _00000011(EmCommon)


            CsCsProtoStructurePacket<EntityAppearNtfIdList> entityAppearNtfIdList = CsProtoResponse.EntityAppearNtfIdList;
            entityAppearNtfIdList.Structure.LogicEntityId.Add(63);
            entityAppearNtfIdList.Structure.LogicEntityType.Add(1);
            entityAppearNtfIdList.Structure.InitType = 1;
            //client.SendCsProtoStructurePacket(entityAppearNtfIdList);







            //CsCsProtoStructurePacket<EntityAppearNtfIdList> entityAppearNtfIdList = CsProtoResponse.EntityAppearNtfIdList;
            //LogicEntityId leId = new LogicEntityId();
            //leId.Type = LogicEntityType.MH_LETYPE_MONSTER;
            //leId.Id = 63;
            //entityAppearNtfIdList.Structure.InitType = 0;
            //entityAppearNtfIdList.Structure.LogicEntityId.Add(leId.Id);
            //entityAppearNtfIdList.Structure.LogicEntityType.Add((uint)leId.Type);
            //client.SendCsProtoStructurePacket(entityAppearNtfIdList);







            // SceneObjAppearNtf sceneObjAppearNtf = new SceneObjAppearNtf()
            // {
            //     NetId = client.spawnId,
            //     EntityName = "Barrel Bomb S",
            //     ClassName = "Bomb.SmallBucketBomb",
            //     Pose = new CSQuatT() { t = client.State.Position },
            //     SubTypeId = 0,
            //     Sync2CE = 1,
            //     SpawnType = 1,
            //     Bone = 0,
            //     Holder = client.spawnId,
            //     Owner = client.spawnId,
            //     Faction = 0,
            //     RegionId = 0,
            //     EntGuid = 0,
            //     PropertityFile = "",
            //     MHSpawnType = 0,
            //     BTState = "",
            //     BBVars = new CSBBVarList(),
            //     ParentId = 0,
            //     ParentGuid = 0
            // };


            // CsCsProtoStructurePacket<SceneObjAppearNtfList> sceneObjAppearNtfList = CsProtoResponse.SceneObjAppearNtfList;
            // sceneObjAppearNtfList.Structure.Appear.Add(sceneObjAppearNtf);
            // client.SendCsProtoStructurePacket(sceneObjAppearNtfList);



            //CsCsProtoStructurePacket<MonsterAppearNtf> monsterAppearNtfId = CsProtoResponse.MonsterAppearNtf;
            //LogicEntityId leId = new LogicEntityId();
            //leId.Type = LogicEntityType.MH_LETYPE_MONSTER;
            //leId.Id = 50080;
            //monsterAppearNtfId.Structure.NetId = client.spawnId;
            //monsterAppearNtfId.Structure.SpawnType = 0;
            //monsterAppearNtfId.Structure.MonsterInfoId = 0;
            //monsterAppearNtfId.Structure.EntGuid = 0;
            //monsterAppearNtfId.Structure.Name = "";
            //monsterAppearNtfId.Structure.Class = "";
            //monsterAppearNtfId.Structure.Pose = new CSQuatT();
            //monsterAppearNtfId.Structure.Faction = 0;
            //monsterAppearNtfId.Structure.BTState = "";
            //monsterAppearNtfId.Structure.BBVars = new CSBBVarList();
            //monsterAppearNtfId.Structure.Dead = 0;
            //monsterAppearNtfId.Structure.LcmState = new CSMonsterLocomotion();
            //monsterAppearNtfId.Structure.AttrInit = new List<CSAttrData>();
            //monsterAppearNtfId.Structure.ProjIds = new List<CSAmmoInfo>();
            //monsterAppearNtfId.Structure.Buff = new List<byte>();
            //monsterAppearNtfId.Structure.ParentGuid = 0;
            //monsterAppearNtfId.Structure.LastChildId = 0;
            //client.SendCsProtoStructurePacket(entityAppearNtfIdList);

        }
    }
}