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
    /// 队员定义
    /// </summary>
    public class TeamMemberInfo : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(TeamMemberInfo));

        public TeamMemberInfo()
        {
            NetId = 0;
            DBId = 0;
            Name = "";
            Level = 0;
            LevelId = 0;
            LevelExtra = 0;
            Sex = 0;
            WeaponType = 0;
            Star = 0;
            CanBeKicked = 0;
            Online = 0;
            Vec3 = new CSVec3();
            Dir = new CSVec3();
            LineId = 0;
            HRLevel = 0;
            HunterStar = "";
            Zone = 0;
            WeaponTitle = 0;
        }

        /// <summary>
        /// 队员logicEntityId
        /// </summary>
        public uint NetId;

        /// <summary>
        /// DBID
        /// </summary>
        public ulong DBId;

        /// <summary>
        /// 队员姓名
        /// </summary>
        public string Name;

        /// <summary>
        /// 等级
        /// </summary>
        public uint Level;

        /// <summary>
        /// 地图LevelID
        /// </summary>
        public uint LevelId;

        /// <summary>
        /// 地图附加信息
        /// </summary>
        public ulong LevelExtra;

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex;

        /// <summary>
        /// 装备的武器类型
        /// </summary>
        public int WeaponType;

        /// <summary>
        /// 星级
        /// </summary>
        public uint Star;

        /// <summary>
        /// 是否可以被踢
        /// </summary>
        public int CanBeKicked;

        /// <summary>
        /// 是否在线
        /// </summary>
        public int Online;

        /// <summary>
        /// 位置
        /// </summary>
        public CSVec3 Vec3;

        /// <summary>
        /// 方向
        /// </summary>
        public CSVec3 Dir;

        /// <summary>
        /// 队员所在线
        /// </summary>
        public int LineId;

        /// <summary>
        /// HRLevel
        /// </summary>
        public int HRLevel;

        /// <summary>
        /// 猎人星级
        /// </summary>
        public string HunterStar;

        /// <summary>
        /// 服
        /// </summary>
        public uint Zone;

        /// <summary>
        /// WeaponTitle
        /// </summary>
        public int WeaponTitle;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(NetId, Endianness.Big);
            buffer.WriteUInt64(DBId, Endianness.Big);
            buffer.WriteInt32(Name.Length + 1, Endianness.Big);
            buffer.WriteCString(Name);
            buffer.WriteUInt32(Level, Endianness.Big);
            buffer.WriteUInt32(LevelId, Endianness.Big);
            buffer.WriteUInt64(LevelExtra, Endianness.Big);
            buffer.WriteInt32(Sex, Endianness.Big);
            buffer.WriteInt32(WeaponType, Endianness.Big);
            buffer.WriteUInt32(Star, Endianness.Big);
            buffer.WriteInt32(CanBeKicked, Endianness.Big);
            buffer.WriteInt32(Online, Endianness.Big);
            Vec3.Write(buffer);
            Dir.Write(buffer);
            buffer.WriteInt32(LineId, Endianness.Big);
            buffer.WriteInt32(HRLevel, Endianness.Big);
            buffer.WriteInt32(HunterStar.Length + 1, Endianness.Big);
            buffer.WriteCString(HunterStar);
            buffer.WriteUInt32(Zone, Endianness.Big);
            buffer.WriteInt32(WeaponTitle, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            NetId = buffer.ReadUInt32(Endianness.Big);
            DBId = buffer.ReadUInt64(Endianness.Big);
            int NameEntryLen = buffer.ReadInt32(Endianness.Big);
            Name = buffer.ReadString(NameEntryLen);
            Level = buffer.ReadUInt32(Endianness.Big);
            LevelId = buffer.ReadUInt32(Endianness.Big);
            LevelExtra = buffer.ReadUInt64(Endianness.Big);
            Sex = buffer.ReadInt32(Endianness.Big);
            WeaponType = buffer.ReadInt32(Endianness.Big);
            Star = buffer.ReadUInt32(Endianness.Big);
            CanBeKicked = buffer.ReadInt32(Endianness.Big);
            Online = buffer.ReadInt32(Endianness.Big);
            Vec3.Read(buffer);
            Dir.Read(buffer);
            LineId = buffer.ReadInt32(Endianness.Big);
            HRLevel = buffer.ReadInt32(Endianness.Big);
            int HunterStarEntryLen = buffer.ReadInt32(Endianness.Big);
            HunterStar = buffer.ReadString(HunterStarEntryLen);
            Zone = buffer.ReadUInt32(Endianness.Big);
            WeaponTitle = buffer.ReadInt32(Endianness.Big);
        }

    }
}