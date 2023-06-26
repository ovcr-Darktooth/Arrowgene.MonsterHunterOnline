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
    /// 同步开包厢内容
    /// </summary>
    public class CSItemBoxOpenBoxNtf : IStructure
    {

        public CSItemBoxOpenBoxNtf()
        {
            Box = 0;
            ItemList = new List<CSItemBoxItemEntry>();
            EquipList = new List<CSItemData>();
            BagFull = 0;
        }

        /// <summary>
        /// 宝箱ID
        /// </summary>
        public int Box;

        /// <summary>
        /// 物品列表
        /// </summary>
        public List<CSItemBoxItemEntry> ItemList;

        /// <summary>
        /// 装备列表
        /// </summary>
        public List<CSItemData> EquipList;

        /// <summary>
        /// 背包是否满 1:满 0 ： 不满
        /// </summary>
        public byte BagFull;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(Box, Endianness.Big);
            byte itemListCount = (byte)ItemList.Count;
            buffer.WriteByte(itemListCount);
            for (int i = 0; i < itemListCount; i++)
            {
                ItemList[i].Write(buffer);
            }
            byte equipListCount = (byte)EquipList.Count;
            buffer.WriteByte(equipListCount);
            for (int i = 0; i < equipListCount; i++)
            {
                EquipList[i].Write(buffer);
            }
            buffer.WriteByte(BagFull);
        }

    }
}