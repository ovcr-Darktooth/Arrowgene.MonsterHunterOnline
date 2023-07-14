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
    /// 玩家购买限制数据
    /// </summary>
    public class CSPlayerBuyLimitData : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSPlayerBuyLimitData));

        public CSPlayerBuyLimitData()
        {
            DayBuyLimitData = new List<CSBuyItemLimitData>();
            WeekBuyLimitData = new List<CSBuyItemLimitData>();
            MonthBuyLimitData = new List<CSBuyItemLimitData>();
            ForeverBuyLimitData = new List<CSBuyItemLimitData>();
        }

        /// <summary>
        /// 日数据
        /// </summary>
        public List<CSBuyItemLimitData> DayBuyLimitData;

        /// <summary>
        /// 周数据
        /// </summary>
        public List<CSBuyItemLimitData> WeekBuyLimitData;

        /// <summary>
        /// 月数据
        /// </summary>
        public List<CSBuyItemLimitData> MonthBuyLimitData;

        /// <summary>
        /// 永久数据
        /// </summary>
        public List<CSBuyItemLimitData> ForeverBuyLimitData;

        public void Write(IBuffer buffer)
        {
            ushort dayBuyLimitDataCount = (ushort)DayBuyLimitData.Count;
            buffer.WriteUInt16(dayBuyLimitDataCount, Endianness.Big);
            for (int i = 0; i < dayBuyLimitDataCount; i++)
            {
                DayBuyLimitData[i].Write(buffer);
            }
            ushort weekBuyLimitDataCount = (ushort)WeekBuyLimitData.Count;
            buffer.WriteUInt16(weekBuyLimitDataCount, Endianness.Big);
            for (int i = 0; i < weekBuyLimitDataCount; i++)
            {
                WeekBuyLimitData[i].Write(buffer);
            }
            ushort monthBuyLimitDataCount = (ushort)MonthBuyLimitData.Count;
            buffer.WriteUInt16(monthBuyLimitDataCount, Endianness.Big);
            for (int i = 0; i < monthBuyLimitDataCount; i++)
            {
                MonthBuyLimitData[i].Write(buffer);
            }
            ushort foreverBuyLimitDataCount = (ushort)ForeverBuyLimitData.Count;
            buffer.WriteUInt16(foreverBuyLimitDataCount, Endianness.Big);
            for (int i = 0; i < foreverBuyLimitDataCount; i++)
            {
                ForeverBuyLimitData[i].Write(buffer);
            }
        }

        public void Read(IBuffer buffer)
        {
            DayBuyLimitData.Clear();
            ushort dayBuyLimitDataCount = buffer.ReadUInt16(Endianness.Big);
            for (int i = 0; i < dayBuyLimitDataCount; i++)
            {
                CSBuyItemLimitData DayBuyLimitDataEntry = new CSBuyItemLimitData();
                DayBuyLimitDataEntry.Read(buffer);
                DayBuyLimitData.Add(DayBuyLimitDataEntry);
            }
            WeekBuyLimitData.Clear();
            ushort weekBuyLimitDataCount = buffer.ReadUInt16(Endianness.Big);
            for (int i = 0; i < weekBuyLimitDataCount; i++)
            {
                CSBuyItemLimitData WeekBuyLimitDataEntry = new CSBuyItemLimitData();
                WeekBuyLimitDataEntry.Read(buffer);
                WeekBuyLimitData.Add(WeekBuyLimitDataEntry);
            }
            MonthBuyLimitData.Clear();
            ushort monthBuyLimitDataCount = buffer.ReadUInt16(Endianness.Big);
            for (int i = 0; i < monthBuyLimitDataCount; i++)
            {
                CSBuyItemLimitData MonthBuyLimitDataEntry = new CSBuyItemLimitData();
                MonthBuyLimitDataEntry.Read(buffer);
                MonthBuyLimitData.Add(MonthBuyLimitDataEntry);
            }
            ForeverBuyLimitData.Clear();
            ushort foreverBuyLimitDataCount = buffer.ReadUInt16(Endianness.Big);
            for (int i = 0; i < foreverBuyLimitDataCount; i++)
            {
                CSBuyItemLimitData ForeverBuyLimitDataEntry = new CSBuyItemLimitData();
                ForeverBuyLimitDataEntry.Read(buffer);
                ForeverBuyLimitData.Add(ForeverBuyLimitDataEntry);
            }
        }

    }
}
