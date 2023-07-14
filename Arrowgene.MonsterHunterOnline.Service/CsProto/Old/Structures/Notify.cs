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
    /// 通知
    /// </summary>
    public class Notify : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(Notify));

        public Notify()
        {
            _Notify = 0;
            Arg1 = 0;
            Arg2 = 0;
            Arg3 = 0;
            Arg4 = 0;
            Arg5 = 0;
            Arg6 = 0;
        }

        /// <summary>
        /// 通知
        /// </summary>
        public int _Notify;

        /// <summary>
        /// 参数1
        /// </summary>
        public int Arg1;

        /// <summary>
        /// 参数2
        /// </summary>
        public int Arg2;

        /// <summary>
        /// 参数3
        /// </summary>
        public int Arg3;

        /// <summary>
        /// 参数4
        /// </summary>
        public int Arg4;

        /// <summary>
        /// 参数5
        /// </summary>
        public long Arg5;

        /// <summary>
        /// 参数6
        /// </summary>
        public ulong Arg6;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(_Notify, Endianness.Big);
            buffer.WriteInt32(Arg1, Endianness.Big);
            buffer.WriteInt32(Arg2, Endianness.Big);
            buffer.WriteInt32(Arg3, Endianness.Big);
            buffer.WriteInt32(Arg4, Endianness.Big);
            buffer.WriteInt64(Arg5, Endianness.Big);
            buffer.WriteUInt64(Arg6, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            _Notify = buffer.ReadInt32(Endianness.Big);
            Arg1 = buffer.ReadInt32(Endianness.Big);
            Arg2 = buffer.ReadInt32(Endianness.Big);
            Arg3 = buffer.ReadInt32(Endianness.Big);
            Arg4 = buffer.ReadInt32(Endianness.Big);
            Arg5 = buffer.ReadInt64(Endianness.Big);
            Arg6 = buffer.ReadUInt64(Endianness.Big);
        }

    }
}
