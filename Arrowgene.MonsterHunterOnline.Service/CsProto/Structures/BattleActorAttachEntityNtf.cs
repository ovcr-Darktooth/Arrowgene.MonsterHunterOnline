using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{
    public class BattleActorAttachEntityNtf : Structure, ICsStructure
    {
        public BattleActorAttachEntityNtf()
        {
            NetObjId = 0;
            SyncTime = 0;
            ParentGUID = 0;
            Location = new CSVec3();
            ActorRot = new CSQuat();
        }

        /// <summary>
        /// 需要同步的Actor的NetObjId
        /// </summary>
        public uint NetObjId;

        /// <summary>
        /// 同步时间
        /// </summary>
        public long SyncTime;

        /// <summary>
        /// parent entityGUID
        /// </summary>
        public ulong ParentGUID;

        /// <summary>
        /// 世界坐标系下的位置
        /// </summary>
        public CSVec3 Location;

        /// <summary>
        /// Actor朝向
        /// </summary>
        public CSQuat ActorRot;

        public void WriteCs(IBuffer buffer)
        {
            WriteUInt32(buffer, NetObjId);
            WriteInt64(buffer, SyncTime);
            WriteUInt64(buffer, ParentGUID);
            WriteCsStructure(buffer, Location);
            WriteCsStructure(buffer, ActorRot);
        }

        public void ReadCs(IBuffer buffer)
        {
            NetObjId = ReadUInt32(buffer);
            SyncTime = ReadInt64(buffer);
            ParentGUID = ReadUInt64(buffer);
            Location = ReadCsStructure(buffer, Location);
            ActorRot = ReadCsStructure(buffer, ActorRot);
        }
    }
}
