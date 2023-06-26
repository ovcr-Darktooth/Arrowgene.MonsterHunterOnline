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
    /// 邮件附件信息
    /// </summary>
    public class CSAccessory : IStructure
    {

        public CSAccessory(CsAccessoryDataUnion _Accessory)
        {
            isGet = 0;
            Accessory = _Accessory;
        }

        /// <summary>
        /// 附件是否被提取标记
        /// </summary>
        public byte isGet;

        /// <summary>
        /// 类型 @EMailAccessoryType
        /// </summary>

        /// <summary>
        /// 附件
        /// </summary>
        public CsAccessoryDataUnion Accessory;

        public void Write(IBuffer buffer)
        {
            buffer.WriteByte(isGet);
            buffer.WriteUInt32((uint)Accessory.Type, Endianness.Big);
            Accessory.Write(buffer);
        }

    }
}