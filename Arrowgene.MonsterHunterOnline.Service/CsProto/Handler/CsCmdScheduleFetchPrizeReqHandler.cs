using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class CsCmdScheduleFetchPrizeReqHandler : ICsProtoHandler
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(CsCmdScheduleFetchPrizeReqHandler));


    public CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_SCHEDULE_FETCH_PRIZE_REQ;


    public void Handle(Client client, CsProtoPacket packet)
    {
        CSFetchPrizeReq req = new CSFetchPrizeReq();
        req.Read(packet.NewBuffer());

        client.SendCsPacket(NewCsPacket.FetchPrizeRes(new CSFetchPrizeRes()
        {
            PrizeId = req.PrizeId,
        }));
    }
}