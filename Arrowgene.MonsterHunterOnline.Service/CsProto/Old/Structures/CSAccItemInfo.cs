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

    public class CSAccItemInfo : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSAccItemInfo));

        public CSAccItemInfo()
        {
            Column = 0;
            Grid = 0;
            ItemId = 0;
            Count = 0;
            BindType = 0;
        }

        /// <summary>
        /// 物品所在栏位
        /// </summary>
        public byte Column;

        /// <summary>
        /// 物品所在格子
        /// </summary>
        public ushort Grid;

        /// <summary>
        /// 道具实例ID
        /// </summary>
        public ulong ItemId;

        /// <summary>
        /// 物品数量
        /// </summary>
        public ushort Count;

        /// <summary>
        /// 绑定类型
        /// </summary>
        public int BindType;

        public void Write(IBuffer buffer)
        {
            buffer.WriteByte(Column);
            buffer.WriteUInt16(Grid, Endianness.Big);
            buffer.WriteUInt64(ItemId, Endianness.Big);
            buffer.WriteUInt16(Count, Endianness.Big);
            buffer.WriteInt32(BindType, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            Column = buffer.ReadByte();
            Grid = buffer.ReadUInt16(Endianness.Big);
            ItemId = buffer.ReadUInt64(Endianness.Big);
            Count = buffer.ReadUInt16(Endianness.Big);
            BindType = buffer.ReadInt32(Endianness.Big);
        }

    }
}
