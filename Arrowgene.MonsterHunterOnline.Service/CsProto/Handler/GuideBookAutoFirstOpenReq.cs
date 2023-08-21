using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class GuideBookAutoFirstOpenReq : CsProtoStructureHandler<CSGuideBookAutoFirstOpenReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(GuideBookAutoFirstOpenReq));

    public override CS_CMD_ID Cmd => CS_CMD_ID.C2S_CMD_GUIDE_BOOK_AUTO_FIRST_OPEN_REQ;


    public GuideBookAutoFirstOpenReq()
    {
    }

    public override void Handle(Client client, CSGuideBookAutoFirstOpenReq req)
    {
        CsProtoStructurePacket<SCGuideBookAutoFirstOpenRsp> rsp = CsProtoResponse.GuideBookAutoFirstOpenRsp;
        rsp.Structure.ErrCode = 0;
        client.SendCsProtoStructurePacket(rsp);
    }
}