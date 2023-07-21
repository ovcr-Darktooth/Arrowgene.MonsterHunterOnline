using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{
    /// <summary>
    /// 日程表刷新
    /// </summary>
    public class FetchPrizeReq : Structure
    {
        public FetchPrizeReq()
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
