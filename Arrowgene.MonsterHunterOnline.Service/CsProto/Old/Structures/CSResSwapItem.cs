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
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Enums;

namespace Arrowgene.MonsterHunterOnline.Service.CsProto.Structures
{

    /// <summary>
    /// 回复交换2个位置的物品
    /// </summary>
    public class CSResSwapItem : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSResSwapItem));

        public CSResSwapItem()
        {
            Ret = 0;
            SrcColumn = 0;
            SrcIndex = 0;
            DstColumn = 0;
            DstIndex = 0;
        }

        /// <summary>
        /// 处理结果
        /// </summary>
        public int Ret;

        /// <summary>
        /// 原栏标志
        /// </summary>
        public byte SrcColumn;

        /// <summary>
        /// 原栏位置
        /// </summary>
        public ushort SrcIndex;

        /// <summary>
        /// 目标栏标志
        /// </summary>
        public byte DstColumn;

        /// <summary>
        /// 目标栏位置
        /// </summary>
        public ushort DstIndex;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(Ret, Endianness.Big);
            buffer.WriteByte(SrcColumn);
            buffer.WriteUInt16(SrcIndex, Endianness.Big);
            buffer.WriteByte(DstColumn);
            buffer.WriteUInt16(DstIndex, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            Ret = buffer.ReadInt32(Endianness.Big);
            SrcColumn = buffer.ReadByte();
            SrcIndex = buffer.ReadUInt16(Endianness.Big);
            DstColumn = buffer.ReadByte();
            DstIndex = buffer.ReadUInt16(Endianness.Big);
        }

    }
}
