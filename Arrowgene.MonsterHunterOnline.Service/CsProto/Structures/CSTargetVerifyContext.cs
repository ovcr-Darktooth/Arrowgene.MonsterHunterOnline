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
    /// 客户端传送到服务器的打击行为验证上下文
    /// </summary>
    public class CSTargetVerifyContext : IStructure
    {

        public CSTargetVerifyContext()
        {
            AttackeeEntityPos = new CSQuatT();
            AttackeeLayers = new List<CSTransitionQueue>();
        }

        /// <summary>
        /// 打击发生时受击者的位置和旋转
        /// </summary>
        public CSQuatT AttackeeEntityPos;

        /// <summary>
        /// 受击者的动画队列组
        /// </summary>
        public List<CSTransitionQueue> AttackeeLayers;

        public void Write(IBuffer buffer)
        {
            AttackeeEntityPos.Write(buffer);
            short attackeeLayersCount = (short)AttackeeLayers.Count;
            buffer.WriteInt16(attackeeLayersCount, Endianness.Big);
            for (int i = 0; i < attackeeLayersCount; i++)
            {
                AttackeeLayers[i].Write(buffer);
            }
        }

    }
}