using System;
using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System.UnlockSystem;

namespace Arrowgene.MonsterHunterOnline.Service.Tdr.TlvStructures;

public class TlvAttr : Structure, ITlvStructure
{
    public TlvAttr()
    {
        CharLevel = new int[7];
        CharMaxSta = new int[7];
        CharStaRecovery = new int[7];
        CharStaDdct = new int[7];
        CharStaDdctPeriod = new int[7];
        CharSpeed = new int[7];
        CharAnimSpeed = new int[7];
        CharFatigue = new int[7];
        CharMaxFatigue = new int[7];
        CharMaxHP = new int[7];
        CharReju = new int[7];
        CharMaxReju = new int[7];
        StaReduce = new int[7];
        CurStaReduce = new int[7];
        
        FacialInfo = new short[CsProtoConstant.CS_MAX_FACIALINFO_COUNT];
    }

    public int[] CharLevel { get; }
    public int CharSex { get; set; }
    public int CharExp { get; set; }
    public int StarLevel { get; set; }
    public int CharHP { get; set; }
    public int[] CharMaxHP { get; set; }
    public int[] CharReju { get; set; }
    public int[] CharMaxReju { get; set; }
    public int Death { get; set; }
    public int CharSta { get; set; }
    public int[] CharMaxSta { get; set; }
    public int[] CharStaRecovery { get; }
    public int[] CharStaDdct { get; }
    public int[] CharStaDdctPeriod { get; }
    public int StaFlag { get; set; }
    public int[] StaReduce { get; }
    public int StaReduceFlag { get; set; }
    public int[] CurStaReduce { get; }
    public int[] CharSpeed { get; }
    public int[] CharAnimSpeed { get; }
    public int[] CharFatigue { get; }
    public int[] CharMaxFatigue { get; }
    public int MaleFace { get; set; }
    public int MaleHair { get; set; }
    public int UnderClothes { get; set; }
    public int SkinColor { get; set; }
    public int HairColor { get; set; }
    public int InnerColor { get; set; }
    public int FaceTattooId { get; set; }
    public int EyeBall { get; set; }
    public int FaceTattooColor { get; set; }
    public int EyeColor { get; set; }
    public bool HideFashion { get; set; }
    public bool HideSuite { get; set; }
    public bool HideHelm { get; set; }
    public SystemUnlockFlags SystemUnlockData { get; set; }
    public SystemUnlockExtFlags SystemUnlockExtData1 { get; set; }
    public short[] FacialInfo { get; }
    public int CharHRLevel { get; set; }
    public int CharHRPoint { get; set; }

    public void SetFacialInfo(short[] facialInfo)
    {
        for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
        {
            FacialInfo[i] = facialInfo[i];
        }
    }

    public void SetCharLevel(int val)
    {
        SetProp(CharLevel, val);
    }

    public void SetCharSpeed(int val)
    {
        SetProp(CharSpeed, val);
    }
    public void SetCharAnimSpeed(int val)
    {
        SetProp(CharAnimSpeed, val);
    }

    public void SetCharFatigue(int val)
    {
        SetProp(CharFatigue, val);
    }

    public void SetCharMaxFatigue(int val)
    {
        SetProp(CharMaxFatigue, val);
    }

    public void SetCharMaxHP(int val)
    {
        SetProp(CharMaxHP, val);
    }
    public void SetCharReju(int val)
    {
        SetProp(CharReju, val);
    }
    public void SetCharMaxReju(int val)
    {
        SetProp(CharMaxReju, val);
    }

    public void SetCharMaxSta(int val)
    {
        SetProp(CharMaxSta, val);
    }

    public void SetCharStaRecovery(int val)
    {
        SetProp(CharStaRecovery, val);
    }

    public void SetCharStaDdct(int val)
    {
        SetProp(CharStaDdct, val);
    }
    public void SetCharStaDdctPeriod(int val)
    {
        SetProp(CharStaDdctPeriod, val);
    }

    public void SetStaReduce(int val)
    {
        SetProp(StaReduce, val);
    }
    public void SetCurStaReduce(int val)
    {
        SetProp(CurStaReduce, val);
    }


    public void WriteTlv(IBuffer buffer)
    {
        WriteTlvInt32Arr(buffer, 2, CharLevel);
        WriteTlvInt32(buffer, 4, CharSex);
        WriteTlvInt32(buffer, 6, CharExp);
        WriteTlvInt32(buffer, 7, StarLevel);
        WriteTlvInt32(buffer, 16, CharHP);
        WriteTlvInt32Arr(buffer, 17, CharMaxHP);
        WriteTlvInt32Arr(buffer, 18, CharReju);
        WriteTlvInt32Arr(buffer, 19, CharMaxReju);
        WriteTlvInt32(buffer, 20, Death);
        WriteTlvInt32(buffer, 21, CharSta);
        WriteTlvInt32Arr(buffer, 22, CharMaxSta);
        WriteTlvInt32Arr(buffer, 23, CharStaRecovery);
        WriteTlvInt32Arr(buffer, 24, CharStaDdct);
        WriteTlvInt32Arr(buffer, 25, CharStaDdctPeriod);
        WriteTlvInt32Arr(buffer, 74, CharSpeed);
        WriteTlvInt32Arr(buffer, 75, CharAnimSpeed);
        WriteTlvInt32Arr(buffer, 86, CharFatigue);
        WriteTlvInt32Arr(buffer, 87, CharMaxFatigue);
        WriteTlvInt32(buffer, 108, MaleFace);
        WriteTlvInt32(buffer, 109, MaleHair);
        WriteTlvInt32(buffer, 129, StaFlag);
        WriteTlvInt32Arr(buffer, 130, StaReduce);
        WriteTlvInt32(buffer, 131, StaReduceFlag);
        WriteTlvInt32Arr(buffer, 138, CurStaReduce);
        WriteTlvInt32(buffer, 173, UnderClothes);
        WriteTlvInt32(buffer, 205, SkinColor);
        WriteTlvInt32(buffer, 206, HairColor);
        WriteTlvInt32(buffer, 207, InnerColor);
        WriteTlvInt32(buffer, 208, FaceTattooId);
        WriteTlvInt32(buffer, 209, EyeBall);
        WriteTlvInt32(buffer, 220, FaceTattooColor);
        WriteTlvInt32(buffer, 221, EyeColor);
        WriteTlvInt16(buffer, 227, HideFashion ? (short)1 : (short)0);
        WriteTlvInt16(buffer, 228, HideSuite ? (short)1 : (short)0);
        WriteTlvInt16(buffer, 229, HideHelm ? (short)1 : (short)0);
        WriteTlvInt32(buffer, 241, SystemUnlockData.ToInt32());
        WriteTlvInt32(buffer, 303, SystemUnlockExtData1.ToInt32());
        int faceAttrId = 252;
        for (int i = 0; i < CsProtoConstant.CS_MAX_FACIALINFO_COUNT; i++)
        {
            WriteTlvInt16(buffer, faceAttrId, FacialInfo[i]);
            faceAttrId++;
            //There is a jump between the 2 FacialInfo parts
            if (faceAttrId == 277)
            {
                faceAttrId = 329;
            }
        }

        WriteTlvInt32(buffer, 322, CharHRLevel);
        WriteTlvInt32(buffer, 323, CharHRPoint);


    }


    public void ReadTlv(IBuffer buffer)
    {
        throw new NotImplementedException();
    }

    private void SetProp(int[] prop, int val)
    {
        for (int i = 0; i < prop.Length; i++)
        {
            prop[i] = val;
        }
    }
}