﻿using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Core
{
    /// <summary>
    /// Maps CsProtoStructures to CsCmdId, names are based on `csproto.xml`
    /// </summary>
    public static class CsProtoResponse
    {
        public static CsProtoStructurePacket<ListRoleRsp> ListRoleRsp =>
            new(CS_CMD_ID.CS_CMD_LIST_ROLE_RSP);

        public static CsProtoStructurePacket<ListRoleRsp> CreateRoleRsp =>
            new(CS_CMD_ID.CS_CMD_LIST_ROLE_RSP);

        public static CsProtoStructurePacket<MultiIspSequenceNtf> MultiIspSequenceNtf =>
            new(CS_CMD_ID.CS_CMD_MULTI_ISP_SEQUENCE_NTF);

        public static CsProtoStructurePacket<RoleDataErrorRsp> RoleDataErrorRsp =>
            new(CS_CMD_ID.CS_CMD_ROLEDATA_ERR_RSP);

        public static CsProtoStructurePacket<NotifyInfo> NotifyInfo =>
            new(CS_CMD_ID.CS_CMD_SYSTEM_NOTIFY_INFO);

        public static CsProtoStructurePacket<DeleteRoleRsp> DeleteRoleRsp =>
            new(CS_CMD_ID.CS_CMD_DELETE_ROLE_RSP);

        public static CsProtoStructurePacket<AccountRsp> AccountRsp =>
            new(CS_CMD_ID.SC_CMD_WORLD_ACCOUNT_RSP);

        public static CsProtoStructurePacket<ModifyFaceRsp> ModifyFaceRsp =>
            new(CS_CMD_ID.CS_CMD_MODIFY_FACE_RSP);

        public static CsProtoStructurePacket<SelectRoleRsp> SelectRoleRsp =>
            new(CS_CMD_ID.CS_CMD_SELECT_ROLE_RSP);

        public static CsProtoStructurePacket<TownSessionStart> TownSessionStart =>
            new(CS_CMD_ID.CS_CMD_TOWN_SESSION_START);

        public static CsProtoStructurePacket<PlayerInitInfo> PlayerInitInfo =>
            new(CS_CMD_ID.CS_CMD_PLAYER_INIT_NTF);

        public static CsProtoStructurePacket DataLoadRsp(RemoteDataLoadRsp structure) =>
            new(CS_CMD_ID.CS_CMD_DATA_LOAD_RSP, structure);

        public static CsProtoStructurePacket<TownInstanceVerifyRsp> TownServerInitNtf =>
            new(CS_CMD_ID.CS_CMD_TOWN_SERVER_INIT_NTF);

        public static CsProtoStructurePacket<AttrSyncList> AttrSyncList =>
            new(CS_CMD_ID.CS_CMD_ATTR_SYNC_LIST_NTF);

        public static CsProtoStructurePacket<AttrSync> AttrSync =>
            new(CS_CMD_ID.CS_CMD_ATTR_SYNC_NTF);

        public static CsProtoStructurePacket<ReselectRoleRsp> ReselectRoleRsp =>
            new(CS_CMD_ID.CS_CMD_RESELECT_ROLE_RSP);

        public static CsProtoStructurePacket<ActorMoveStateNtf> ActorMoveStateNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_MOVESTATE_NTF);

        public static CsProtoStructurePacket<ActorIdleMoveNtf> ActorIdleMoveNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_IDLEMOVE_NTF);

        public static CsProtoStructurePacket<ActorBeginMoveNtf> ActorBeginMoveNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_BEGINMOVE_NTF);

        public static CsProtoStructurePacket<ActorStopMoveNtf> ActorStopMoveNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_STOPMOVE);

        public static CsProtoStructurePacket<UpdateRushState> UpdateRushState =>
            new(CS_CMD_ID.CS_CMD_UPDATE_RUSHSTATE);

        public static CsProtoStructurePacket<RefreshScheduleRsp> RefreshScheduleRsp =>
            new(CS_CMD_ID.S2C_CMD_TASK_REFRESHSCHEDULE);

        public static CsProtoStructurePacket<FetchPrizeRes> FetchPrizeRes =>
            new(CS_CMD_ID.CS_CMD_SCHEDULE_FETCH_PRIZE_RES);

        /// <summary>
        /// C2表现FIFO同步消息
        /// C2 represents FIFO synchronization messages
        /// </summary>
        public static CsProtoStructurePacket<FifoSyncInfoNtf> FifoSyncInfoNtf =>
            new(CS_CMD_ID.CS_CMD_BATTLE_ACTOR_FIFO_SYNC_NTF);
        
        /// <summary>
        /// 服务器强制状态变化消息
        /// Server force state change message
        /// </summary>
        public static CsProtoStructurePacket<ServerSyncInfoNtf> ServerSyncInfoNtf =>
            new(CS_CMD_ID.CS_CMD_SERVER_ACTOR_FIFO_SYNC_NTF);

        public static CsProtoStructurePacket<PlayerRegionJumpRsp> PlayerRegionJumpRsp =>
            new(CS_CMD_ID.CS_CMD_PLAYER_REGION_JUMP_RSP);

        public static CsProtoStructurePacket<ChangeTownInstanceRsp> ChangeTownInstanceRsp =>
            new(CS_CMD_ID.CS_CMD_CHANGE_TOWN_INSTANCE_RSP);

        public static CsProtoStructurePacket<PlayerTeleport> PlayerTeleport =>
            new(CS_CMD_ID.CS_CMD_PLAYER_TELEPORT_NTF);

        //CS_CMD_SERVER_ACTOR_FIFO_SYNC_NTF - registered by client
        //CS_CMD_BATTLE_ACTOR_FIFO_SYNC_NTF - registered by client

        // ServerSyncInfoAck type="CSServerSyncInfoAck" id="CS_CMD_SERVER_ACTOR_FIFO_SYNC_ACK" desc="服务器强制状态变化的确认" Acknowledgment of Server Forced State Change
        // ServerSyncMsgNtf  type="CSServerSyncInfoNtf" id="CS_CMD_SERVER_ACTOR_FIFO_SYNC_NTF" desc="服务器强制状态变化消息"/> Server force state change message
        // FIFOSyncMsgNtf    type="CSFIFOSyncInfoNtf" id="CS_CMD_BATTLE_ACTOR_FIFO_SYNC_NTF" desc="C2表现FIFO同步消息"/> C2 represents FIFO synchronization messages
        // FIFOSyncMsg       type="CSFIFOSyncInfo" id="CS_CMD_BATTLE_ACTOR_FIFO_SYNC" desc="FIFO同步消息"/> FIFO synchronization message
    }
}