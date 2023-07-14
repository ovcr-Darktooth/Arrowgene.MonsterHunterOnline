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
    /// 攻击的玩家信息
    /// </summary>
    public class CSGrabInfo : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSGrabInfo));

        public CSGrabInfo()
        {
            PlayerId = new List<ulong>();
            OwnGuildId = 0;
            MinTime = 0;
        }

        /// <summary>
        /// 攻击的玩家
        /// </summary>
        public List<ulong> PlayerId;

        /// <summary>
        /// 占领的猎团id
        /// </summary>
        public ulong OwnGuildId;

        /// <summary>
        /// 最短时间
        /// </summary>
        public uint MinTime;

        public void Write(IBuffer buffer)
        {
            uint playerIdCount = (uint)PlayerId.Count;
            buffer.WriteUInt32(playerIdCount, Endianness.Big);
            for (int i = 0; i < playerIdCount; i++)
            {
                buffer.WriteUInt64(PlayerId[i], Endianness.Big);
            }
            buffer.WriteUInt64(OwnGuildId, Endianness.Big);
            buffer.WriteUInt32(MinTime, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            PlayerId.Clear();
            uint playerIdCount = buffer.ReadUInt32(Endianness.Big);
            for (int i = 0; i < playerIdCount; i++)
            {
                ulong PlayerIdEntry = buffer.ReadUInt64(Endianness.Big);
                PlayerId.Add(PlayerIdEntry);
            }
            OwnGuildId = buffer.ReadUInt64(Endianness.Big);
            MinTime = buffer.ReadUInt32(Endianness.Big);
        }

    }
}
