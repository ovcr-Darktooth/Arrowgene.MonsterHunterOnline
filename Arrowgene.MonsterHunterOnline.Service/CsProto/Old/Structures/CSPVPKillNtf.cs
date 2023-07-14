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
    /// pvp mode kill
    /// </summary>
    public class CSPVPKillNtf : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSPVPKillNtf));

        public CSPVPKillNtf()
        {
            KillerID = 0;
            DeadID = 0;
            Reserved = 0;
        }

        public uint KillerID;

        public uint DeadID;

        public int Reserved;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(KillerID, Endianness.Big);
            buffer.WriteUInt32(DeadID, Endianness.Big);
            buffer.WriteInt32(Reserved, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            KillerID = buffer.ReadUInt32(Endianness.Big);
            DeadID = buffer.ReadUInt32(Endianness.Big);
            Reserved = buffer.ReadInt32(Endianness.Big);
        }

    }
}