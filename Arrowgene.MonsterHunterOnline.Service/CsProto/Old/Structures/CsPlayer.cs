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
    /// 角色
    /// </summary>
    public class CsPlayer : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CsPlayer));

        public CsPlayer()
        {
            Uid = 0;
            Zone = 0;
            Uin = 0;
            Rtid = 0;
        }

        /// <summary>
        /// 标识（全局）
        /// </summary>
        public ulong Uid;

        /// <summary>
        /// 服
        /// </summary>
        public uint Zone;

        /// <summary>
        /// 标识（TCONND）
        /// </summary>
        public ulong Uin;

        /// <summary>
        /// 标识（动态）
        /// </summary>
        public uint Rtid;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt64(Uid, Endianness.Big);
            buffer.WriteUInt32(Zone, Endianness.Big);
            buffer.WriteUInt64(Uin, Endianness.Big);
            buffer.WriteUInt32(Rtid, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            Uid = buffer.ReadUInt64(Endianness.Big);
            Zone = buffer.ReadUInt32(Endianness.Big);
            Uin = buffer.ReadUInt64(Endianness.Big);
            Rtid = buffer.ReadUInt32(Endianness.Big);
        }

    }
}
