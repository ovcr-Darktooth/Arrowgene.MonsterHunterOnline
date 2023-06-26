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
    /// 宠物添加同步
    /// </summary>
    public class CSPetAddSync : IStructure
    {

        public CSPetAddSync()
        {
            PetID = 0;
            Idx = 0;
            UID = 0;
            sex = 0;
            giftSkill = 0;
            RandAttrs = new CSPetRandAttrs();
        }

        /// <summary>
        /// 宠物ID
        /// </summary>
        public int PetID;

        /// <summary>
        /// pet index
        /// </summary>
        public int Idx;

        /// <summary>
        /// pet UID
        /// </summary>
        public int UID;

        /// <summary>
        /// pet sex
        /// </summary>
        public byte sex;

        /// <summary>
        /// 天生技能
        /// </summary>
        public int giftSkill;

        /// <summary>
        /// 随机属性
        /// </summary>
        public CSPetRandAttrs RandAttrs;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(PetID, Endianness.Big);
            buffer.WriteInt32(Idx, Endianness.Big);
            buffer.WriteInt32(UID, Endianness.Big);
            buffer.WriteByte(sex);
            buffer.WriteInt32(giftSkill, Endianness.Big);
            RandAttrs.Write(buffer);
        }

    }
}