using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.System;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class TaskRefreshScheduleHandler : CsProtoStructureHandler<RefreshSchedule>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(TaskRefreshScheduleHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.C2S_CMD_TASK_REFRESHSCHEDULE;


    public override void Handle(Client client, RefreshSchedule req)
    {
        Logger.Info($"Task refresh schedule: {req.Task}");


        CsProtoStructurePacket<RefreshScheduleRsp> refreshRsp = CsProtoResponse.RefreshScheduleRsp;

        //TODO: understand group/refreshtime

        // 0/1 still makes crash, refreshtime the problem ?
        refreshRsp.Structure.Group = 1; // group of quests to show ?
        refreshRsp.Structure.RefreshTime = (uint)Util.GetUnixTime(client.Character.Created)+3600; // next reset ?

        //Makes the client crash when sending for now
        //client.SendCsProtoStructurePacket(refreshRsp);

    }
}