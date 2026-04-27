using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Core
{
    /// <summary>
    /// Maps CsProtoStructures to CsCmdId, names are based on `csproto.xml`
    /// </summary>
    public static class CsProtoResponse
    {
        public static CsCsProtoStructurePacket<ListRoleRsp> ListRoleRsp =>
            new(CS_CMD_ID.CS_CMD_LIST_ROLE_RSP);

        public static CsCsProtoStructurePacket<ListRoleRsp> CreateRoleRsp =>
            new(CS_CMD_ID.CS_CMD_LIST_ROLE_RSP);

        public static CsCsProtoStructurePacket<MultiIspSequenceNtf> MultiIspSequenceNtf =>
            new(CS_CMD_ID.CS_CMD_MULTI_ISP_SEQUENCE_NTF);

        public static CsCsProtoStructurePacket<RoleDataErrorRsp> RoleDataErrorRsp =>
            new(CS_CMD_ID.CS_CMD_ROLEDATA_ERR_RSP);

        public static CsCsProtoStructurePacket<NotifyInfo> NotifyInfo =>
            new(CS_CMD_ID.CS_CMD_SYSTEM_NOTIFY_INFO);

        public static CsCsProtoStructurePacket<DeleteRoleRsp> DeleteRoleRsp =>
            new(CS_CMD_ID.CS_CMD_DELETE_ROLE_RSP);

        public static CsCsProtoStructurePacket<AccountRsp> AccountRsp =>
            new(CS_CMD_ID.SC_CMD_WORLD_ACCOUNT_RSP);

        public static CsCsProtoStructurePacket<ModifyFaceRsp> ModifyFaceRsp =>
            new(CS_CMD_ID.CS_CMD_MODIFY_FACE_RSP);

        public static CsCsProtoStructurePacket<SelectRoleRsp> SelectRoleRsp =>
            new(CS_CMD_ID.CS_CMD_SELECT_ROLE_RSP);

        public static CsCsProtoStructurePacket<TownSessionStart> TownSessionStart =>
            new(CS_CMD_ID.CS_CMD_TOWN_SESSION_START);

        public static CsCsProtoStructurePacket<PlayerInitInfo> PlayerInitInfo =>
            new(CS_CMD_ID.CS_CMD_PLAYER_INIT_NTF);

        public static CsCsProtoStructurePacket DataLoadRsp(RemoteDataLoadRsp structure) =>
            new(CS_CMD_ID.CS_CMD_DATA_LOAD_RSP, structure);

        public static CsCsProtoStructurePacket<TownInstanceVerifyRsp> TownServerInitNtf =>
            new(CS_CMD_ID.CS_CMD_TOWN_SERVER_INIT_NTF);

        public static CsCsProtoStructurePacket<AttrSyncList> AttrSyncList =>
            new(CS_CMD_ID.CS_CMD_ATTR_SYNC_LIST_NTF);

        public static CsCsProtoStructurePacket<AttrSync> AttrSync =>
            new(CS_CMD_ID.CS_CMD_ATTR_SYNC_NTF);

        public static CsCsProtoStructurePacket<ReselectRoleRsp> ReselectRoleRsp =>
            new(CS_CMD_ID.CS_CMD_RESELECT_ROLE_RSP);

        public static CsCsProtoStructurePacket<ActorMoveStateNtf> ActorMoveStateNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_MOVESTATE_NTF);

        public static CsCsProtoStructurePacket<ActorIdleMoveNtf> ActorIdleMoveNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_IDLEMOVE_NTF);

        public static CsCsProtoStructurePacket<ActorBeginMoveNtf> ActorBeginMoveNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_BEGINMOVE_NTF);

        public static CsCsProtoStructurePacket<ActorStopMoveNtf> ActorStopMoveNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_STOPMOVE);

        public static CsCsProtoStructurePacket<UpdateRushState> UpdateRushState =>
            new(CS_CMD_ID.CS_CMD_UPDATE_RUSHSTATE);

        /// <summary>
        /// C2表现FIFO同步消息
        /// C2 represents FIFO synchronization messages
        /// </summary>
        public static CsCsProtoStructurePacket<FifoSyncInfoNtf> FifoSyncInfoNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_FIFO_SYNC_NTF);

        /// <summary>
        /// 服务器强制状态变化消息
        /// Server force state change message
        /// </summary>
        public static CsCsProtoStructurePacket<ServerSyncInfoNtf> ServerSyncInfoNtf =>
            new(CS_CMD_ID.CS_CMD_SERVER_ACTOR_FIFO_SYNC_NTF);

        public static CsCsProtoStructurePacket<PlayerRegionJumpRsp> PlayerRegionJumpRsp =>
            new(CS_CMD_ID.CS_CMD_PLAYER_REGION_JUMP_RSP);

        public static CsCsProtoStructurePacket<ChangeTownInstanceRsp> ChangeTownInstanceRsp =>
            new(CS_CMD_ID.CS_CMD_CHANGE_TOWN_INSTANCE_RSP);

        public static CsCsProtoStructurePacket<PlayerTeleport> PlayerTeleport =>
            new(CS_CMD_ID.CS_CMD_PLAYER_TELEPORT_NTF);

        //CS_CMD_SERVER_ACTOR_FIFO_SYNC_NTF - registered by client
        //CS_CMD_BATTLE_ACTOR_FIFO_SYNC_NTF - registered by client

        // ServerSyncInfoAck type="CSServerSyncInfoAck" id="CS_CMD_SERVER_ACTOR_FIFO_SYNC_ACK" desc="服务器强制状态变化的确认" Acknowledgment of Server Forced State Change
        // ServerSyncMsgNtf  type="CSServerSyncInfoNtf" id="CS_CMD_SERVER_ACTOR_FIFO_SYNC_NTF" desc="服务器强制状态变化消息"/> Server force state change message
        // FIFOSyncMsgNtf    type="CSFIFOSyncInfoNtf" id="CS_CMD_BATTLE_ACTOR_FIFO_SYNC_NTF" desc="C2表现FIFO同步消息"/> C2 represents FIFO synchronization messages
        // FIFOSyncMsg       type="CSFIFOSyncInfo" id="CS_CMD_BATTLE_ACTOR_FIFO_SYNC" desc="FIFO同步消息"/> FIFO synchronization message

        public static CsCsProtoStructurePacket<ChatNtf> ChatNtf => new(CS_CMD_ID.CS_CMD_CHAT_NTF);
        public static CsCsProtoStructurePacket<SpawnSrvEnt> SpawnSrvEnt => new(CS_CMD_ID.CS_CMD_SPAWN_SRVENT);

        public static CsCsProtoStructurePacket<SpawnSrvEntList> SpawnSrvEntList =>
            new(CS_CMD_ID.CS_CMD_SPAWN_SRVENTLIST);

        /// <summary>
        /// Looks like client ignores this, send CS_CMD_MONSTER_APPEAR_NTF_LIST instead
        /// </summary>
        public static CsCsProtoStructurePacket<MonsterAppearNtf> MonsterAppearNtf =>
            new(CS_CMD_ID.CS_CMD_MONSTER_APPEAR_NTF);

        public static CsCsProtoStructurePacket<MonsterAppearNtfList> MonsterAppearNtfList =>
            new(CS_CMD_ID.CS_CMD_MONSTER_APPEAR_NTF_LIST);

        public static CsCsProtoStructurePacket<EntityAppearNtfIdList> EntityAppearNtfIdList =>
            new(CS_CMD_ID.CS_CMD_ENTITY_APPEAR_NTF_ID_LIST);

        public static CsCsProtoStructurePacket<MonsterActiveState> MonsterActiveState =>
            new(CS_CMD_ID.CS_CMD_MONSTER_ACTIVE);

        public static CsCsProtoStructurePacket<MonsterSequenceState> MonsterSequenceState =>
            new(CS_CMD_ID.CS_CMD_BATTLE_MONSTER_SEQUENCESTATE);

        public static CsCsProtoStructurePacket<CtrledMonsterAppearNtf> CtrledMonsterAppearNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_CTRLED_MONSTER_APPEAR_NTF); 

        /// <summary>
        /// Looks like client ignores this, send CS_CMD_SCENEOBJ_APPEAR_NTF_LIST instead
        /// </summary>
        public static CsCsProtoStructurePacket<SceneObjAppearNtf> SceneObjAppearNtf =>
            new(CS_CMD_ID.CS_CMD_SCENEOBJ_APPEAR_NTF);

        public static CsCsProtoStructurePacket<SceneObjAppearNtfList> SceneObjAppearNtfList =>
            new(CS_CMD_ID.CS_CMD_SCENEOBJ_APPEAR_NTF_LIST);

        public static CsCsProtoStructurePacket<MainInstanceOptSynRsp> MainInstanceOptSynRsp =>
            new(CS_CMD_ID.CS_CMD_MAIN_INSTANCE_OPT_SYN_RSP);

        public static CsCsProtoStructurePacket<MainInstanceEnterOptRsp> MainInstanceEnterOptRsp =>
            new(CS_CMD_ID.CS_CMD_MAIN_INSTANCE_ENTER_OPT_RSP);

        public static CsCsProtoStructurePacket<EnterInstanceCountDown> EnterInstanceCountDown =>
            new(CS_CMD_ID.CS_CMD_ENTER_INSTANCE_COUNT_DOWN);

        public static CsCsProtoStructurePacket<ItemMgrMoveItemNtf> ItemMgrMoveItemNtf =>
            new(CS_CMD_ID.CS_CMD_ITEMMGR_MOVE_ITEM_NTF);

        public static CsCsProtoStructurePacket<ItemMgrAddItemNtf> ItemMgrAddItemNtf =>
            new(CS_CMD_ID.CS_CMD_ITEMMGR_ADD_ITEM_NTF);

        public static CsCsProtoStructurePacket<ItemMgrSwapItemNtf> ItemMgrSwapItemNtf =>
            new(CS_CMD_ID.CS_CMD_ITEMMGR_SWAP_ITEM_NTF);

        public static CsCsProtoStructurePacket<ItemMgrUseItemReq> ItemMgrUseItemReq =>
            new(CS_CMD_ID.CS_CMD_ITEMMGR_USE_ITEM_REQ);

        public static CsCsProtoStructurePacket<MainInstanceAgreeOptRsp> MainInstanceAgreeOptRsp =>
            new(CS_CMD_ID.CS_CMD_MAIN_INSTANCE_AGREE_OPT_RSP);

        public static CsCsProtoStructurePacket<InstanceInitInfo> InstanceInitInfo =>
            new(CS_CMD_ID.CS_CMD_INSTANCE_INIT_NTF);

        public static CsCsProtoStructurePacket<EnterInstanceRsp> EnterInstanceRsp =>
            new(CS_CMD_ID.CS_CMD_ENTER_INSTANCE_RSP);

        public static CsCsProtoStructurePacket<LeaveInstanceRsp> LeaveInstanceRsp =>
            new(CS_CMD_ID.CS_CMD_LEAVE_INSTANCE_RSP);

        public static CsCsProtoStructurePacket<InstanceVerifyRsp> InstanceVerifyRsp =>
            new(CS_CMD_ID.CS_CMD_INSTANCE_VERIFY_RSP);

        public static CsCsProtoStructurePacket<PlayerLevelInitInfo> PlayerLevelInitNtf =>
            new(CS_CMD_ID.CS_CMD_PLAYER_LEVEL_INIT_NTF);

        public static CsCsProtoStructurePacket<MainInstanceClose> MainInstanceClose =>
            new(CS_CMD_ID.CS_CMD_MAIN_INSTANCE_CLOSE);

        public static CsCsProtoStructurePacket<SkillEffectInfo> SkillEffectInfo =>
            new(CS_CMD_ID.CS_CMD_SKILL_EFFECT_SYNC);

        public static CsCsProtoStructurePacket<HealthSyncNtf> HealthSyncNtf =>
            new(CS_CMD_ID.CS_CMD_HEALTH_SYNC);

        public static CsCsProtoStructurePacket<ManufactureProduceRsp> ManufactureProduceRsp =>
            new(CS_CMD_ID.CS_CMD_MANUFACTURE_PRODUCE_RSP);

        public static CsCsProtoStructurePacket<ManufactureProduceReq> ManufactureProduceReq =>
            new(CS_CMD_ID.CS_CMD_MANUFACTURE_PRODUCE_REQ);

        public static CsCsProtoStructurePacket<ChangeAmmoRsp> ChangeAmmoRsp =>
            new(CS_CMD_ID.CS_CMD_CHANGE_AMMO_RSP);

        public static CsCsProtoStructurePacket<ChangeAmmoReq> ChangeAmmoReq =>
            new(CS_CMD_ID.CS_CMD_CHANGE_AMMO_REQ);

        public static CsCsProtoStructurePacket<PlayerAmmoChangeReq> PlayerAmmoChangeReq =>
            new(CS_CMD_ID.CS_CMD_PLAYER_AMMO_CHANGE_REQ);

        public static CsCsProtoStructurePacket<PlayerAmmoChangeRsp> PlayerAmmoChangeRsp =>
            new(CS_CMD_ID.CS_CMD_PLAYER_AMMO_CHANGE_RSP);

        public static CsCsProtoStructurePacket<ReloadAmmoReq> ReloadAmmoReq =>
            new(CS_CMD_ID.CS_CMD_RELOAD_AMMO_REQ);

        public static CsCsProtoStructurePacket<ReloadAmmoRsp> ReloadAmmoRsp =>
            new(CS_CMD_ID.CS_CMD_RELOAD_AMMO_RSP);

        public static CsCsProtoStructurePacket<ProjectileLaunchNtfList> ProjectileLaunchNtfList =>
            new(CS_CMD_ID.CS_CMD_PROJECTILE_LAUNCH_NTF_LIST);

        public static CsCsProtoStructurePacket<ProjectileLaunchNtf> ProjectileLaunchNtf =>
            new(CS_CMD_ID.CS_CMD_PROJECTILE_LAUNCH_NTF);

        public static CsCsProtoStructurePacket<BattleDMG> BattleDMG =>
            new(CS_CMD_ID.CS_CMD_BATTLE_DMG_VERIFY);

        public static CsCsProtoStructurePacket<DMGResult> DMGResult =>
            new(CS_CMD_ID.CS_CMD_BATTLE_DMGRESULT);

        public static CsCsProtoStructurePacket<BattlePVPDMG> BattlePVPDMG =>
            new(CS_CMD_ID.CS_CMD_BATTLE_PVP_DMG);

        public static CsCsProtoStructurePacket<BattlePVPDMGNtf> BattlePVPDMGNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_PVP_DMG_NTF);

        public static CsCsProtoStructurePacket<CSGuideBookAutoFirstOpenReq> CSGuideBookAutoFirstOpenReq =>
            new(CS_CMD_ID.C2S_CMD_GUIDE_BOOK_AUTO_FIRST_OPEN_REQ);

        public static CsCsProtoStructurePacket<SCGuideBookAutoFirstOpenRsp> SCGuideBookAutoFirstOpenRsp =>
            new(CS_CMD_ID.S2C_CMD_GUIDE_BOOK_AUTO_FIRST_OPEN_RSP);

        public static CsCsProtoStructurePacket<GetGuilds> GetGuilds =>
            new(CS_CMD_ID.S2C_CMD_GUILD_GETGUILDS);

        public static CsCsProtoStructurePacket<GetGuilders> GetGuilders =>
            new(CS_CMD_ID.S2C_CMD_GUILD_GETGUILDERS);

        public static CsCsProtoStructurePacket<GetGuildDetail> GetGuildDetail =>
            new(CS_CMD_ID.S2C_CMD_GUILD_GETGUILDDETAIL);

        public static CsCsProtoStructurePacket<CSInstanceResultRsp> InstanceResultRsp =>
            new(CS_CMD_ID.CS_CMD_BATTLE_RESULT_RSP);



    }
}