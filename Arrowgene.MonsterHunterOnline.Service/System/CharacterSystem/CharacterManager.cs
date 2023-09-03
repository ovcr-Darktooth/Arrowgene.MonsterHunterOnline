﻿using System.Collections.Generic;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.Database;
using Arrowgene.MonsterHunterOnline.Service.System.ItemSystem;
using Arrowgene.MonsterHunterOnline.Service.System.ItemSystem.Constant;
using Arrowgene.MonsterHunterOnline.Service.System.UnlockSystem;
using Arrowgene.MonsterHunterOnline.Service.Tdr.TlvStructures;

namespace Arrowgene.MonsterHunterOnline.Service.System.CharacterSystem;

public class CharacterManager
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(CharacterManager));

    private readonly IDatabase _database;

    public CharacterManager(IDatabase database)
    {
        _database = database;
    }

    public bool ModifyCharacter(Client client, ModifyFaceReq req)
    {
        Character character = _database.SelectCharacterByRoleIndex(client.Account.Id, (byte)req.RoleIndex);
        if (character == null)
        {
            return false;
        }

        character.AccountId = client.Account.Id;
        //req.Uin;
        //req.DbId;
        //req.ChangeGender;
        character.Gender = (byte)req.Gender;

        character.FaceId = req.FaceId;
        character.HairId = req.HairId;
        character.UnderclothesId = req.UnderclothesId;
        character.SkinColor = req.SkinColor;
        character.HairColor = req.HairColor;
        character.InnerColor = req.InnerColor;
        character.EyeBall = req.EyeBall;
        character.EyeColor = req.EyeColor;
        character.FaceTattooIndex = req.FaceTattooIndex;
        character.FaceTattooColor = req.FaceTattooColor;
        for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
        {
            character.FacialInfo[i] = req.FacialInfo[i];
        }

        return _database.UpdateCharacter(character);
    }

    public bool DeleteCharacter(Client client, byte roleIndex)
    {
        // TODO it looks like there is a grace period, but not sure about the mechanic, for now just delete it
        CsCsProtoStructurePacket<DeleteRoleRsp> rsp = CsProtoResponse.DeleteRoleRsp;
        rsp.Structure.RoleState = 0;
        // 2 = removing role failed -> login protection
        // 3 = the leader cant delete the guild
        // 4 = you are the leader, you can not transfer the role
        rsp.Structure.RoleStateEndLeftTime = 0;
        client.SendCsProtoStructurePacket(rsp);

        Character character = _database.SelectCharacterByRoleIndex(client.Account.Id, roleIndex);
        if (character == null)
        {
            return false;
        }

        if (!_database.DeleteCharacter(character.Id))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a new character with default values
    /// </summary>
    public Character CreateCharacter(Client client, RoleCreateInfo req)
    {
        Logger.Trace(client, "CreateCharacter");
        Character character = new Character();
        character.AccountId = client.Account.Id;
        character.Index = 0;
        character.Gender = req.Gender;
        character.Level = 1;
        character.Name = req.Name;
        character.RoleState = 0;
        character.RoleStateEndLeftTime = 0;
        character.AvatarSetId = 0;
        character.FaceId = req.FaceId;
        character.HairId = req.HairId;
        character.UnderclothesId = req.UnderclothesId;
        character.SkinColor = req.SkinColor;
        character.HairColor = req.HairColor;
        character.InnerColor = req.InnerColor;
        character.EyeBall = req.EyeBall;
        character.EyeColor = req.EyeColor;
        character.FaceTattooIndex = req.FaceTattooIndex;
        character.FaceTattooColor = req.FaceTattooColor;
        for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
        {
            character.FacialInfo[i] = req.FacialInfo[i];
        }

        character.StarLevel = "";
        character.HrLevel = 0;
        character.SoulStoneLv = 0;
        character.HideHelm = false;
        character.HideFashion = false;
        character.HideSuite = false;

        if (!_database.CreateCharacter(character))
        {
            return null;
        }

        return character;
    }

    /// <summary>
    /// Adds all roles for given client to `RoleList`
    /// </summary>
    public void PopulateRoleList(Client client, ListRoleRsp rsp)
    {
        Logger.Trace(client, "PopulateRoleList");
        List<Character> characters = _database.SelectCharactersByAccountId(client.Account.Id);
        foreach (Character character in characters)
        {
            List<Item> items = _database.SelectItemsByCharacterIdAndColumn(
                character.Id,
                (byte)ItemColumnType.Equipment
            );

            RoleBaseInfo role = new RoleBaseInfo();
            foreach (Item item in items)
            {
                if (item.PosGrid == null)
                {
                    continue;
                }

                AvatarItem avatarItem = new AvatarItem();
                avatarItem.ItemType = (int)item.ItemId;
                avatarItem.PosIndex = item.PosGrid.Value;
                role.Equip.Add(avatarItem);
            }

            role.RoleId = character.Id;
            role.RoleIndex = character.Index;
            role.Name = character.Name;
            role.Gender = character.Gender;
            role.Level = (int)character.Level;
            role.RoleState = character.RoleState;
            role.RoleStateEndLeftTime = character.RoleStateEndLeftTime;
            role.AvatarSetId = character.AvatarSetId;
            role.FaceId = character.FaceId;
            role.HairId = character.HairId;
            role.UnderclothesId = character.UnderclothesId;
            role.SkinColor = character.SkinColor;
            role.HairColor = character.HairColor;
            role.InnerColor = character.InnerColor;
            role.EyeBall = character.EyeBall;
            role.EyeColor = character.EyeColor;
            role.FaceTattooIndex = character.FaceTattooIndex;
            role.FaceTattooColor = character.FaceTattooColor;
            // TODO reference table
            // role.Equip = character.Equip; 
            role.HideHelm = character.HideHelm;
            role.HideFashion = character.HideFashion;
            role.HideSuite = character.HideSuite;
            for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
            {
                role.FacialInfo[i] = character.FacialInfo[i];
            }

            role.StarLevel = character.StarLevel;
            role.HrLevel = character.HrLevel;
            role.SoulStoneLv = character.SoulStoneLv;
            rsp.RoleList.Add(role);
        }
    }

    public Character GetCharacter(uint accountId, byte roleIndex)
    {
        return _database.SelectCharacterByRoleIndex(accountId, roleIndex);
    }

    public void PopulatePlayerInitInfo(Client client, Character character, PlayerInitInfo structure)
    {
        structure.AccountId = client.Account.Id;
        // TODO if this is entityId then need to be unique across NPC/Monster/Char
        structure.NetId = (int)character.Id;
        structure.DbId = character.Id;
        // for now set all to 1
        structure.SessionId = 1;
        structure.WorldId = 1;
        structure.ServerId = 1;
        structure.WorldSvrId = 1;
        //
        structure.ServerTime = 0;
        structure.IsReConnect = 0;
        structure.Name = character.Name;
        structure.Gender = character.Gender;
        structure.IsGm = 0;
        //spawn location
        structure.Pose.t.x = 409.91379f;
        structure.Pose.t.y = 358.74976f;
        structure.Pose.t.z = 100.0f; // height

        // TODO hack
        structure.Pose.t = client.State.InitSpawnPos;

        structure.Pose.q.v.x = 10;
        structure.Pose.q.v.y = 10;
        structure.Pose.q.v.z = 10;
        structure.Pose.q.w = 10;
        //
        structure.ParentEntityGuid = 0;
        structure.AvatarSetId = 0;
        structure.Faction = 0;
        structure.RandSeed = 0;
        structure.Weapon = 0;
        structure.LastLoginTime = 0;
        structure.CreateTime = (uint)Util.GetUnixTime(character.Created);
        structure.StoreSize = 0;
        structure.NormalSize = 0;
        structure.MaterialStoreSize = 0;
        structure.FirstEnterLevel = 0;
        structure.FirstEnterMap = 0;
        structure.PvpPrepareStageState = 0;
        structure.CurPlayerUsedCatCarCount = 0;
        structure.CatCuisineId = 0;
        structure.CatCuisineLevel = 0;
        structure.CatCuisineBuffs = 0;
        structure.CatCuisineLastTm = 0;
        structure.CatCuisineFormulaCount = 0;
        structure.IsSpectating = 0;
        structure.DragonShopBox = 0;
        structure.CanGetRewarded = 0;
        for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
        {
            structure.FacialInfo[i] = character.FacialInfo[i];
        }

        // attributes
        structure.Attr.SetCharLevel((int)character.Level);
        structure.Attr.CharSex = character.Gender;
        structure.Attr.SetCharSpeed(100);
        structure.Attr.CharSta = 100;
        structure.Attr.SetCharMaxSta(100);
        structure.Attr.StarLevel = character.HrLevel; // character.StarLevel is string;
        structure.Attr.CharHP = 100;
        structure.Attr.SetCharMaxHP(100);
        structure.Attr.MaleFace = character.FaceId;
        structure.Attr.MaleHair = character.HairId;
        structure.Attr.UnderClothes = character.UnderclothesId;
        structure.Attr.SkinColor = character.SkinColor;
        structure.Attr.HairColor = character.HairColor;
        structure.Attr.InnerColor = character.InnerColor;
        structure.Attr.FaceTattooId = character.FaceTattooIndex;
        structure.Attr.EyeBall = character.EyeBall;
        structure.Attr.FaceTattooColor = character.FaceTattooColor;
        structure.Attr.EyeColor = character.EyeColor;
        structure.Attr.SetFacialInfo(character.FacialInfo);
        structure.Attr.CharHRLevel = character.HrLevel;
        structure.Attr.CharHRPoint = 0; //TODO: check if it's hr exp
        structure.Attr.HideFashion = character.HideFashion;
        structure.Attr.HideSuite = character.HideSuite;
        structure.Attr.HideHelm = character.HideHelm;

        SystemUnlockFlags systemUnlockData = SystemUnlock.GetForLevel(character.Level);
        structure.Attr.SystemUnlockData = systemUnlockData;
        structure.Attr.SystemUnlockExtData1 = systemUnlockData.ToExtFlags();

        // task
        structure.Task.Tasks.Add(new TlvTaskEntry()
        {
            Id = 3001,
            AcceptTime = 0,
            State = 0,
            Timeout = 0
        });
        structure.Task.Tasks.Add(new TlvTaskEntry()
        {
            Id = 1002,
            AcceptTime = 0,
            State = 0,
            Timeout = 0
        });

        // equip
        // TODO null check
        client.Inventory.PopulateItemListProperties(structure);
    }

    public void SyncAllAttr(Client client)
    {
        List<AttrSync> attrs = GetAllAttrSync(client, client.Character);
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
    }

    public List<AttrSync> GetAllAttrSync(Client client, Character character)
    {
        List<AttrSync> attrs = new List<AttrSync>();

        AttrSync sync;

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 1;
        sync.BonusId = 1;
        sync.Data.Int = (int)character.Level;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 2;
        sync.BonusId = 0;
        sync.Data.String = character.Name;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 3;
        sync.BonusId = 0;
        sync.Data.Int = character.Gender;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 16;
        sync.BonusId = 1;
        sync.Data.Int = 100; // max hp
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 20;
        sync.BonusId = 0;
        sync.Data.Int = 100; // stamina (needed to move around)
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 21;
        sync.BonusId = 0;
        sync.Data.Int = 100; // max stamina (not needed to move around)
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 25;
        sync.BonusId = 1;
        sync.Data.Int = 1; // str
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 26;
        sync.BonusId = 1;
        sync.Data.Int = 1; // bst
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 27;
        sync.BonusId = 1;
        sync.Data.Int = 1; // lck
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 28;
        sync.BonusId = 1;
        sync.Data.Int = 1; // vir
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 73;
        sync.BonusId = 1;
        sync.Data.Int = 100; // speed
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 107;
        sync.BonusId = 0;
        sync.Data.Int = character.FaceId;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 108;
        sync.BonusId = 0;
        sync.Data.Int = character.HairId;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 172;
        sync.BonusId = 0;
        sync.Data.Int = character.UnderclothesId;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 200;
        sync.BonusId = 0;
        sync.Data.Int = character.SkinColor;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 201;
        sync.BonusId = 0;
        sync.Data.Int = character.HairColor;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 202;
        sync.BonusId = 0;
        sync.Data.Int = character.InnerColor;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 203;
        sync.BonusId = 0;
        sync.Data.Int = character.FaceTattooIndex;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 204;
        sync.BonusId = 0;
        sync.Data.Int = character.EyeBall;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 215;
        sync.BonusId = 0;
        sync.Data.Int = character.FaceTattooColor;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 216;
        sync.BonusId = 0;
        sync.Data.Int = character.EyeColor;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 222;
        sync.BonusId = 0;
        sync.Data.Bool = character.HideFashion;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 223;
        sync.BonusId = 0;
        sync.Data.Bool = character.HideSuite;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 224;
        sync.BonusId = 0;
        sync.Data.Bool = character.HideHelm;
        attrs.Add(sync);

        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 317;
        sync.BonusId = 0;
        sync.Data.Int = character.HrLevel;
        attrs.Add(sync);

        uint faceAttrId = 247;
        // TODO verify this logic
        for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
        {
            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = faceAttrId;
            sync.BonusId = 0;
            sync.Data.Int = character.FacialInfo[i];
            attrs.Add(sync);
            faceAttrId++;
            if (faceAttrId == 272)
            {
                faceAttrId = 324;
            }
        }

        //System unlock data
        sync = new AttrSync();
        sync.EntityId = character.Id;
        sync.AttrId = 236;
        sync.BonusId = 0;
        sync.Data.UInt64 = SystemUnlock.GetForLevel(character.Level).ToUInt64();
        //sync.Data.UInt64 = 8796093022207; //Debug value, everything unlocked
        //Logger.Info($"System unlock = {sync.Data.UInt64}\n binary : {Convert.ToString((long)sync.Data.UInt64, 2)}");
        attrs.Add(sync);


        return attrs;
    }
}