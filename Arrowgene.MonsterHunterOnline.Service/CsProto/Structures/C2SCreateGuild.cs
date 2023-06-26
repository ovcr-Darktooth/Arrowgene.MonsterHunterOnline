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
    /// 创建猎团
    /// </summary>
    public class C2SCreateGuild : IStructure
    {

        public C2SCreateGuild()
        {
            Name = "";
            Icon = 0;
            Note = "";
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 图标
        /// </summary>
        public int Icon;

        /// <summary>
        /// 注释
        /// </summary>
        public string Note;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(Name.Length + 1, Endianness.Big);
            buffer.WriteCString(Name);
            buffer.WriteInt32(Icon, Endianness.Big);
            buffer.WriteInt32(Note.Length + 1, Endianness.Big);
            buffer.WriteCString(Note);
        }

    }
}