using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class MainInstanceEnterOptReqHandler : CsProtoStructureHandler<MainInstanceEnterOptReq>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(MainInstanceEnterOptReqHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_MAIN_INSTANCE_ENTER_OPT_REQ;

    public MainInstanceEnterOptReqHandler()
    {
    }

    public override void Handle(Client client, MainInstanceEnterOptReq req)
    {
        Logger.Info($"Level requested: ${req.LevelId}");

        if (req.LevelId == 191101) // For some reason the button enter the Weapon Training takes us to the wrong level (arena_010)
            req.LevelId = 140900; // Set levelId to the Actual Weapon Training level (arena_003)

        client.State.MainInstanceLevelId = req.LevelId;

        CsCsProtoStructurePacket<MainInstanceEnterOptRsp> rsp = CsProtoResponse.MainInstanceEnterOptRsp;
        rsp.Structure.NetId = (int)client.Character.Id;
        rsp.Structure.ErrCode = 0;
        rsp.Structure.LevelId = req.LevelId;
        rsp.Structure.WarningFlag = 0;
        rsp.Structure.UIInfos.Add(new BSMRoomUIPlayerInfo()
        {
            RoleIndex =1,
            CharLevel = (int)client.Character.Level,
            Weapon = 0,
            BoxId = 0,
            RoleName = client.Character.Name,
            StarLevel = client.Character.StarLevel,
            Faction = 0,
            Officer = 0, // mentor 1
            HRLevel = client.Character.HrLevel,
            BigRand = 0,
        });
        rsp.Structure.UseEmploye = req.UseEmploye;
        rsp.Structure.WeaponTrialLevel = 0;
        rsp.Structure.WeaponType = 0;
        client.SendCsProtoStructurePacket(rsp);
    }
}