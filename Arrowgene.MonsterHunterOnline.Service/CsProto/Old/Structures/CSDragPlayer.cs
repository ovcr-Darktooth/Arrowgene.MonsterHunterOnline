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
    /// 拉玩家
    /// </summary>
    public class CSDragPlayer : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSDragPlayer));

        public CSDragPlayer()
        {
            NetObjId = 0;
            SyncTime = 0;
            Location = new CSVec3();
            ActorRot = new CSQuat();
        }

        /// <summary>
        /// Actor的NetObjId
        /// </summary>
        public uint NetObjId;

        /// <summary>
        /// 同步时间
        /// </summary>
        public long SyncTime;

        /// <summary>
        /// 世界坐标系下的位置
        /// </summary>
        public CSVec3 Location;

        /// <summary>
        /// Actor朝向
        /// </summary>
        public CSQuat ActorRot;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(NetObjId, Endianness.Big);
            buffer.WriteInt64(SyncTime, Endianness.Big);
            Location.Write(buffer);
            ActorRot.Write(buffer);
        }

        public void Read(IBuffer buffer)
        {
            NetObjId = buffer.ReadUInt32(Endianness.Big);
            SyncTime = buffer.ReadInt64(Endianness.Big);
            Location.Read(buffer);
            ActorRot.Read(buffer);
        }

    }
}
