using System.Collections.Generic;
using Arrowgene.MonsterHunterOnline.Service.CsProto;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Structures;
using Arrowgene.MonsterHunterOnline.Service.System.CharacterSystem;

namespace Arrowgene.MonsterHunterOnline.Service.System.ChatSystem.Command.Commands
{
    /// <summary>
    /// test random stuff
    /// </summary>
    public class TestCommand : ChatCommand
    {
        public override AccountType Account => AccountType.User;
        public override string Key => "test";
        public override string HelpText => "usage: `/test` - test random stuff";

        private readonly CharacterManager _characterManager;

        public TestCommand(CharacterManager characterManager)
        {
            _characterManager = characterManager;
        }

        public override void Execute(string[] command, Client client, ChatMessage message, List<ChatMessage> responses)
        {
            //client.SendCsPacket(NewCsPacket.SelectHuntingBagRsp(new CSSelectHuntingBagRsp()));
           
        //   client.SendCsPacket(NewCsPacket.HunterStarInitNtf(new CSHunterStarInitNtf()
        //   {
        //       Entry = 0
        //   }));

            CsCsProtoStructurePacket<EnterInstanceCountDown>
                enterInstanceCountDown = CsProtoResponse.EnterInstanceCountDown;

            enterInstanceCountDown.Structure.Second = 5;
            enterInstanceCountDown.Structure.LevelId = client.State.MainInstanceLevelId;
            //client.SendCsProtoStructurePacket(enterInstanceCountDown);

            CsCsProtoStructurePacket<PlayerInitInfo> playerInitInfo = CsProtoResponse.PlayerInitInfo;
            playerInitInfo.Structure.Pose.t.x = 15f;
            playerInitInfo.Structure.Pose.t.y = 15f;
            playerInitInfo.Structure.Pose.t.z = 2f;
            //_characterManager.PopulatePlayerInitInfo(client, client.Character, playerInitInfo.Structure);
            //client.SendCsProtoStructurePacket(playerInitInfo);

            //CsCsProtoStructurePacket<InstanceVerifyRsp> instanceVerifyRsp = CsProtoResponse.InstanceVerifyRsp;
            //client.SendCsProtoStructurePacket(instanceVerifyRsp);

            SyncAllAttr(client);

        }

        public void SyncAllAttr(Client client)
        {
            List<AttrSync> attrs = GetStaAttrSync(client, client.Character);
            List<List<AttrSync>> attrChunks = Util.Chunk(attrs, CsProtoConstant.CS_ATTR_SYNC_LIST_MAX);

            foreach (List<AttrSync> attrChunk in attrChunks)
            {
                if (attrChunk.Count > CsProtoConstant.CS_ATTR_SYNC_LIST_MAX)
                {
                    //Logger.Error(client, "Chunk error");
                }

                CsCsProtoStructurePacket<AttrSyncList> attrSyncList = CsProtoResponse.AttrSyncList;
                for (int i = 0; i < attrChunk.Count; i++)
                {
                    attrSyncList.Structure.Attr.Add(attrChunk[i]);
                }

                client.SendCsProtoStructurePacket(attrSyncList);
            }
        }
        public List<AttrSync> GetStaAttrSync(Client client, Character character)
        {
            List<AttrSync> attrs = new List<AttrSync>();

            AttrSync sync;


            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 15;
            sync.BonusId = 0;
            sync.Data.Int = 10; // hp
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 16;
            sync.BonusId = 1;
            sync.Data.Int = 20; // maxhp
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 17;
            sync.BonusId = 0;
            sync.Data.Int = 1; // reju
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 18;
            sync.BonusId = 1;
            sync.Data.Int = 10; // maxreju
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 20; //sta
            sync.BonusId = 0;
            sync.Data.Float = 100;
            attrs.Add(sync);


            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 21; //maxsta
            sync.BonusId = 1;
            sync.Data.Int = 100;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 22; //starecovery
            sync.BonusId = 1;
            sync.Data.Int = 2;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 23; //staddct
            sync.BonusId = 1;
            sync.Data.Int = 25;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 24; //staddctperiod
            sync.BonusId = 1;
            sync.Data.Int = 360;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 128; //staflag // natural endurance restoration
            sync.BonusId = 0; // official translation : The program is for self-use, and the endurance is restored naturally. 0 means enabled, 1 means disabled.
            sync.Data.Int = 0;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 208; //BackBossRunStaReduce
            sync.BonusId = 1;
            sync.Data.Int = 1;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 129; //stareduce
            sync.BonusId = 1;
            sync.Data.Float = 1;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 130; //stareduceflag
            sync.BonusId = 0;
            sync.Data.Int = 1;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 137; //CurStaReduce
            sync.BonusId = 1;
            sync.Data.Int = 0;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 132; //DefenceState
            sync.BonusId = 0;
            sync.Data.Int = 0;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 109; //CharRage
            sync.BonusId = 0;
            sync.Data.Int = 10000;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 110; //CharMaxRage
            sync.BonusId = 1;
            sync.Data.Int = 20000;
            attrs.Add(sync);




            return attrs;
        }
    }
}