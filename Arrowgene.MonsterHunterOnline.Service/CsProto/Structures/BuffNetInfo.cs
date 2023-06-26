/*
* This file is part of Arrowgene.MonsterHunterOnline
*
* Arrowgene.MonsterHunterOnline is a server implementation for the game "Monster Hunter Online".
* Copyright (C) 2023-2024 "all contributors"
*
* Github: https://github.com/sebastian-heinz/Arrowgene.MonsterHunterOnline
*
*  This program is free software: you can redistribute it and/or modify
*  it under the terms of the GNU Affero General Public License as published
*  by the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*
*  This program is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU Affero General Public License for more details.
*
*  You should have received a copy of the GNU Affero General Public License
*  along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

/* THIS IS AN AUTOGENERATED FILE. DO NOT EDIT THIS FILE DIRECTLY. */

using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{

    /// <summary>
    /// BUFF的网络消息结构
    /// </summary>
    public class BuffNetInfo : IStructure
    {

        public BuffNetInfo()
        {
            UniqueID = 0;
            TypeID = 0;
            OwnerID = 0;
            StackCount = 0;
            TriggerCount = 0;
            RetainTime = 0;
            Param = new List<byte>();
            isNew = 0;
            from = 0;
            EffectData = new List<BuffEffectData>();
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public int UniqueID;

        /// <summary>
        /// 类型ID
        /// </summary>
        public int TypeID;

        /// <summary>
        /// 谁上的
        /// </summary>
        public int OwnerID;

        /// <summary>
        /// 叠加数量
        /// </summary>
        public short StackCount;

        /// <summary>
        /// 触发次数
        /// </summary>
        public int TriggerCount;

        /// <summary>
        /// 持续时间 (毫秒)
        /// </summary>
        public int RetainTime;

        /// <summary>
        /// Buff动态数据
        /// </summary>
        public List<byte> Param;

        /// <summary>
        /// 是否是新加的
        /// </summary>
        public byte isNew;

        /// <summary>
        /// buff来源
        /// </summary>
        public ushort from;

        /// <summary>
        /// 效果数据
        /// </summary>
        public List<BuffEffectData> EffectData;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(UniqueID, Endianness.Big);
            buffer.WriteInt32(TypeID, Endianness.Big);
            buffer.WriteInt32(OwnerID, Endianness.Big);
            buffer.WriteInt16(StackCount, Endianness.Big);
            buffer.WriteInt32(TriggerCount, Endianness.Big);
            buffer.WriteInt32(RetainTime, Endianness.Big);
            ushort paramCount = (ushort)Param.Count;
            buffer.WriteUInt16(paramCount, Endianness.Big);
            for (int i = 0; i < paramCount; i++)
            {
                buffer.WriteByte(Param[i]);
            }
            buffer.WriteByte(isNew);
            buffer.WriteUInt16(from, Endianness.Big);
            ushort effectDataCount = (ushort)EffectData.Count;
            buffer.WriteUInt16(effectDataCount, Endianness.Big);
            for (int i = 0; i < effectDataCount; i++)
            {
                EffectData[i].Write(buffer);
            }
        }

    }
}