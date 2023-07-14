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
    /// 检查版本响应
    /// </summary>
    public class CSCheckVersionRsp : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSCheckVersionRsp));

        public CSCheckVersionRsp()
        {
            ErrNo = 0;
            MajorVerNo = 0;
            MinorVerNo = 0;
            RevisVerNo = 0;
            BuildVerNo = 0;
            Feature = new CSFeatureInfo();
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrNo;

        /// <summary>
        /// 服务器主版本号，测试时候用，正式版本可考虑取消，避免客户端作弊
        /// </summary>
        public int MajorVerNo;

        /// <summary>
        /// 服务器子版本号，测试时候用，正式版本可考虑取消，避免客户端作弊
        /// </summary>
        public int MinorVerNo;

        /// <summary>
        /// 服务器修正版本号，测试时候用，正式版本可考虑取消，避免客户端作弊
        /// </summary>
        public int RevisVerNo;

        /// <summary>
        /// 服务器编译版本号，测试时候用，正式版本可考虑取消，避免客户端作弊
        /// </summary>
        public int BuildVerNo;

        /// <summary>
        /// Feature
        /// </summary>
        public CSFeatureInfo Feature;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(ErrNo, Endianness.Big);
            buffer.WriteInt32(MajorVerNo, Endianness.Big);
            buffer.WriteInt32(MinorVerNo, Endianness.Big);
            buffer.WriteInt32(RevisVerNo, Endianness.Big);
            buffer.WriteInt32(BuildVerNo, Endianness.Big);
            Feature.Write(buffer);
        }

        public void Read(IBuffer buffer)
        {
            ErrNo = buffer.ReadInt32(Endianness.Big);
            MajorVerNo = buffer.ReadInt32(Endianness.Big);
            MinorVerNo = buffer.ReadInt32(Endianness.Big);
            RevisVerNo = buffer.ReadInt32(Endianness.Big);
            BuildVerNo = buffer.ReadInt32(Endianness.Big);
            Feature.Read(buffer);
        }

    }
}