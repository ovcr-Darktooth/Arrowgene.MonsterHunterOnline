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
    /// 怪物AI技能同步消息
    /// </summary>
    public class CSAIAnimSequence : IStructure
    {

        public CSAIAnimSequence()
        {
            SyncTime = 0;
            MonsterID = 0;
            AnimSeqName = "";
            SkillID = 0;
            SkillLv = 0;
            SyncFlag = 0;
            TargetSrvID = 0;
            MoveSpeed = new CSVec3();
            RotSpeed = new CSVec3();
            MonsterPos = new CSVec3();
            MonsterRot = new CSQuat();
            SkillSpeed = 0.0f;
            RestartAnim = 0;
            TargetMultiAttackPos = new List<CSVec3>();
            TargetAttackPos = new CSVec3();
            NeedTargetAttackPos = 0;
            AckFlag = 0;
            SetRotate = 0;
            SetPos = 0;
            NoTransferCorrection = 0;
            NeedMoveSpeedAcc = 0;
            MoveSpeedAccelerate = new CSVec3();
            MoveSpeedAccStart = 0.0f;
            MoveSpeedAccEnd = 0.0f;
            MoveSplineScale = new CSVec3();
        }

        /// <summary>
        /// 同步时间
        /// </summary>
        public long SyncTime;

        /// <summary>
        /// 怪物ID
        /// </summary>
        public uint MonsterID;

        /// <summary>
        /// 技能name
        /// </summary>
        public string AnimSeqName;

        /// <summary>
        /// 技能id
        /// </summary>
        public int SkillID;

        /// <summary>
        /// 技能Lv
        /// </summary>
        public int SkillLv;

        /// <summary>
        /// 消息Flag
        /// </summary>
        public uint SyncFlag;

        /// <summary>
        /// 目标SrvID
        /// </summary>
        public int TargetSrvID;

        /// <summary>
        /// 移动速度
        /// </summary>
        public CSVec3 MoveSpeed;

        /// <summary>
        /// 旋转速度
        /// </summary>
        public CSVec3 RotSpeed;

        /// <summary>
        /// 怪物位置
        /// </summary>
        public CSVec3 MonsterPos;

        /// <summary>
        /// 怪物朝向
        /// </summary>
        public CSQuat MonsterRot;

        /// <summary>
        /// 技能速度
        /// </summary>
        public float SkillSpeed;

        /// <summary>
        /// 重新播放技能
        /// </summary>
        public byte RestartAnim;

        /// <summary>
        /// 攻击目标多个位置
        /// </summary>
        public List<CSVec3> TargetMultiAttackPos;

        /// <summary>
        /// 目标攻击坐标
        /// </summary>
        public CSVec3 TargetAttackPos;

        /// <summary>
        /// 需要追踪目标
        /// </summary>
        public byte NeedTargetAttackPos;

        /// <summary>
        /// Ack Flag
        /// </summary>
        public uint AckFlag;

        /// <summary>
        /// 重新设置朝向
        /// </summary>
        public byte SetRotate;

        /// <summary>
        /// 重新设置位置
        /// </summary>
        public byte SetPos;

        /// <summary>
        /// 是否需要Transfer修正
        /// </summary>
        public byte NoTransferCorrection;

        /// <summary>
        /// 是否需要移动加速
        /// </summary>
        public byte NeedMoveSpeedAcc;

        /// <summary>
        /// 移动加速度
        /// </summary>
        public CSVec3 MoveSpeedAccelerate;

        /// <summary>
        /// 移动加速度开始时间
        /// </summary>
        public float MoveSpeedAccStart;

        /// <summary>
        /// 移动加速度结束时间
        /// </summary>
        public float MoveSpeedAccEnd;

        /// <summary>
        /// 运动曲线缩放
        /// </summary>
        public CSVec3 MoveSplineScale;

        public void Write(IBuffer buffer)
        {
            buffer.WriteInt64(SyncTime, Endianness.Big);
            buffer.WriteUInt32(MonsterID, Endianness.Big);
            buffer.WriteInt32(AnimSeqName.Length + 1, Endianness.Big);
            buffer.WriteCString(AnimSeqName);
            buffer.WriteInt32(SkillID, Endianness.Big);
            buffer.WriteInt32(SkillLv, Endianness.Big);
            buffer.WriteUInt32(SyncFlag, Endianness.Big);
            buffer.WriteInt32(TargetSrvID, Endianness.Big);
            MoveSpeed.Write(buffer);
            RotSpeed.Write(buffer);
            MonsterPos.Write(buffer);
            MonsterRot.Write(buffer);
            buffer.WriteFloat(SkillSpeed, Endianness.Big);
            buffer.WriteByte(RestartAnim);
            int targetMultiAttackPosCount = (int)TargetMultiAttackPos.Count;
            buffer.WriteInt32(targetMultiAttackPosCount, Endianness.Big);
            for (int i = 0; i < targetMultiAttackPosCount; i++)
            {
                TargetMultiAttackPos[i].Write(buffer);
            }
            TargetAttackPos.Write(buffer);
            buffer.WriteByte(NeedTargetAttackPos);
            buffer.WriteUInt32(AckFlag, Endianness.Big);
            buffer.WriteByte(SetRotate);
            buffer.WriteByte(SetPos);
            buffer.WriteByte(NoTransferCorrection);
            buffer.WriteByte(NeedMoveSpeedAcc);
            MoveSpeedAccelerate.Write(buffer);
            buffer.WriteFloat(MoveSpeedAccStart, Endianness.Big);
            buffer.WriteFloat(MoveSpeedAccEnd, Endianness.Big);
            MoveSplineScale.Write(buffer);
        }

    }
}