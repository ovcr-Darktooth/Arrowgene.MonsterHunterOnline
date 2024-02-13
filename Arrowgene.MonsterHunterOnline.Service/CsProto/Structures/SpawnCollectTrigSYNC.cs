using System;
using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System.ItemSystem;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{
    /// <summary>
    /// 下发EntityID
    /// </summary>
    public class SpawnCollectTrigSYNC : Structure, ICsStructure
    {
        public SpawnCollectTrigSYNC()
        {
            ItemID = 0;
            BoxParam = new CSVec3();
            TriggerType = "";
            Position = new CSVec3();
            Rotation = new CSQuat();
            RelativeID = 0;
        }

        /// <summary>
        /// 需要同步的生成物品的ID
        /// </summary>
        public int ItemID;

        /// <summary>
        /// Trigger的大小
        /// </summary>
        public CSVec3 BoxParam;

        /// <summary>
        /// Trigger的类型名称
        /// </summary>
        public string TriggerType;

        /// <summary>
        /// Trigger的位置
        /// </summary>
        public CSVec3 Position;

        /// <summary>
        /// Trigger的旋转
        /// </summary>
        public CSQuat Rotation;

        /// <summary>
        /// 需要参考的Monster的NetObjId
        /// </summary>
        public uint RelativeID;

        public  void WriteCs(IBuffer buffer)
        {
            WriteInt32(buffer, ItemID);
            WriteCsStructure(buffer, BoxParam);
            WriteString(buffer, TriggerType);
            WriteCsStructure(buffer, Position);
            WriteCsStructure(buffer, Rotation);
            WriteUInt32(buffer, RelativeID);
        }

        public void ReadCs(IBuffer buffer)
        {
            ItemID = ReadInt32(buffer);
            BoxParam = ReadCsStructure(buffer, BoxParam);
            TriggerType = ReadString(buffer);
            Position = ReadCsStructure(buffer, Position);
            Rotation = ReadCsStructure(buffer, Rotation);
            RelativeID = ReadUInt32(buffer);

        }
    }
}