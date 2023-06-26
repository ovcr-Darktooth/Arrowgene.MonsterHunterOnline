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

    public class CSGuildBanGuilderReq : IStructure
    {

        public CSGuildBanGuilderReq()
        {
            BanOperate = 0;
            DBID = 0;
            RoleID = 0;
            LevelID = 0;
            LineId = 0;
        }

        /// <summary>
        /// 禁言操作，0是禁言1是解除禁言
        /// </summary>
        public int BanOperate;

        /// <summary>
        /// 玩家DBID
        /// </summary>
        public ulong DBID;

        /// <summary>
        /// 玩家NetID
        /// </summary>
        public int RoleID;

        /// <summary>
        /// 玩家当前所在的副本编号/城镇地图编号
        /// </summary>
        public uint LevelID;

        /// <summary>
        /// 玩家当前所在线
        /// </summary>
        public int LineId;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(BanOperate, Endianness.Big);
            buffer.WriteUInt64(DBID, Endianness.Big);
            buffer.WriteInt32(RoleID, Endianness.Big);
            buffer.WriteUInt32(LevelID, Endianness.Big);
            buffer.WriteInt32(LineId, Endianness.Big);
        }

    }
}