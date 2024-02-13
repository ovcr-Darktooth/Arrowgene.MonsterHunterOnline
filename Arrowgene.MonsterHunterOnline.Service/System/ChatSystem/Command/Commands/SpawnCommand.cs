using System.Collections.Generic;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.System.ChatSystem.Command.Commands
{
    /// <summary>
    /// Spawns enteties
    /// </summary>
    public class SpawnCommand : ChatCommand
    {
        private static readonly ServiceLogger Logger =
            LogProvider.Logger<ServiceLogger>(typeof(SpawnCommand));

        public override AccountType Account => AccountType.User;
        public override string Key => "spawn";
        public override string HelpText => "usage: `/spawn` - spawns entity";

        public override void Execute(string[] command, Client client, ChatMessage message, List<ChatMessage> responses)
        {
            

            CsCsProtoStructurePacket<BattleActorAttachEntity> attachEntity = CsProtoResponse.BattleActorAttachEntity;
            attachEntity.Structure.SyncTime = 0;
            attachEntity.Structure.ParentGUID = 50080;
            attachEntity.Structure.Location = new CSVec3() {
                x = 1620,
                y = 1635,
                z = 143
            };
            attachEntity.Structure.ActorRot = new CSQuat()
            {
                v = new CSVec3()
                {
                    x = 0f,
                    y = 0f,
                    z = 0f
                },
                w = 0f,
            };
            //client.SendCsProtoStructurePacket(attachEntity);

            CsCsProtoStructurePacket<BattleActorAttachEntityNtf> attachEntityNtf = CsProtoResponse.BattleActorAttachEntityNtf;
            attachEntityNtf.Structure.NetObjId = client.Character.Id;
            attachEntityNtf.Structure.SyncTime = 0;
            attachEntityNtf.Structure.ParentGUID = 50080;
            attachEntityNtf.Structure.Location = new CSVec3()
            {
                x = 1620,
                y = 1635,
                z = 143
            };
            attachEntityNtf.Structure.ActorRot = new CSQuat()
            {
                v = new CSVec3()
                {
                    x = 0f,
                    y = 0f,
                    z = 0f
                },
                w = 0f,
            };
            //client.SendCsProtoStructurePacket(attachEntityNtf);

            CsCsProtoStructurePacket<EntityAppearNtfIdList> entityAppearNtfIdList = CsProtoResponse.EntityAppearNtfIdList;
            LogicEntityId leId = new LogicEntityId();
            leId.Type = LogicEntityType.MH_LETYPE_MONSTER;
            leId.Id = 50080;
            entityAppearNtfIdList.Structure.InitType = 4;
            entityAppearNtfIdList.Structure.LogicEntityId.Add(leId.Id);
            entityAppearNtfIdList.Structure.LogicEntityType.Add((uint)leId.Type);
            //client.SendCsProtoStructurePacket(entityAppearNtfIdList);

            CsCsProtoStructurePacket<SpawnCollectTrigSYNC> spawnCollectTrigSYNC = CsProtoResponse.SpawnCollectTrigSYNC;
            spawnCollectTrigSYNC.Structure.ItemID = 1; //potion is the next item to collect ? don't think so
            spawnCollectTrigSYNC.Structure.BoxParam = new CSVec3()
            {
                x = 1601.128f,
                y = 1608.0612f,
                z = 142.376f
            };
            spawnCollectTrigSYNC.Structure.TriggerType = "kill";
            spawnCollectTrigSYNC.Structure.Position = new CSVec3()
            {
                x = 1601.128f,
                y = 1608.0612f,
                z = 142.376f
            };
            spawnCollectTrigSYNC.Structure.Rotation = new CSQuat()
            {
                v = new CSVec3()
                {
                    x = 0f,
                    y = 0f,
                    z = 0f
                },
                w = 0f,
            };
            spawnCollectTrigSYNC.Structure.RelativeID = 50050;
            client.SendCsProtoStructurePacket(spawnCollectTrigSYNC);
        }
    }
}