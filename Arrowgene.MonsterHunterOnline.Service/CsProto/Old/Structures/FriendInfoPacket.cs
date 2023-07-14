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
    /// 好友信息数据
    /// </summary>
    public class FriendInfoPacket : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(FriendInfoPacket));

        public FriendInfoPacket()
        {
            RoleDBID = 0;
            Level = 0;
            RoleName = "";
            GroupID = 0;
            Friendly = 0;
            FarmPoint = 0;
            FarmCanBeGatheredCount = 0;
            HRLevel = 0;
            SvrId = 0;
            AddTime = 0;
        }

        public ulong RoleDBID;

        /// <summary>
        /// 等级
        /// </summary>
        public int Level;

        public string RoleName;

        /// <summary>
        /// 分组ID
        /// </summary>
        public byte GroupID;

        /// <summary>
        /// 友好度
        /// </summary>
        public uint Friendly;

        /// <summary>
        /// 农场点数
        /// </summary>
        public int FarmPoint;

        /// <summary>
        /// 玩家农场可被采集的剩余次数
        /// </summary>
        public int FarmCanBeGatheredCount;

        /// <summary>
        /// HR等级
        /// </summary>
        public int HRLevel;

        /// <summary>
        /// SvrId
        /// </summary>
        public uint SvrId;

        /// <summary>
        /// AddTime
        /// </summary>
        public int AddTime;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt64(RoleDBID, Endianness.Big);
            buffer.WriteInt32(Level, Endianness.Big);
            buffer.WriteInt32(RoleName.Length + 1, Endianness.Big);
            buffer.WriteCString(RoleName);
            buffer.WriteByte(GroupID);
            buffer.WriteUInt32(Friendly, Endianness.Big);
            buffer.WriteInt32(FarmPoint, Endianness.Big);
            buffer.WriteInt32(FarmCanBeGatheredCount, Endianness.Big);
            buffer.WriteInt32(HRLevel, Endianness.Big);
            buffer.WriteUInt32(SvrId, Endianness.Big);
            buffer.WriteInt32(AddTime, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            RoleDBID = buffer.ReadUInt64(Endianness.Big);
            Level = buffer.ReadInt32(Endianness.Big);
            int RoleNameEntryLen = buffer.ReadInt32(Endianness.Big);
            RoleName = buffer.ReadString(RoleNameEntryLen);
            GroupID = buffer.ReadByte();
            Friendly = buffer.ReadUInt32(Endianness.Big);
            FarmPoint = buffer.ReadInt32(Endianness.Big);
            FarmCanBeGatheredCount = buffer.ReadInt32(Endianness.Big);
            HRLevel = buffer.ReadInt32(Endianness.Big);
            SvrId = buffer.ReadUInt32(Endianness.Big);
            AddTime = buffer.ReadInt32(Endianness.Big);
        }

    }
}
