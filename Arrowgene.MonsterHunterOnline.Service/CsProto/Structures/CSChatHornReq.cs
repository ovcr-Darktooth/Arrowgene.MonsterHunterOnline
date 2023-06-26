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
    /// 客户端发送的喇叭聊天请求
    /// </summary>
    public class CSChatHornReq : IStructure
    {

        public CSChatHornReq()
        {
            BagColumn = 0;
            BagGrid = 0;
            Content = "";
            Items = new CsChatItemsPkg();
        }

        /// <summary>
        /// 喇叭所在栏位置
        /// </summary>
        public byte BagColumn;

        /// <summary>
        /// 喇叭所在格子位
        /// </summary>
        public short BagGrid;

        /// <summary>
        /// 聊天内容
        /// </summary>
        public string Content;

        /// <summary>
        /// 聊天道具数据
        /// </summary>
        public CsChatItemsPkg Items;

        public void Write(IBuffer buffer)
        {
            buffer.WriteByte(BagColumn);
            buffer.WriteInt16(BagGrid, Endianness.Big);
            buffer.WriteInt32(Content.Length + 1, Endianness.Big);
            buffer.WriteCString(Content);
            Items.Write(buffer);
        }

    }
}