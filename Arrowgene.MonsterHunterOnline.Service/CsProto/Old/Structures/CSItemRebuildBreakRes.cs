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
    /// 请求装备突破
    /// </summary>
    public class CSItemRebuildBreakRes : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSItemRebuildBreakRes));

        public CSItemRebuildBreakRes()
        {
            ErrCode = 0;
            SuccessFlag = 0;
            EquipID = 0;
            Column = 0;
            Grid = 0;
            UseDiamondCnt = 0;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode;

        /// <summary>
        /// 突破是否成功
        /// </summary>
        public byte SuccessFlag;

        /// <summary>
        /// 物品实例
        /// </summary>
        public ulong EquipID;

        /// <summary>
        /// 栏位置
        /// </summary>
        public byte Column;

        /// <summary>
        /// 格子位
        /// </summary>
        public short Grid;

        /// <summary>
        /// 使用宝石
        /// </summary>
        public short UseDiamondCnt;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(ErrCode, Endianness.Big);
            buffer.WriteByte(SuccessFlag);
            buffer.WriteUInt64(EquipID, Endianness.Big);
            buffer.WriteByte(Column);
            buffer.WriteInt16(Grid, Endianness.Big);
            buffer.WriteInt16(UseDiamondCnt, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            ErrCode = buffer.ReadInt32(Endianness.Big);
            SuccessFlag = buffer.ReadByte();
            EquipID = buffer.ReadUInt64(Endianness.Big);
            Column = buffer.ReadByte();
            Grid = buffer.ReadInt16(Endianness.Big);
            UseDiamondCnt = buffer.ReadInt16(Endianness.Big);
        }

    }
}
