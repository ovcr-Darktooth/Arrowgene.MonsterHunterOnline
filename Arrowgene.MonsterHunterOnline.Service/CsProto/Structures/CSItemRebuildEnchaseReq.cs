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
    /// 请求镶嵌装备
    /// </summary>
    public class CSItemRebuildEnchaseReq : IStructure
    {

        public CSItemRebuildEnchaseReq()
        {
            EquipID = 0;
            EquipColumn = 0;
            EquipGrid = 0;
            DstSlot = 0;
            ItemID = 0;
            Column = 0;
            Grid = 0;
        }

        /// <summary>
        /// 物品实例
        /// </summary>
        public ulong EquipID;

        /// <summary>
        /// 装备所在栏位
        /// </summary>
        public byte EquipColumn;

        /// <summary>
        /// 装备所在格子
        /// </summary>
        public ushort EquipGrid;

        /// <summary>
        /// 装备槽
        /// </summary>
        public byte DstSlot;

        /// <summary>
        /// 物品实例
        /// </summary>
        public ulong ItemID;

        /// <summary>
        /// 技能珠所在栏位
        /// </summary>
        public byte Column;

        /// <summary>
        /// 技能珠所在格子
        /// </summary>
        public ushort Grid;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt64(EquipID, Endianness.Big);
            buffer.WriteByte(EquipColumn);
            buffer.WriteUInt16(EquipGrid, Endianness.Big);
            buffer.WriteByte(DstSlot);
            buffer.WriteUInt64(ItemID, Endianness.Big);
            buffer.WriteByte(Column);
            buffer.WriteUInt16(Grid, Endianness.Big);
        }

    }
}