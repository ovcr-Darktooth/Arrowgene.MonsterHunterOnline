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
    /// 协议包跟踪
    /// </summary>
    public class CSPkgTimerRecord : IStructure
    {

        public CSPkgTimerRecord()
        {
            Timer1 = 0;
            Timer2 = 0;
            Timer3 = 0;
            Timer4 = 0;
            Timer5 = 0;
            Timer6 = 0;
            Timer7 = 0;
            Timer8 = 0;
            Timer9 = 0;
            Timer10 = 0;
        }

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer1;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer2;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer3;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer4;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer5;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer6;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer7;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer8;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer9;

        /// <summary>
        /// 时间记录1
        /// </summary>
        public long Timer10;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt64(Timer1, Endianness.Big);
            buffer.WriteInt64(Timer2, Endianness.Big);
            buffer.WriteInt64(Timer3, Endianness.Big);
            buffer.WriteInt64(Timer4, Endianness.Big);
            buffer.WriteInt64(Timer5, Endianness.Big);
            buffer.WriteInt64(Timer6, Endianness.Big);
            buffer.WriteInt64(Timer7, Endianness.Big);
            buffer.WriteInt64(Timer8, Endianness.Big);
            buffer.WriteInt64(Timer9, Endianness.Big);
            buffer.WriteInt64(Timer10, Endianness.Big);
        }

    }
}