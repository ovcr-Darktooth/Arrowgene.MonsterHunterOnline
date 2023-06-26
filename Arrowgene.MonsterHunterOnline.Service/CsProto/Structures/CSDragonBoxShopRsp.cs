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
    /// 小铺回应
    /// </summary>
    public class CSDragonBoxShopRsp : IStructure
    {

        public CSDragonBoxShopRsp()
        {
            ShopID = 0;
            ShopType = 0;
            ItemList = new List<CSItemBoxItemEntry>();
        }

        /// <summary>
        /// 商店ID
        /// </summary>
        public int ShopID;

        /// <summary>
        /// 商店类型
        /// </summary>
        public int ShopType;

        /// <summary>
        /// 物品列表(请注意其中的道具数量用来当做购买次数使用，道具数量请从表中读取)
        /// </summary>
        public List<CSItemBoxItemEntry> ItemList;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(ShopID, Endianness.Big);
            buffer.WriteInt32(ShopType, Endianness.Big);
            int itemListCount = (int)ItemList.Count;
            buffer.WriteInt32(itemListCount, Endianness.Big);
            for (int i = 0; i < itemListCount; i++)
            {
                ItemList[i].Write(buffer);
            }
        }

    }
}