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
            sync.BonusId = 1;
            sync.Data.Int = 60; // hp
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 20; //sta
            sync.BonusId = 0;
            sync.Data.Float = 50;
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
            sync.AttrId = 128; //staflag 0 = disabled, 1 = enabled in theory
            sync.BonusId = 0;
            sync.Data.Int = 0;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 129; //stareduce
            sync.BonusId = 1;
            sync.Data.Float = 5;
            attrs.Add(sync);

            sync = new AttrSync();
            sync.EntityId = character.Id;
            sync.AttrId = 130; //stareduceflag
            sync.BonusId = 0;
            sync.Data.Int = -1;
            attrs.Add(sync);



            return attrs;
        }
    }
}