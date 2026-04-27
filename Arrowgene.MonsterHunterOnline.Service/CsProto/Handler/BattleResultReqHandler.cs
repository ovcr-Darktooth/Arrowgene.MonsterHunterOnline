using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Constant;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System;
using Arrowgene.MonsterHunterOnline.Service.System.CharacterSystem;
using System;
using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

/// <summary>
/// Handles client request to settle battle/instance results (CS_CMD_BATTLE_RESULT_REQ).
/// Responds with CSInstanceResultRsp containing minimal result information.
/// </summary>
public class BattleResultReqHandler : CsProtoStructureHandler<CSInstanceResultReq>
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(BattleResultReqHandler));

    private readonly CharacterManager _characterManager;

    public BattleResultReqHandler(CharacterManager characterManager)
    {
        _characterManager = characterManager;
    }

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_BATTLE_RESULT_REQ;

    public override void Handle(Client client, CSInstanceResultReq req)
    {
        try
        {
            Logger.Info(client, $"Received battle result request: PlayerID={req.PlayerID} RewardFlag={req.RewardFlag} for Levelid : {client.State?.levelId ?? 0}");

            // Basic validation: ensure client has character
            if (client.Character == null)
            {
                Logger.Error(client, "BattleResultReq: no character attached to client");
                return;
            }

            // TODO: perform reward calculation / persistence here based on req.RewardFlag
            CsCsProtoStructurePacket<CSInstanceResultRsp> instanceResultRsp = CsProtoResponse.InstanceResultRsp;
            instanceResultRsp.Structure.LevelID = client.State?.levelId ?? 0;
            instanceResultRsp.Structure.GameMode = (int)GameMode.Story;
            instanceResultRsp.Structure.HuntingMode = 0;

            instanceResultRsp.Structure.FakeItemInfo.FakeItemID.Add(1); // idk, potion test ?
            instanceResultRsp.Structure.FakeItemInfo.ActItemID.Add(1);

            instanceResultRsp.Structure.SelfResult = new CSPlayerResultInfo();

            //instanceResultRsp.Structure.SelfResult.BaseInfo
            //instanceResultRsp.Structure.SelfResult.StatInfo
            //instanceResultRsp.Structure.SelfResult.RewardInfoList
            //instanceResultRsp.Structure.SelfResult.SizeChangeInfoList


            instanceResultRsp.Structure.InstanceStatResult = new InstanceResultStatInfo();

            //did try 0-4 (5 stats), got a crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(0); //time //no crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(31);
            /*instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(1); //time //no crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(100);
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(2); //time //no crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(55);
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(3); //time //no crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(0);
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(4); //time //no crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(101);*/

            //did with type id
            //instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(4); //time //no crash
            //instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(30);
            /*instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(46); //damage percent //crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(100);*/
            /*instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(65); //damage taken //crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(33);*/
            /*instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(45); //auxiliary ? //crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(1);*/
            /*instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(5); //score //crash
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(100);*/



            //instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(97); //score //not crashing but nothing visible
            //instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(8800);

            /*instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(300); //damage taken //crash so maybe not the good value
            instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(101);*/

            //instanceResultRsp.Structure.InstanceStatResult.InstanceDataType.Add(399); //zenny //crash so maybe not the good value
            //instanceResultRsp.Structure.InstanceStatResult.InstanceDataValue.Add(44000);


            // 4 entry max ? Weapon Id as faction id ? 12 data type (dmg dealt/weapon hits/hh songs plyd/max dmg/topple/status eff/traps used/broken parts/mobs killed/hits taken/carted(died)/potion used)
            FactionResultStatInfo factionResult = new FactionResultStatInfo();
            factionResult.FactionID = 1;
            factionResult.FactionDataType.Add(1);
            factionResult.FactionDataValue.Add(2);

            //instanceResultRsp.Structure.FactionStatResult.Add(factionResult);


            //maybe old method ? because there is a SizeChangeInfoList in SelfResult
            //couldn't make it work
            MonsterSizeChange newCaeserberSize = new MonsterSizeChange();
            newCaeserberSize.MonsterID = 60010;
            newCaeserberSize.OldType = 0; // Careful on enum name : EMonsterSizeLevel
            newCaeserberSize.OldSize = 10;
            newCaeserberSize.ChangeType = 1; // Careful on enum name : EMonsterSizeType
            newCaeserberSize.NewSize = 20;

            instanceResultRsp.Structure.SelfResult.SizeChangeInfoList.Add(newCaeserberSize);


            instanceResultRsp.Structure.OtherResultList = new List<CSOtherResultInfo>();
            client.SendCsProtoStructurePacket(instanceResultRsp);


            Logger.Info(client, "Sent CSInstanceResultRsp (battle result)");
        }
        catch (Exception ex)
        {
            Logger.Exception(client, ex);
        }
    }
}
