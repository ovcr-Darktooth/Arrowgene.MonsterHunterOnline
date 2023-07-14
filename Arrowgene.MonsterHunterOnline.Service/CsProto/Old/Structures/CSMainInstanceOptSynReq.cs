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
    /// 副本主UI操作同步请求
    /// </summary>
    public class CSMainInstanceOptSynReq : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSMainInstanceOptSynReq));

        public CSMainInstanceOptSynReq()
        {
            TriggerId = 0;
            InstancePoint = 0;
            LevelID = 0;
            LevelDiff = 0;
            LevelMode = 0;
        }

        /// <summary>
        /// 触发点
        /// </summary>
        public int TriggerId;

        /// <summary>
        /// 副本UI点
        /// </summary>
        public int InstancePoint;

        /// <summary>
        /// levelId
        /// </summary>
        public int LevelID;

        /// <summary>
        /// 难度
        /// </summary>
        public int LevelDiff;

        /// <summary>
        /// 模式
        /// </summary>
        public int LevelMode;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(TriggerId, Endianness.Big);
            buffer.WriteInt32(InstancePoint, Endianness.Big);
            buffer.WriteInt32(LevelID, Endianness.Big);
            buffer.WriteInt32(LevelDiff, Endianness.Big);
            buffer.WriteInt32(LevelMode, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            TriggerId = buffer.ReadInt32(Endianness.Big);
            InstancePoint = buffer.ReadInt32(Endianness.Big);
            LevelID = buffer.ReadInt32(Endianness.Big);
            LevelDiff = buffer.ReadInt32(Endianness.Big);
            LevelMode = buffer.ReadInt32(Endianness.Big);
        }

    }
}