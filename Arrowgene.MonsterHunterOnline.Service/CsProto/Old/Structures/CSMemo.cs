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

    public class CSMemo : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSMemo));

        public CSMemo()
        {
            title = "";
            mailFromName = "";
            mailFrom = 0;
            accessoriesNum = 0;
            itemId = 0;
            itemCount = 0;
        }

        /// <summary>
        /// 邮件标题名称长度
        /// </summary>
        public string title;

        /// <summary>
        /// 邮件角色类型和发件人
        /// </summary>
        public string mailFromName;

        /// <summary>
        /// 邮件角色ID
        /// </summary>
        public ulong mailFrom;

        /// <summary>
        /// 附件数量
        /// </summary>
        public int accessoriesNum;

        /// <summary>
        /// 道具ID
        /// </summary>
        public uint itemId;

        /// <summary>
        /// 道具数量
        /// </summary>
        public byte itemCount;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(title.Length + 1, Endianness.Big);
            buffer.WriteCString(title);
            buffer.WriteInt32(mailFromName.Length + 1, Endianness.Big);
            buffer.WriteCString(mailFromName);
            buffer.WriteUInt64(mailFrom, Endianness.Big);
            buffer.WriteInt32(accessoriesNum, Endianness.Big);
            buffer.WriteUInt32(itemId, Endianness.Big);
            buffer.WriteByte(itemCount);
        }

        public void Read(IBuffer buffer)
        {
            int titleEntryLen = buffer.ReadInt32(Endianness.Big);
            title = buffer.ReadString(titleEntryLen);
            int mailFromNameEntryLen = buffer.ReadInt32(Endianness.Big);
            mailFromName = buffer.ReadString(mailFromNameEntryLen);
            mailFrom = buffer.ReadUInt64(Endianness.Big);
            accessoriesNum = buffer.ReadInt32(Endianness.Big);
            itemId = buffer.ReadUInt32(Endianness.Big);
            itemCount = buffer.ReadByte();
        }

    }
}
