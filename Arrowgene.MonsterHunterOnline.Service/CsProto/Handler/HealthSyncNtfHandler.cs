using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class HealthSyncNtfHandler : CsProtoStructureHandler<HealthSyncNtf>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(HealthSyncNtfHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_HEALTH_SYNC;

    public override void Handle(Client client, HealthSyncNtf req)
    {
        CsCsProtoStructurePacket<HealthSyncNtf> healthSync = CsProtoResponse.HealthSyncNtf;

        var attr = client.Attr;
        int maxHp = attr != null && attr.CharMaxHP != null && attr.CharMaxHP.Length > 0
            ? attr.CharMaxHP[0]
            : 100;
        if (maxHp <= 0) maxHp = 100;
        int curHp = attr?.CharHP ?? maxHp;
        float ratio = (float)curHp / maxHp;
        if (ratio < 0f) ratio = 0f;
        if (ratio > 1f) ratio = 1f;

        healthSync.Structure.Health = ratio;
        healthSync.Structure.HealthRecover = ratio;
        healthSync.Structure.NetID = (int)client.Character.Id;

        client.SendCsProtoStructurePacket(healthSync);
    }
}
