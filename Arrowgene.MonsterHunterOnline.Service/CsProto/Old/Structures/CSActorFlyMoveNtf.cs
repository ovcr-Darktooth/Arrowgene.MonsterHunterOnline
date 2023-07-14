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
    /// 开始滑翔消息
    /// </summary>
    public class CSActorFlyMoveNtf : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSActorFlyMoveNtf));

        public CSActorFlyMoveNtf()
        {
            NetObjId = 0;
            SyncTime = 0;
            Location = new CSVec3();
            Rotation = new CSQuat();
            MoveSpeed = new CSVec3();
            AngleSpeed = new CSVec3();
            InputWorldDir = new CSVec3();
        }

        /// <summary>
        /// 需要同步的Actor的NetObjId
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
        /// 朝向
        /// </summary>
        public CSQuat Rotation;

        /// <summary>
        /// 移动速度
        /// </summary>
        public CSVec3 MoveSpeed;

        /// <summary>
        /// 移动速度
        /// </summary>
        public CSVec3 AngleSpeed;

        /// <summary>
        /// 世界坐标系下的位置
        /// </summary>
        public CSVec3 InputWorldDir;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(NetObjId, Endianness.Big);
            buffer.WriteInt64(SyncTime, Endianness.Big);
            Location.Write(buffer);
            Rotation.Write(buffer);
            MoveSpeed.Write(buffer);
            AngleSpeed.Write(buffer);
            InputWorldDir.Write(buffer);
        }

        public void Read(IBuffer buffer)
        {
            NetObjId = buffer.ReadUInt32(Endianness.Big);
            SyncTime = buffer.ReadInt64(Endianness.Big);
            Location.Read(buffer);
            Rotation.Read(buffer);
            MoveSpeed.Read(buffer);
            AngleSpeed.Read(buffer);
            InputWorldDir.Read(buffer);
        }

    }
}