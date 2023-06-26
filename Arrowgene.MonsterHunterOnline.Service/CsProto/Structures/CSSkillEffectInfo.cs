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

    /// <summary>
    /// 技能效果同步信息
    /// </summary>
    public class CSSkillEffectInfo : IStructure
    {

        public CSSkillEffectInfo()
        {
            EntityId = 0;
            SkillID = 0;
            SkillLevel = 0;
            Type = 0;
            EventName = "";
        }

        /// <summary>
        /// Entity ID
        /// </summary>
        public uint EntityId;

        /// <summary>
        /// 技能ID
        /// </summary>
        public int SkillID;

        /// <summary>
        /// 技能等级
        /// </summary>
        public int SkillLevel;

        /// <summary>
        /// 技能效果类型
        /// </summary>
        public int Type;

        /// <summary>
        /// 技能效果名
        /// </summary>
        public string EventName;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(EntityId, Endianness.Big);
            buffer.WriteInt32(SkillID, Endianness.Big);
            buffer.WriteInt32(SkillLevel, Endianness.Big);
            buffer.WriteInt32(Type, Endianness.Big);
            buffer.WriteInt32(EventName.Length + 1, Endianness.Big);
            buffer.WriteCString(EventName);
        }

    }
}