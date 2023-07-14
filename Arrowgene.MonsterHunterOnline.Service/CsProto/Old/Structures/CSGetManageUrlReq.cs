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
    /// 敏感操作管理页面URL请求
    /// </summary>
    public class CSGetManageUrlReq : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSGetManageUrlReq));

        public CSGetManageUrlReq()
        {
            Type = 0;
        }

        /// <summary>
        /// 管理页面类型
        /// </summary>
        public uint Type;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(Type, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            Type = buffer.ReadUInt32(Endianness.Big);
        }

    }
}
