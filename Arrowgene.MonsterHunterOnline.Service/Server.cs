﻿using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;
using Arrowgene.MonsterHunterOnline.Service.System.Chat;
using Arrowgene.MonsterHunterOnline.Service.TQQApi;
using Arrowgene.MonsterHunterOnline.Service.TQQApi.Handler;
using Arrowgene.MonsterHunterOnline.Service.Web;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.MonsterHunterOnline.Service
{
    public class Server
    {
        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(Server));

        private readonly TpduConsumer _tpduConsumer;
        private readonly CsProtoPacketHandler _csProtoPacketHandler;
        private readonly CsProtoConsumer _battleServerConsumer;
        private readonly AsyncEventServer _server;
        private readonly AsyncEventServer _battleServer;
        private readonly Setting _setting;
        private readonly MhoWebServer _webServer;

        public Server(Setting setting)
        {
            _setting = new Setting(setting);
            _tpduConsumer = new TpduConsumer(_setting);
            _csProtoPacketHandler = new CsProtoPacketHandler(_setting);
            _battleServerConsumer = new CsProtoConsumer(_setting, _csProtoPacketHandler);
            _webServer = new MhoWebServer();

            _server = new AsyncEventServer(
                _setting.ListenIpAddress,
                _setting.ServerPort,
                _tpduConsumer,
                _setting.SocketSettings
            );

            _battleServer = new AsyncEventServer(
                _setting.ListenIpAddress,
                _setting.BattleServerPort,
                _battleServerConsumer,
                _setting.SocketSettings
            );

            Chat = new ChatSystem();

            LoadPacketHandler();
        }

        public ChatSystem Chat { get; }

        private void LoadPacketHandler()
        {
            _csProtoPacketHandler.AddHandler(new C2SCmdActivityAddSecretQuestHandler());
            _csProtoPacketHandler.AddHandler(new C2SCmdPetRngHandler());
            _csProtoPacketHandler.AddHandler(new C2SCmdSActivityListReqHandler());
            _csProtoPacketHandler.AddHandler(new C2SCmdShopRefreshShopsHandler());
            _csProtoPacketHandler.AddHandler(new CS2CmdDemonTrailGetLevelsPassTimeReq());
            _csProtoPacketHandler.AddHandler(new CS2CmdDemonTrailGetLevelsReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdBattleActorBeginMoveHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdBattleActorFifoSyncHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdBattleActorIdleMoveHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdBattleActorMoveStateHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdChangeTownInstanceReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdChatBroadcastReqHandler(Chat));
            _csProtoPacketHandler.AddHandler(new CsCmdChatEncryptData(_csProtoPacketHandler, Chat));
            _csProtoPacketHandler.AddHandler(new CsCmdCheckVersionHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdClientSendLogHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdCreateRoleReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdDataLoadHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdDragonBoxDetailReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdEnterLevelNtfHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdFileCheckHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdFriendsOnlineReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdGiftBagGroupStateReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdInstanceVerifyReq());
            _csProtoPacketHandler.AddHandler(new CsCmdItemReBuildLimitDataHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdMailUnreadGetReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdMartGoodsListReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdMultiNetIpInfoHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdPlayerExtNotifyHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdReselectRoleReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdSelectRoleHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdSystemEncryptData(_csProtoPacketHandler));
            _csProtoPacketHandler.AddHandler(new CsCmdSystemPkgTimerRecordHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdSystemTransAntiDataHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdTeamInfoGetReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdVipServiceExpireReqHandler());
            _csProtoPacketHandler.AddHandler(new CsCmdBattleActorStopmove());

            _tpduConsumer.AddHandler(new TpduCmdAuthHandler());
            _tpduConsumer.AddHandler(new TpduCmdSynAckHandler());
            _tpduConsumer.AddHandler(new TpduCmdNoneHandler(_csProtoPacketHandler));
            _tpduConsumer.AddHandler(new TpduCmdCloseHandler());
        }


        public void Start()
        {
            _webServer.Start();
            _server.Start();
            _battleServer.Start();
        }

        public void Stop()
        {
            _webServer.Stop();
            _server.Stop();
            _battleServer.Stop();
        }
    }
}