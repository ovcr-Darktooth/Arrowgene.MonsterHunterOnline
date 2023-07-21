using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{
    /// <summary>
    /// 日程表刷新
    /// </summary>
    public class RefreshSchedule : Structure
    {
        public RefreshSchedule()
        {
            Task = 0;
        }

        /// <summary>
        /// 任务
        /// </summary>
        public int Task { get; set; }


        public override void Write(IBuffer buffer)
        {
            WriteInt32(buffer, Task);
        }

        public override void Read(IBuffer buffer)
        {
            Task = ReadInt32(buffer);
        }
    }
}