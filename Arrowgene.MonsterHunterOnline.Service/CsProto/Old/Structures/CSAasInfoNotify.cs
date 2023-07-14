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
    /// 防沉迷信息通知
    /// </summary>
    public class CSAasInfoNotify : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSAasInfoNotify));

        public CSAasInfoNotify()
        {
            AdultType = 0;
            OnlineTime = 0;
        }

        /// <summary>
        /// 是否成年标志
        /// </summary>
        public int AdultType;

        /// <summary>
        /// 在线时间
        /// </summary>
        public int OnlineTime;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(AdultType, Endianness.Big);
            buffer.WriteInt32(OnlineTime, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            AdultType = buffer.ReadInt32(Endianness.Big);
            OnlineTime = buffer.ReadInt32(Endianness.Big);
        }

    }
}
