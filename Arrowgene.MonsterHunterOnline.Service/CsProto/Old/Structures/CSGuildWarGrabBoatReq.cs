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

    public class CSGuildWarGrabBoatReq : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSGuildWarGrabBoatReq));

        public CSGuildWarGrabBoatReq()
        {
            PlayerIds = new List<uint>();
            TargetId = 0;
        }

        /// <summary>
        /// 玩家NETID
        /// </summary>
        public List<uint> PlayerIds;

        /// <summary>
        /// 目标ID
        /// </summary>
        public ulong TargetId;

        public void Write(IBuffer buffer)
        {
            ushort playerIdsCount = (ushort)PlayerIds.Count;
            buffer.WriteUInt16(playerIdsCount, Endianness.Big);
            for (int i = 0; i < playerIdsCount; i++)
            {
                buffer.WriteUInt32(PlayerIds[i], Endianness.Big);
            }
            buffer.WriteUInt64(TargetId, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            PlayerIds.Clear();
            ushort playerIdsCount = buffer.ReadUInt16(Endianness.Big);
            for (int i = 0; i < playerIdsCount; i++)
            {
                uint PlayerIdsEntry = buffer.ReadUInt32(Endianness.Big);
                PlayerIds.Add(PlayerIdsEntry);
            }
            TargetId = buffer.ReadUInt64(Endianness.Big);
        }

    }
}
