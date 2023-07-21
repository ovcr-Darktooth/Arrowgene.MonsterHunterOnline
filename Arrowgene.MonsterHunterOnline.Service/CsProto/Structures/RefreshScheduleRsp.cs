using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{
    /// <summary>
    /// 日程表刷新
    /// </summary>
    public class RefreshScheduleRsp : Structure
    {
        public RefreshScheduleRsp()
        {
            Group = 0;
            RefreshTime = 0;
        }

        /// <summary>
        /// 组
        /// </summary>
        public int Group;

        /// <summary>
        /// 刷新时间
        /// </summary>
        public uint RefreshTime;


        public override void Write(IBuffer buffer)
        {
            WriteInt32(buffer, Group);
            WriteUInt32(buffer, RefreshTime);
        }

        public override void Read(IBuffer buffer)
        {
            Group = ReadInt32(buffer);
            RefreshTime = ReadUInt32(buffer);
        }
    }
}