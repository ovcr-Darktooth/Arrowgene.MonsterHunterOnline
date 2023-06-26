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
    /// 方案装备
    /// </summary>
    public class EquipPlanItem : IStructure
    {

        public EquipPlanItem()
        {
            ItemId = 0;
            ItemType = 0;
            TargetPos = 0;
            curCol = 0;
            curGrid = 0;
            skillBeadsInfo = new SkillBeads[3];
            for (int i = 0; i < skillBeadsInfo.Length; i++)
            {
                skillBeadsInfo[i] = new SkillBeads();
            }
        }

        /// <summary>
        /// 装备唯一ID
        /// </summary>
        public ulong ItemId;

        /// <summary>
        /// 装备类型ID
        /// </summary>
        public int ItemType;

        /// <summary>
        /// 装备目标位置
        /// </summary>
        public byte TargetPos;

        /// <summary>
        /// 当前位置栏位
        /// </summary>
        public byte curCol;

        /// <summary>
        /// 当前格子index
        /// </summary>
        public int curGrid;

        /// <summary>
        /// 装备孔技能珠信息
        /// </summary>
        public SkillBeads[] skillBeadsInfo;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt64(ItemId, Endianness.Big);
            buffer.WriteInt32(ItemType, Endianness.Big);
            buffer.WriteByte(TargetPos);
            buffer.WriteByte(curCol);
            buffer.WriteInt32(curGrid, Endianness.Big);
            for (int i = 0; i < skillBeadsInfo.Length; i++)
            {
                skillBeadsInfo[i].Write(buffer);
            }
        }

    }
}