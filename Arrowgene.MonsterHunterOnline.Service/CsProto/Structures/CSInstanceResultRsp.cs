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
    /// 副本结算通知
    /// </summary>
    public class CSInstanceResultRsp : IStructure
    {

        public CSInstanceResultRsp()
        {
            LevelID = 0;
            GameMode = 0;
            HuntingMode = 0;
            FakeItemInfo = new FakeItemMap();
            InstanceStatResult = new InstanceResultStatInfo();
            FactionStatResult = new List<FactionResultStatInfo>();
            SelfResult = new CSPlayerResultInfo();
            OtherResultList = new List<CSOtherResultInfo>();
        }

        /// <summary>
        /// 关卡ID
        /// </summary>
        public int LevelID;

        /// <summary>
        /// PVPorPVE
        /// </summary>
        public int GameMode;

        /// <summary>
        /// 玩家当局狩猎模式
        /// </summary>
        public int HuntingMode;

        /// <summary>
        /// 当局狩猎临时道具映射关系
        /// </summary>
        public FakeItemMap FakeItemInfo;

        /// <summary>
        /// 副本结算统计
        /// </summary>
        public InstanceResultStatInfo InstanceStatResult;

        /// <summary>
        /// 阵营统计数据
        /// </summary>
        public List<FactionResultStatInfo> FactionStatResult;

        /// <summary>
        /// 玩家结算信息
        /// </summary>
        public CSPlayerResultInfo SelfResult;

        /// <summary>
        /// 玩家关卡结算信息
        /// </summary>
        public List<CSOtherResultInfo> OtherResultList;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt32(LevelID, Endianness.Big);
            buffer.WriteInt32(GameMode, Endianness.Big);
            buffer.WriteInt32(HuntingMode, Endianness.Big);
            FakeItemInfo.Write(buffer);
            InstanceStatResult.Write(buffer);
            int factionStatResultCount = (int)FactionStatResult.Count;
            buffer.WriteInt32(factionStatResultCount, Endianness.Big);
            for (int i = 0; i < factionStatResultCount; i++)
            {
                FactionStatResult[i].Write(buffer);
            }
            SelfResult.Write(buffer);
            byte otherResultListCount = (byte)OtherResultList.Count;
            buffer.WriteByte(otherResultListCount);
            for (int i = 0; i < otherResultListCount; i++)
            {
                OtherResultList[i].Write(buffer);
            }
        }

    }
}