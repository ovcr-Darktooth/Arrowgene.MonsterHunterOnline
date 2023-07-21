using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{
    public class FetchPrizeRes : Structure
    {
        public FetchPrizeRes()
        {
            PrizeId = 0;
        }

        /// <summary>
        /// 奖励包id
        /// </summary>
        public uint PrizeId;


        public override void Write(IBuffer buffer)
        {
            WriteUInt32(buffer, PrizeId);
        }

        public override void Read(IBuffer buffer)
        {
            PrizeId = ReadUInt32(buffer);
        }
    }
}
