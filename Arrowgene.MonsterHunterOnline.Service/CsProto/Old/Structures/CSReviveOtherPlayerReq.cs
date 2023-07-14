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
    /// 复活其他玩家的请求
    /// </summary>
    public class CSReviveOtherPlayerReq : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSReviveOtherPlayerReq));

        public CSReviveOtherPlayerReq()
        {
            PlayerID = 0;
            TargetID = 0;
            ReviveType = 0;
        }

        /// <summary>
        /// 申请复活其他玩家的ID
        /// </summary>
        public int PlayerID;

        /// <summary>
        /// 目标
        /// </summary>
        public int TargetID;

        /// <summary>
        /// 复活方式，1猫车 2猫车券 3复活币
        /// </summary>
        public int ReviveType;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(PlayerID, Endianness.Big);
            buffer.WriteInt32(TargetID, Endianness.Big);
            buffer.WriteInt32(ReviveType, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            PlayerID = buffer.ReadInt32(Endianness.Big);
            TargetID = buffer.ReadInt32(Endianness.Big);
            ReviveType = buffer.ReadInt32(Endianness.Big);
        }

    }
}
