using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.System;
using Arrowgene.MonsterHunterOnline.Service.System.CharacterSystem;
using System.Threading;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Handler;

public class EnterLevelNtfHandler : CsProtoStructureHandler<EnterLevelNtf>
{
    private static readonly ServiceLogger Logger =
        LogProvider.Logger<ServiceLogger>(typeof(EnterLevelNtfHandler));

    public override CS_CMD_ID Cmd => CS_CMD_ID.CS_CMD_ENTER_LEVEL_NTF;

    private readonly CharacterManager _characterManager;

    public EnterLevelNtfHandler(CharacterManager characterManager)
    {
        _characterManager = characterManager;
    }

    public override void Handle(Client client, EnterLevelNtf req)
    {
        //_characterManager.SyncAllAttr(client);
        if (client.State.MainInstanceLevelId == 100292)
        {
            Thread.Sleep(2000);
            client.SendCsPacket(NewCsPacket.PlayerTeleport(new CSPlayerTeleport()
            {
                SyncTime = 0,
                NetObjId = client.Character.Id,
                Region = client.State.levelId,
                TargetPos = new CSQuatT()
                {
                    t = new CSVec3()
                    {
                        x = 300.0f,
                        y = 255.0f,
                        z = 358.0f
                    }
                },
                ParentGUID = 1,
                InitState = 1
            }
            ));
        }

        /*CsCsProtoStructurePacket<EntityAppearNtfIdList> entityAppearNtfIdList = CsProtoResponse.EntityAppearNtfIdList;
        LogicEntityId leId = new LogicEntityId();
        leId.Type = LogicEntityType.MH_LETYPE_MONSTER;
        leId.Id = 60030;
        entityAppearNtfIdList.Structure.InitType = 1;
        entityAppearNtfIdList.Structure.LogicEntityId.Add(leId.Id);
        //entityAppearNtfIdList.Structure.LogicEntityType.Add((uint)leId.Type);
        entityAppearNtfIdList.Structure.LogicEntityType.Add((uint)1);
        Logger.Debug($"{leId.Type}");

        entityAppearNtfIdList.Structure.LogicEntityId.Add(60030);
        //entityAppearNtfIdList.Structure.LogicEntityType.Add((uint)leId.Type);
        entityAppearNtfIdList.Structure.LogicEntityType.Add((uint)2);
        client.SendCsProtoStructurePacket(entityAppearNtfIdList);*/
    }
}