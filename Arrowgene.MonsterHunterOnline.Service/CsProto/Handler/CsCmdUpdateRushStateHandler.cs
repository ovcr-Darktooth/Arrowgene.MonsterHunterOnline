using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class CsCmdUpdateRushStateHandler : ICsProtoHandler
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(CsCmdUpdateRushStateHandler));


    public CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_UPDATE_RUSHSTATE;


    public void Handle(Client client, CsProtoPacket packet)
    {
        CSUpdateRushState req = new CSUpdateRushState();
        req.Read(packet.NewBuffer());

        //Logger.Info($"update rushstate: rush:{req.Rush} type:{req.Type}");
        client.SendCsPacket(NewCsPacket.UpdateRushState(new CSUpdateRushState()
        {
            Rush = req.Rush,
            Type = req.Type
        }));
    }
}