using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class ScheduleFetchPrizeReqHandler : CsProtoStructureHandler<FetchPrizeReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(ScheduleFetchPrizeReqHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_SCHEDULE_FETCH_PRIZE_REQ;


    public override void Handle(Client client, FetchPrizeReq req)
    {
        Logger.Info($"PrizeId: {req.PrizeId}");

        CsProtoStructurePacket<FetchPrizeRes> fetchPrize = CsProtoResponse.FetchPrizeRes;

        fetchPrize.Structure.PrizeId = req.PrizeId;
        client.SendCsProtoStructurePacket(fetchPrize);

        //TODO: send the items to the client if after baginit it's not giving the item

    }
}
