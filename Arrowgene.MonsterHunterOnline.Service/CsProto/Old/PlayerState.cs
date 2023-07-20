using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.System;
using Arrowgene.MonsterHunterOnline.Service.System.Chat;
using Arrowgene.MonsterHunterOnline.Service.Tdr;

namespace Arrowgene.MonsterHunterOnline.Service;

/// <summary>
///  this is a temporary holder for central management of packet and information,
/// it can be considered the "playground" for now.
///
/// On*- function are lifecycle hooks
/// Send*- functions are to send specific data that has been consistently populated
/// </summary>
public class PlayerState
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(PlayerState));

    private Client _client;
    public static Server Server;
    public int levelId { get; set; }
    public int prevLevelId { get; set; }


    public PlayerState(Client client)
    {
        _client = client;
        

        StreamBuffer ast = new StreamBuffer();
        ast.WriteByte((byte)TdrTlvMagic.NoVariant);

        int sizePos = ast.Position;
        ast.WriteInt32(0, Endianness.Big); // size

        // case 0
        uint tag = TdrTlv.MakeTag(2, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(5, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(1, Endianness.Big);
        }

        // case 1
        // skips X bytes
        tag = TdrTlv.MakeTag(3, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(11, Endianness.Big);

        // case 2
        // read int32
        tag = TdrTlv.MakeTag(4, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(12, Endianness.Big);


        // case 3 / skip bytes??
        // read int32
        tag = TdrTlv.MakeTag(5, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(13, Endianness.Big);


        // case 4
        // read int32
        tag = TdrTlv.MakeTag(6, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(14, Endianness.Big);


        // case 5
        // read int32
        tag = TdrTlv.MakeTag(7, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(15, Endianness.Big);


        // case 6
        // read int32
        tag = TdrTlv.MakeTag(8, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(16, Endianness.Big);


        // case 7
        // read int32
        tag = TdrTlv.MakeTag(9, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(17, Endianness.Big);


        // case 8
        // read int32
        tag = TdrTlv.MakeTag(10, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(18, Endianness.Big);


        // case 9
        // read int32
        tag = TdrTlv.MakeTag(11, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(19, Endianness.Big);

        // case 10
        // read int32
        tag = TdrTlv.MakeTag(12, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(20, Endianness.Big);

        // case 11
        // read int32
        tag = TdrTlv.MakeTag(13, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(21, Endianness.Big);

        // case 12
        // read int32
        tag = TdrTlv.MakeTag(14, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(22, Endianness.Big);

        // case 13
        // read int32
        tag = TdrTlv.MakeTag(15, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(23, Endianness.Big);

        // case 14
        // read int32
        tag = TdrTlv.MakeTag(16, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(24, Endianness.Big);

        // case 15
        // read int32
        tag = TdrTlv.MakeTag(17, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(25, Endianness.Big);
        }

        // case 16 - 0x10
        // read int32
        tag = TdrTlv.MakeTag(18, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(26, Endianness.Big);
        }

        // 17 - readx7 1878525
        tag = TdrTlv.MakeTag(19, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(27, Endianness.Big);
        }

        //18 - readint32 1878660
        tag = TdrTlv.MakeTag(20, TdrTlvType.ID_2_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt16(28, Endianness.Big);

//19 - readint32 1878706
        tag = TdrTlv.MakeTag(21, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(29, Endianness.Big);

// 20 - readx7 1878752
        tag = TdrTlv.MakeTag(22, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(30, Endianness.Big);
        }

// 21 - readx7 1878893
        tag = TdrTlv.MakeTag(23, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(31, Endianness.Big);
        }

        //22 - readx7 1879037
        tag = TdrTlv.MakeTag(24, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(32, Endianness.Big);
        }

        //23 - readx7 1879181
        tag = TdrTlv.MakeTag(25, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(33, Endianness.Big);
        }

//24 - skip 1900773
        tag = TdrTlv.MakeTag(26, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(34, Endianness.Big);


        tag = TdrTlv.MakeTag(27, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(34, Endianness.Big);


        tag = TdrTlv.MakeTag(28, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(34, Endianness.Big);

        tag = TdrTlv.MakeTag(29, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(34, Endianness.Big);


        tag = TdrTlv.MakeTag(30, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(34, Endianness.Big);

// 29
        tag = TdrTlv.MakeTag(31, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(34, Endianness.Big);


        // 30 - readx7 1879325

        tag = TdrTlv.MakeTag(32, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(10, Endianness.Big);
        }

        tag = TdrTlv.MakeTag(33, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(10, Endianness.Big);
        }

        tag = TdrTlv.MakeTag(34, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(10, Endianness.Big);
        }

        tag = TdrTlv.MakeTag(35, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
        for (int i = 0; i < 7; i++)
        {
            ast.WriteInt32(10, Endianness.Big);
        }

        //34 - readint32 1879901
        tag = TdrTlv.MakeTag(36, TdrTlvType.ID_4_BYTE);
        TdrBuffer.WriteVarUInt32(ast, tag);
        ast.WriteInt32(35, Endianness.Big);


        for (int i = 37; i < 200; i++)
        {
            tag = TdrTlv.MakeTag(i, TdrTlvType.ID_4_BYTE);
            TdrBuffer.WriteVarUInt32(ast, tag);
            ast.WriteInt32(7 * 4, Endianness.Big); // size, max 7*4
            for (int j = 0; j < 7; j++)
            {
                ast.WriteInt32(2, Endianness.Big);
            }
        }


        int size = ast.Position - sizePos;
        ast.Position = sizePos;
        ast.WriteInt32(size, Endianness.Big); // size
    }

    public void OnChatMsg(ChatMessage chatMessage)
    {
        if (chatMessage.Message == "test")
        {
            CSSpawnPlayer _spawnPlayer = new CSSpawnPlayer();
            _spawnPlayer.PlayerId = 1;
            _spawnPlayer.NetObjId = 1;
            _spawnPlayer.Name = "Name";
            _spawnPlayer.Gender = 1;
            _spawnPlayer.Scale = 1.0f;
            _spawnPlayer.NewConnect = 1;
            _spawnPlayer.SendSrvId = 1;
            _spawnPlayer.AvatarSetID = 1;
            _spawnPlayer.Position = new XYZPosition();

            _client.SendCsPacket(NewCsPacket.SpawnPlayer(_spawnPlayer));
        }
        else if (chatMessage.Message == "sync")
        {
            List<AttrSync> attrs = Server.CharacterManager.GetAllAttrSync(_client, _client.Character);
            List<List<AttrSync>> attrChunks = Util.Chunk(attrs, CsProtoConstant.CS_ATTR_SYNC_LIST_MAX);

            foreach (List<AttrSync> attrChunk in attrChunks)
            {
                if (attrChunk.Count > CsProtoConstant.CS_ATTR_SYNC_LIST_MAX)
                {
                    Logger.Error(_client, "Chunk error");
                }

                CsProtoStructurePacket<AttrSyncList> attrSyncList = CsProtoResponse.AttrSyncList;
                for (int i = 0; i < attrChunk.Count; i++)
                {
                    attrSyncList.Structure.Attr.Add(attrChunk[i]);
                }

                _client.SendCsProtoStructurePacket(attrSyncList);
            }
        }
        else if (chatMessage.Message == "town_init")
        {
            CsProtoStructurePacket<TownInstanceVerifyRsp> townServerInitNtf = CsProtoResponse.TownServerInitNtf;
            TownInstanceVerifyRsp verifyRsp = townServerInitNtf.Structure;
            verifyRsp.ErrNo = 0;
            verifyRsp.LineId = 0;
            verifyRsp.LevelEnterType = 0;

            InstanceInitInfo instanceInitInfo = verifyRsp.InstanceInitInfo;
            instanceInitInfo.BattleGroundId = 0;
            instanceInitInfo.LevelId = 150301;
            instanceInitInfo.CreateMaxPlayerCount = 4;
            instanceInitInfo.GameMode = GameMode.Town;
            instanceInitInfo.TimeType = TimeType.Noon;
            instanceInitInfo.WeatherType = WeatherType.Sunny;
            instanceInitInfo.Time = 1;
            instanceInitInfo.LevelRandSeed = 1;
            instanceInitInfo.WarningFlag = 0;
            instanceInitInfo.CreatePlayerMaxLv = 99;

            _client.SendCsProtoStructurePacket(townServerInitNtf);
            prevLevelId = levelId;
            levelId = instanceInitInfo.LevelId;
        }
        else if (chatMessage.Message == "money")
        {

            CsProtoStructurePacket<AttrSync> sync = CsProtoResponse.AttrSync;
            
            sync.Structure.EntityId = _client.Character.Id;
            sync.Structure.AttrId = 75;
            sync.Structure.BonusId = 0;
            sync.Structure.Data.Int = 999910; // gold
            _client.SendCsProtoStructurePacket(sync);

            sync.Structure.AttrId = 76;
            sync.Structure.BonusId = 0;
            sync.Structure.Data.Int = 999909; // silver
            _client.SendCsProtoStructurePacket(sync);


            /*sync.AttrId = 77;
            sync.BonusId = 0;
            sync.Data.Int = 999908;
            attrSyncList.Structure.Attr.Add(sync);
            sync.AttrId = 78;
            sync.BonusId = 0;
            sync.Data.Int = 999907;
            attrSyncList.Structure.Attr.Add(sync);*/

        }
        else if (chatMessage.Message == "tp")
        {

            CsProtoStructurePacket<PlayerTeleport> PlayerTeleport = CsProtoResponse.PlayerTeleport;
            PlayerTeleport.Structure.SyncTime = 1;
            PlayerTeleport.Structure.NetObjId = _client.Character.Id;
            PlayerTeleport.Structure.Region = 604001; // 180201 = meze
            PlayerTeleport.Structure.TargetPos = new CSQuatT()
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
                    x = 20f,
                    y = 18.6145f,
                    z = 30.703117f
                }
            };
            PlayerTeleport.Structure.ParentGuid = 1;
            PlayerTeleport.Structure.InitState = 1;
            _client.SendCsProtoStructurePacket(PlayerTeleport);
        }
        else if (chatMessage.Message == "mz")
        {
            CsProtoStructurePacket<CSMonsterSizeNtf> monsterSize = CsProtoResponse.MonsterSizeNtf; 

            monsterSize.Structure.Infos = new List<CSMonsterSizeInfo>();
            CSMonsterSizeInfo monsterSizeInfo = new CSMonsterSizeInfo();
            //monsterSizeInfo.MonsterID = 66;
            monsterSizeInfo.MinSize = 1000f;
            monsterSizeInfo.MaxSize = 2000f;
            monsterSizeInfo.SizeLevel = 1;

            //monsterSize.Structure.Infos.Add(monsterSizeInfo);

            //_client.SendCsProtoStructurePacket(monsterSize);

            for(int i = 1;i<105;i++)
            {
                if (i != 98) { 
                    monsterSizeInfo.MonsterID = i;

                    monsterSize.Structure.Infos.Add(monsterSizeInfo);
                }
            }
            _client.SendCsProtoStructurePacket(monsterSize);
        }
        else if (chatMessage.Message == "bag")
        {
            CharacterManager characterManager = new CharacterManager(Server.Database);
            CsProtoStructurePacket<PlayerInitInfo> playerInitInfo = CsProtoResponse.PlayerInitInfo;
            characterManager.PopulatePlayerInitInfo(_client, _client.Character, playerInitInfo.Structure);
            _client.SendCsProtoStructurePacket(playerInitInfo);

        }
        else if (chatMessage.Message == "hb")
        {
            CsProtoStructurePacket<CSSelectHuntingBagRsp>  huntingbagRsp = CsProtoResponse.SelectHuntingBagRsp;

            huntingbagRsp.Structure.ErrCode = 0;
            huntingbagRsp.Structure.NetId = (int)_client.Character.Id;
            huntingbagRsp.Structure.Name = "大剑武器训练包";
            huntingbagRsp.Structure.HuntingBagID = 903;

            _client.SendCsProtoStructurePacket(huntingbagRsp);

            //Errcode :
            // 0, 1 ,3 to 29 = nothing ?
            // 2 = already choosen by other player
            // 
        }

    }

    public void SendBruteForceT()
    {
        Thread bruteForce = new Thread(SendBruteForce);
        bruteForce.Start();
    }

    public void SendBruteForce()
    {
        Thread.Sleep(3000);

        List<string> exclude = new List<string>()
        {
            "CatTreatureErr",
            "CatTreatureOpen",
            "XHunterResultNtf",
            "CommanderChgNtf",
            "BattlePunishNtf",
            "ExcellentPointNtf",
            "SensitiveVerify",
            "SensitiveVerifyResult",
            "CatCuisineUnlockNtf",
            "C2STalkExec",
            "C2STalkEnd",
            "S2CTalkErr",
            "S2CTalkExec",
            "EquipPlanUnlockNtf",
            "EquipPlanEditNtf",
            "LineUpStateNtf",
            "GuildMatchSignUpReadyNtf",
            "GuildMatchSignUpListNtf",
            "GuildMatchSignUpAdd",
            "GuildMatchSignUpDel",
            "ScheduleError",
            "ObtainTargetListRes",
            "GuildMatchQualifierFirstNtf",
            "GuildMatchStateNtf",
            "GuildMatchPairListNtf",
            "PkgEncryptData",
            "Ping",
            "HeartBeat",
            "GameManagerCmd",
            "GlobalErrNtf",
            "PkgTimerRecord",
            "PkgTransAntiData",
            "PingReply",
            "PkgChatEncryptData",
            "ZipPkg",
            "DelBuff",
            "BuffParamChange",
            "NotifyInfo",
            "DropClientNotifyInfo",
            "PropSync",
            "TimeOfDayNtf",
            "AttrSync",
            "AttrInfo",
            "FarmSetEquipAvatarNtf",
            "FarmWoodIndexIDMappingNtf",
            "CatTreatureInfo",
            "C2SSpeakWord",
            "C2SSpeakSetSelfDef",
            "C2SSpeakSetAuto",
            "S2CSculptureLibSnapshot",
            "S2CGetSculptureLibs",
            "S2CSculptureErr",
            "S2CSpeakErr",
            "SurrenderVoteNtf",
            "C2SGetSculptureLibs",
            "S2CGetSculpture",
            "S2CSculptureAvatarSnapshot",
            "S2CScriptAddSculpture",
            "S2CSpoorFetchPrize",
            "S2CSpoorErr",
            "S2CSpoorAddPoints",
            "ItemMgrMoveSwapItemsNtf",
        };

        Type type = typeof(NewCsPacket);
        List<MethodInfo> collectedMethods = new List<MethodInfo>();
        foreach (MethodInfo mi in type.GetMethods())
        {
            if (exclude.Contains(mi.Name))
            {
                continue;
            }

            if (mi.Name == "Equals"
                || mi.Name == "GetType"
                || mi.Name == "ToString"
                || mi.Name == "GetHashCode")
            {
                continue;
            }

            if (mi.Name.EndsWith("Req"))
            {
                continue;
            }

            if (mi.Name.StartsWith("C2S"))
            {
                continue;
            }

            if (mi.Name.EndsWith("Rsp"))
            {
                continue;
            }

            collectedMethods.Add(mi);
        }

        int start = 0;
        int current = 0;
        int total = collectedMethods.Count;
        foreach (MethodInfo mi in collectedMethods)
        {
            if (current < start)
            {
                current++;
                continue;
            }

            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters.Length != 1)
            {
                Logger.Error($"parameters.Length != 1 ({mi})");
                continue;
            }

            object parameterInstance = CreateInstance(parameters[0].ParameterType);
            object ret = mi.Invoke(null, new object[] { parameterInstance });
            if (ret is CsPacket csPacket)
            {
                Logger.Error($"Sending {mi.Name} ({current}/{total})");
                try
                {
                    _client.SendCsPacket(csPacket);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }

                Thread.Sleep(1000);
                current++;
            }
            else
            {
                Logger.Error($"ret is NOT CsPacket csPacket({mi})");
            }
        }
    }

    private object CreateInstance(Type type)
    {
        if (type.IsInterface)
        {
            Type concrete = null;
            foreach (Type t in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (!t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t))
                {
                    concrete = t;
                    break;
                }
            }

            if (concrete == null)
            {
                Logger.Error($"Interface ({type}) can not be created, no implementation found");
                return null;
            }

            Logger.Info($"Interface ({type}) can not be created, using implementation {concrete}");
            type = concrete;
        }

        ConstructorInfo[] cis = type.GetConstructors();

        if (cis.Length <= 0)
        {
            return Activator.CreateInstance(type);
        }

        ParameterInfo[] pis = cis[0].GetParameters();
        if (pis.Length <= 0)
        {
            return Activator.CreateInstance(type);
        }

        object[] paramInstances = new object[pis.Length];
        for (int i = 0; i < pis.Length; i++)
        {
            paramInstances[i] = CreateInstance(pis[i].ParameterType);
        }

        return Activator.CreateInstance(type, paramInstances);
    }
}