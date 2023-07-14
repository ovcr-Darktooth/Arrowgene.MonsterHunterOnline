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
    /// 动画播放信息
    /// </summary>
    public class CSParametric : IStructure
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(CSParametric));

        public CSParametric()
        {
            ParametricCRC = 0;
            AnimCRC = new List<uint>();
            SegmentCounter = new List<short>();
            BlendWeight = new List<float>();
        }

        /// <summary>
        /// 参数系统ID的CRC
        /// </summary>
        public uint ParametricCRC;

        /// <summary>
        /// 组成LMG的动画CRC
        /// </summary>
        public List<uint> AnimCRC;

        /// <summary>
        /// 组成LMG的动画所含片段数目
        /// </summary>
        public List<short> SegmentCounter;

        /// <summary>
        /// 组成LMG动画的Blending权值
        /// </summary>
        public List<float> BlendWeight;

        public void Write(IBuffer buffer)
        {
            buffer.WriteUInt32(ParametricCRC, Endianness.Big);
            short animCRCCount = (short)AnimCRC.Count;
            buffer.WriteInt16(animCRCCount, Endianness.Big);
            for (int i = 0; i < animCRCCount; i++)
            {
                buffer.WriteUInt32(AnimCRC[i], Endianness.Big);
            }
            short segmentCounterCount = (short)SegmentCounter.Count;
            buffer.WriteInt16(segmentCounterCount, Endianness.Big);
            for (int i = 0; i < segmentCounterCount; i++)
            {
                buffer.WriteInt16(SegmentCounter[i], Endianness.Big);
            }
            short blendWeightCount = (short)BlendWeight.Count;
            buffer.WriteInt16(blendWeightCount, Endianness.Big);
            for (int i = 0; i < blendWeightCount; i++)
            {
                buffer.WriteFloat(BlendWeight[i], Endianness.Big);
            }
        }

        public void Read(IBuffer buffer)
        {
            ParametricCRC = buffer.ReadUInt32(Endianness.Big);
            AnimCRC.Clear();
            short animCRCCount = buffer.ReadInt16(Endianness.Big);
            for (int i = 0; i < animCRCCount; i++)
            {
                uint AnimCRCEntry = buffer.ReadUInt32(Endianness.Big);
                AnimCRC.Add(AnimCRCEntry);
            }
            SegmentCounter.Clear();
            short segmentCounterCount = buffer.ReadInt16(Endianness.Big);
            for (int i = 0; i < segmentCounterCount; i++)
            {
                short SegmentCounterEntry = buffer.ReadInt16(Endianness.Big);
                SegmentCounter.Add(SegmentCounterEntry);
            }
            BlendWeight.Clear();
            short blendWeightCount = buffer.ReadInt16(Endianness.Big);
            for (int i = 0; i < blendWeightCount; i++)
            {
                float BlendWeightEntry = buffer.ReadFloat(Endianness.Big);
                BlendWeight.Add(BlendWeightEntry);
            }
        }

    }
}
