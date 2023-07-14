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

    public class CSMailNotify : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSMailNotify));

        public CSMailNotify()
        {
            MailType = 0;
            MailNotifyType = 0;
            MailCount = 0;
            MailListEntry = new CSMailListEntry();
            uin = 0;
        }

        /// <summary>
        /// 0:玩家邮件;1:系统发送的邮件 2:礼物箱;3:索取箱; 0xff:所有类型的邮件
        /// </summary>
        public byte MailType;

        /// <summary>
        /// 0:玩家有新邮件;1:系统退信由于邮箱满被删除 2:系统退信需要玩家接收;3:邮件由于过期被删除
        /// </summary>
        public byte MailNotifyType;

        /// <summary>
        /// 邮件总数
        /// </summary>
        public uint MailCount;

        /// <summary>
        /// 邮件详细信息
        /// </summary>
        public CSMailListEntry MailListEntry;

        public uint uin;

        public void Write(IBuffer buffer)
        {
            buffer.WriteByte(MailType);
            buffer.WriteByte(MailNotifyType);
            buffer.WriteUInt32(MailCount, Endianness.Big);
            MailListEntry.Write(buffer);
            buffer.WriteUInt32(uin, Endianness.Big);
        }

        public void Read(IBuffer buffer)
        {
            MailType = buffer.ReadByte();
            MailNotifyType = buffer.ReadByte();
            MailCount = buffer.ReadUInt32(Endianness.Big);
            MailListEntry.Read(buffer);
            uin = buffer.ReadUInt32(Endianness.Big);
        }

    }
}
