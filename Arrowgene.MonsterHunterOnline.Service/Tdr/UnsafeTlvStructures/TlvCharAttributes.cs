using System;
using Arrowgene.Buffers;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.System.UnlockSystem;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Constant;

namespace Arrowgene.MonsterHunterOnline.Service.Tdr.TlvStructures
{
    /// <summary>
    /// Reconstructed TLV Structure.
    /// C++ Writer: crygame.dll+sub_101C01B0
    /// C++ Reader: crygame.dll+sub_101CB370
    /// C++ Printer: crygame.dll+sub_XXXXX
    /// </summary>
    public class TlvCharAttributes : Structure, ITlvStructure
    {
        // The universal array boundary for character attributes
        public const int MaxArrayElements = 7;
        #region Properties - Integer Arrays
        public int[] CharLevel { get; set; } = new int[MaxArrayElements];
        public int[] CharMaxHP { get; set; } = new int[MaxArrayElements];
        public int[] CharReju { get; set; } = new int[MaxArrayElements];
        public int[] CharMaxReju { get; set; } = new int[MaxArrayElements];
        public int[] CharMaxSta { get; set; } = new int[MaxArrayElements];
        public int[] CharStaDdctPeriod { get; set; } = new int[MaxArrayElements];
        public int[] CharDefence { get; set; } = new int[MaxArrayElements];
        public int[] CritLevel { get; set; } = new int[MaxArrayElements];
        public int[] CritDmg { get; set; } = new int[MaxArrayElements];
        public int[] AntiCritDmg { get; set; } = new int[MaxArrayElements];
        public int[] MaxSharpness { get; set; } = new int[MaxArrayElements];
        public int[] SharpAttackMdf { get; set; } = new int[MaxArrayElements];
        public int[] WaterAttack { get; set; } = new int[MaxArrayElements];
        public int[] FireAttack { get; set; } = new int[MaxArrayElements];
        public int[] LightningAttack { get; set; } = new int[MaxArrayElements];
        public int[] DragonAttack { get; set; } = new int[MaxArrayElements];
        public int[] IceAttack { get; set; } = new int[MaxArrayElements];
        public int[] NonAttack { get; set; } = new int[MaxArrayElements];
        public int[] PoisonAttack { get; set; } = new int[MaxArrayElements];
        public int[] SleepyAttack { get; set; } = new int[MaxArrayElements];
        public int[] ParalysisAttack { get; set; } = new int[MaxArrayElements];
        public int[] WaterThrsh { get; set; } = new int[MaxArrayElements];
        public int[] FireThrsh { get; set; } = new int[MaxArrayElements];
        public int[] LightningThrsh { get; set; } = new int[MaxArrayElements];
        public int[] DragonThrsh { get; set; } = new int[MaxArrayElements];
        public int[] IceThrsh { get; set; } = new int[MaxArrayElements];
        public int[] NonThrsh { get; set; } = new int[MaxArrayElements];
        public int[] PiyoThrsh { get; set; } = new int[MaxArrayElements];
        public int[] PoisonThrsh { get; set; } = new int[MaxArrayElements];
        public int[] SleepingThrsh { get; set; } = new int[MaxArrayElements];
        public int[] ParaThrsh { get; set; } = new int[MaxArrayElements];
        public int[] PiyoDdct { get; set; } = new int[MaxArrayElements];
        public int[] CharSpeed { get; set; } = new int[MaxArrayElements];
        public int[] CharAnimSpeed { get; set; } = new int[MaxArrayElements];
        public int[] CharMaxRage { get; set; } = new int[MaxArrayElements];
        public int[] RejuDmgRatio { get; set; } = new int[MaxArrayElements];
        public int[] RageRatio { get; set; } = new int[MaxArrayElements];
        public int[] QiRenMax { get; set; } = new int[MaxArrayElements];
        public int[] QirenSpeed { get; set; } = new int[MaxArrayElements];
        public int[] LV1QiRenTime { get; set; } = new int[MaxArrayElements];
        public int[] LV2QiRenTime { get; set; } = new int[MaxArrayElements];
        public int[] LV3QiRenTime { get; set; } = new int[MaxArrayElements];
        public int[] QirenMaxTime { get; set; } = new int[MaxArrayElements];
        public int[] AmmoMax { get; set; } = new int[MaxArrayElements];
        public int[] RageSpeed { get; set; } = new int[MaxArrayElements];
        public int[] PerfectReloadAmount { get; set; } = new int[MaxArrayElements];
        public int[] AttackLevelDef { get; set; } = new int[MaxArrayElements];
        public int[] ReloadLevel { get; set; } = new int[MaxArrayElements];
        public int[] RecoilLevel { get; set; } = new int[MaxArrayElements];
        public int[] BulletSpeed { get; set; } = new int[MaxArrayElements];
        public int[] RecoilCameraShake { get; set; } = new int[MaxArrayElements];
        public int[] CharRejuPer { get; set; } = new int[MaxArrayElements];
        public int[] BowStringTenacity { get; set; } = new int[MaxArrayElements];
        public int[] MeleeBottleMax { get; set; } = new int[MaxArrayElements];
        public int[] StrikeBottleMax { get; set; } = new int[MaxArrayElements];
        public int[] PoisonBottleMax { get; set; } = new int[MaxArrayElements];
        public int[] SleepyBottleMax { get; set; } = new int[MaxArrayElements];
        public int[] ParaBottleMax { get; set; } = new int[MaxArrayElements];
        public int[] BowChargeLevelMax { get; set; } = new int[MaxArrayElements];
        public int[] RapidShootAdd { get; set; } = new int[MaxArrayElements];
        public int[] CharDefenseSuperArmorLevel { get; set; } = new int[MaxArrayElements];
        public int[] CharLanceDefenseChargeLv { get; set; } = new int[MaxArrayElements];
        public int[] LuckValule { get; set; } = new int[MaxArrayElements];
        public int[] ZhanJiValue { get; set; } = new int[MaxArrayElements];
        public int[] CharEvadeTime { get; set; } = new int[MaxArrayElements];
        public int[] PvPAttackLevelDef { get; set; } = new int[MaxArrayElements];
        public int[] DynamiteAttack { get; set; } = new int[MaxArrayElements];
        public int[] DynamiteThrsh { get; set; } = new int[MaxArrayElements];
        public int[] DynamiteBottleMax { get; set; } = new int[MaxArrayElements];
        public int[] GhostStaminaDdctSpeed { get; set; } = new int[MaxArrayElements];
        public int[] JinLiStep1MaxValue { get; set; } = new int[MaxArrayElements];
        public int[] JinLiStep2MaxValue { get; set; } = new int[MaxArrayElements];
        public int[] HammerModeTime { get; set; } = new int[MaxArrayElements];
        public int[] AttrAtkFlag { get; set; } = new int[MaxArrayElements];
        public int[] GunLanceMaxAmmoCount { get; set; } = new int[MaxArrayElements];
        #endregion

        #region Properties - Float Arrays
        public float[] CharStaRecovery { get; set; } = new float[MaxArrayElements];
        public float[] CharStaDdct { get; set; } = new float[MaxArrayElements];
        public float[] StaReduce { get; set; } = new float[MaxArrayElements];
        public float[] CurStaReduce { get; set; } = new float[MaxArrayElements];
        public float[] PerfectReloadTime { get; set; } = new float[MaxArrayElements];
        public float[] PlayerAtk { get; set; } = new float[MaxArrayElements];
        public float[] BackBossRunStaReduce { get; set; } = new float[MaxArrayElements];
        public float[] PlayerCrit { get; set; } = new float[MaxArrayElements];
        public float[] GhostMax { get; set; } = new float[MaxArrayElements];
        public float[] GhostDdctSpeed { get; set; } = new float[MaxArrayElements];
        public float[] DefenseReduceHPModifyRate { get; set; } = new float[MaxArrayElements];
        public float[] DefenseReduceStaModifyRate { get; set; } = new float[MaxArrayElements];
        public float[] GunLanceSkillZAngleSpeed { get; set; } = new float[MaxArrayElements];
        public float[] BowBlastSPMax { get; set; } = new float[MaxArrayElements];
        public float[] CurrentBowBlastSpeed { get; set; } = new float[MaxArrayElements];
        public float[] GunLanceMax { get; set; } = new float[MaxArrayElements];
        public float[] GunLanceDdctSpeed { get; set; } = new float[MaxArrayElements];
        #endregion

        #region Properties - Singular Integers
        public int CharSex { get; set; }
        public int CharExp { get; set; }
        public int StarLevel { get; set; }
        public int CharHP { get; set; }
        public int Sharpness { get; set; }
        public int WaterAccum { get; set; }
        public int FireAccum { get; set; }
        public int LightningAccum { get; set; }
        public int DragonAccum { get; set; }
        public int IceAccum { get; set; }
        public int NonAccum { get; set; }
        public int PiyoAccum { get; set; }
        public int PoisonAccum { get; set; }
        public int SleepingAccum { get; set; }
        public int ParaAccum { get; set; }
        public int CharMoney { get; set; }
        public int CharBoundMoney { get; set; }
        public int CharCredit { get; set; }
        public int CharBoundCredit { get; set; }
        public int CharFatigue { get; set; }
        public int CharMaxFatigue { get; set; }
        public int ClaymoreExp { get; set; }
        public int HammerExp { get; set; }
        public int KatanaExp { get; set; }
        public int DuelSwordExp { get; set; }
        public int SwordExp { get; set; }
        public int SpearExp { get; set; }
        public int GunExp { get; set; }
        public int BowExp { get; set; }
        public int CrossbowExp { get; set; }
        public int FluteExp { get; set; }
        public int MaleFace { get; set; }
        public int MaleHair { get; set; }
        public int CharRage { get; set; }
        public int QiRenValue { get; set; }
        public int QiRenLevel { get; set; }
        public int RejuFlag { get; set; }
        public int AmmoCount { get; set; }
        public int AmmoID { get; set; }
        public int NextAmmoID { get; set; }
        public int SubAmmoID { get; set; }
        public int TeamID { get; set; }
        public int PerfAmmoPos { get; set; }
        public int PowerAmmoPos { get; set; }
        public int WindPressureDef { get; set; }
        public int QuakeDef { get; set; }
        public int RoarDef { get; set; }
        public int PalsyDef { get; set; }
        public int SnowManDef { get; set; }
        public int TiredDef { get; set; }
        public int Region { get; set; }
        public int Adult { get; set; }
        public int AASStatus { get; set; }
        public int EquipFoundDay { get; set; }
        public int UnderClothes { get; set; }
        public int StateFlag { get; set; }
        public int PetCarryNum { get; set; }
        public int PetHomeNum { get; set; }
        public int PetOwendNumMax { get; set; }
        public int CharContribution { get; set; }
        public int MeleeBottleUsed { get; set; }
        public int StrikeBottleUsed { get; set; }
        public int PoisonBottleUsed { get; set; }
        public int SleepyBottleUsed { get; set; }
        public int ParaBottleUsed { get; set; }
        public int SecurityStatus { get; set; }
        public int BowShootCount { get; set; }
        public int BowBlastType { get; set; }
        public int CharRemainsExp { get; set; }
        public int FarmExp { get; set; }
        public int FarmEvaluation { get; set; }
        public int SkinColor { get; set; }
        public int HairColor { get; set; }
        public int InnerColor { get; set; }
        public int FaceTattooIndex { get; set; }
        public int EyeBall { get; set; }
        public int FarmFriendGatherCount { get; set; }
        public int FaceTattooColor { get; set; }
        public int EyeColor { get; set; }
        public int BattleState { get; set; }
        public int HammerMode { get; set; }
        public int CharCatCredit { get; set; }
        public int CharReviveCredit { get; set; }
        public int JinLiValue { get; set; }
        public int JinLiStep1ReduceValue { get; set; }
        public int JinLiStep2ReduceValue { get; set; }
        public int EquipTitleID { get; set; }
        public int TitleExp { get; set; }
        public int TitleLevel { get; set; }
        public int EquipTitleBuff { get; set; }
        public SystemUnlockFlags SystemUnlockData { get; set; }
        public int GuildContribution { get; set; }
        public int ExtDailyExp { get; set; }
        public int GuildId { get; set; }
        public int GhostLevel { get; set; }
        public int DistArrowUsed { get; set; }
        public int ExplodeArrowUsed { get; set; }
        public int TiringArrowUsed { get; set; }
        public int CharScore { get; set; }
        public int EntrustMoney1 { get; set; }
        public int EntrustMoney2 { get; set; }
        public int CharmFoundTimes { get; set; }
        public int WeapSysUnlockFlag { get; set; }
        public int CharRemainsDoubleExp { get; set; }
        public int ExtDailyDoubleExp { get; set; }
        public int VIPLevel { get; set; }
        public int VIPExp { get; set; }
        public int VIPBaseEndTime { get; set; }
        public int VIPGrowthEndTime { get; set; }
        public int VIPProfitEndTime { get; set; }
        public int BanChatEndTime { get; set; }
        public SystemUnlockExtFlags SystemUnlockExtData1 { get; set; }
        public int VIPBaseExpireLastNtfTime { get; set; }
        public int VIPVASFrozenTime { get; set; }
        public int ClanScore { get; set; }
        public int ClanScoreMax { get; set; }
        public int ClanMoney { get; set; }
        public int GunLanceAmmoCount { get; set; }
        public int CharHRLevel { get; set; }
        public int CharHRPoint { get; set; }
        public int ClanMoneyPVP { get; set; }
        public int ClanLeaveTime { get; set; }
        public int SanctionPunishEndTime { get; set; }
        public int MVMMonsterFirst { get; set; }
        public int MVMMonsterSecond { get; set; }
        public int MVMMonsterThird { get; set; }
        public int SanctionPunishRatio { get; set; }
        public int CheatSanctionPunishRatio { get; set; }
        public int FluteTune { get; set; }
        public int DeadTime { get; set; }
        public int PersonalELO { get; set; }
        public int PVPMoney { get; set; }
        public int CharCatMoney { get; set; }
        public int CharCatMoneyWeek { get; set; }
        public int CharCatMoneyWeekMax { get; set; }
        public int HuntSoul { get; set; }
        public int WildHuntCamp { get; set; }
        public int WildHuntPhase { get; set; }
        public int BattleCount { get; set; }
        public int FirstLoginTime { get; set; }
        public int LastHuntSoul { get; set; }
        public int TotalHRPoint { get; set; }
        public int LikeHunterOfficer { get; set; }
        public int HuntingCreditExchange { get; set; }
        public int LevelShowType { get; set; }
        public int MonolopyRoundCount { get; set; }
        public int MonolopyActivity { get; set; }
        public int MonolopyCurGrid { get; set; }
        public int ShouHunPoint { get; set; }
        public int LieHunPoint { get; set; }
        public int MVPPoint { get; set; }
        public int XHunterScore { get; set; }
        public int MonolopyAccStep { get; set; }
        public int XHunterHighScore { get; set; }
        public int SoulStoneLevel { get; set; }
        public int WeekMoneyGain { get; set; }
        public int SoulStoneAtkLevel { get; set; }
        public int XHunterActivity { get; set; }
        public int LevelHuntSoul { get; set; }
        public int TaskHuntSoul { get; set; }
        public int CampHuntSoul { get; set; }
        public int PrivateRealHuntSoul { get; set; }
        public int HideWeaponBreakEffect { get; set; }
        public int IllustratePoint { get; set; }
        public int DynamiteAccum { get; set; }
        public int DynamiteDef { get; set; }
        public int DynamiteBottleUsed { get; set; }
        public int GuildBanChatEndTime { get; set; }
        #endregion

        #region Properties - Singular Floats
        public float CharSta { get; set; }
        public float GunLanceValue { get; set; }
        public float LeftGhostValue { get; set; }
        public float RightGhostValue { get; set; }
        public float TotalCredit { get; set; }
        public float BowBlastSpeedVeryFast { get; set; }
        public float BowBlastSpeedFast { get; set; }
        public float BowBlastSpeedNormal { get; set; }
        public float BowBlastSpeedSlow { get; set; }
        public float BowBlastSP { get; set; }
        #endregion

        #region Properties - Singular booleans
        public bool HideFashion { get; set; }
        public bool HideSuite { get; set; }
        public bool HideHelm { get; set; }
        #endregion

        #region Properties - Singular Shorts
        public short Death { get; set; }
        public short TeamPasswordFlag { get; set; }
        public short VIP { get; set; }
        public short VIPVASFrozen { get; set; }
        public short VIPBaseCanUse { get; set; }
        public short VIPGrowthCanUse { get; set; }
        public short VIPProfitCanUse { get; set; }
        public short GameVIP { get; set; }
        public short QQVIP { get; set; }
        public short YearQQVIP { get; set; }
        public short SuperQQVIP { get; set; }
        public short NetbarLevel { get; set; }
        public short XYVIP { get; set; }
        public short TGPVIP { get; set; }
        public short IsNewbie { get; set; }
        public short IsVIPBaseExpireNtf { get; set; }

        public short[] FacialInfo { get; set; } = new short[47]; // Collapsed FacialInfo 1 to 47
        #endregion

        #region Properties - Singular Long
        public long WildHuntGuild { get; set; }
        #endregion

        #region Serialization Methods

        public void ReadTlv(IBuffer buffer)
        {
            throw new NotImplementedException();
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
            WriteTlvInt16(buffer, 20, Death);
            WriteTlvFloat(buffer, 21, CharSta);
            WriteTlvInt32Arr(buffer, 22, CharMaxSta);
            WriteTlvFloatArr(buffer, 23, CharStaRecovery);
            WriteTlvFloatArr(buffer, 24, CharStaDdct);
            WriteTlvInt32Arr(buffer, 25, CharStaDdctPeriod);
            WriteTlvInt32Arr(buffer, 32, CharDefence);
            WriteTlvInt32Arr(buffer, 33, CritLevel);
            WriteTlvInt32Arr(buffer, 34, CritDmg);
            WriteTlvInt32Arr(buffer, 35, AntiCritDmg);
            WriteTlvInt32(buffer, 36, Sharpness);
            WriteTlvInt32Arr(buffer, 37, MaxSharpness);
            WriteTlvInt32Arr(buffer, 38, SharpAttackMdf);
            WriteTlvInt32Arr(buffer, 39, WaterAttack);
            WriteTlvInt32Arr(buffer, 40, FireAttack);
            WriteTlvInt32Arr(buffer, 41, LightningAttack);
            WriteTlvInt32Arr(buffer, 42, DragonAttack);
            WriteTlvInt32Arr(buffer, 43, IceAttack);
            WriteTlvInt32Arr(buffer, 44, NonAttack);
            WriteTlvInt32Arr(buffer, 45, PoisonAttack);
            WriteTlvInt32Arr(buffer, 46, SleepyAttack);
            WriteTlvInt32Arr(buffer, 47, ParalysisAttack);
            WriteTlvInt32(buffer, 53, WaterAccum);
            WriteTlvInt32(buffer, 54, FireAccum);
            WriteTlvInt32(buffer, 55, LightningAccum);
            WriteTlvInt32(buffer, 56, DragonAccum);
            WriteTlvInt32(buffer, 57, IceAccum);
            WriteTlvInt32(buffer, 58, NonAccum);
            WriteTlvInt32Arr(buffer, 59, WaterThrsh);
            WriteTlvInt32Arr(buffer, 60, FireThrsh);
            WriteTlvInt32Arr(buffer, 61, LightningThrsh);
            WriteTlvInt32Arr(buffer, 62, DragonThrsh);
            WriteTlvInt32Arr(buffer, 63, IceThrsh);
            WriteTlvInt32Arr(buffer, 64, NonThrsh);
            WriteTlvInt32Arr(buffer, 65, PiyoThrsh);
            WriteTlvInt32Arr(buffer, 66, PoisonThrsh);
            WriteTlvInt32Arr(buffer, 67, SleepingThrsh);
            WriteTlvInt32Arr(buffer, 68, ParaThrsh);
            WriteTlvInt32Arr(buffer, 69, PiyoDdct);
            WriteTlvInt32(buffer, 70, PiyoAccum);
            WriteTlvInt32(buffer, 71, PoisonAccum);
            WriteTlvInt32(buffer, 72, SleepingAccum);
            WriteTlvInt32(buffer, 73, ParaAccum);
            WriteTlvInt32Arr(buffer, 74, CharSpeed);
            WriteTlvInt32Arr(buffer, 75, CharAnimSpeed);
            WriteTlvInt32(buffer, 76, CharMoney);
            WriteTlvInt32(buffer, 77, CharBoundMoney);
            WriteTlvInt32(buffer, 78, CharCredit);
            WriteTlvInt32(buffer, 79, CharBoundCredit);
            WriteTlvInt32(buffer, 86, CharFatigue);
            WriteTlvInt32(buffer, 87, CharMaxFatigue);
            WriteTlvInt32(buffer, 90, ClaymoreExp);
            WriteTlvInt32(buffer, 91, HammerExp);
            WriteTlvInt32(buffer, 92, KatanaExp);
            WriteTlvInt32(buffer, 93, DuelSwordExp);
            WriteTlvInt32(buffer, 94, SwordExp);
            WriteTlvInt32(buffer, 95, SpearExp);
            WriteTlvInt32(buffer, 96, GunExp);
            WriteTlvInt32(buffer, 97, BowExp);
            WriteTlvInt32(buffer, 98, CrossbowExp);
            WriteTlvInt32(buffer, 99, FluteExp);
            WriteTlvInt32(buffer, 108, MaleFace);
            WriteTlvInt32(buffer, 109, MaleHair);
            WriteTlvInt32(buffer, 110, CharRage);
            WriteTlvInt32Arr(buffer, 111, CharMaxRage);
            WriteTlvInt32Arr(buffer, 112, RejuDmgRatio);
            WriteTlvInt32Arr(buffer, 113, RageRatio);
            WriteTlvInt32(buffer, 114, QiRenValue);
            WriteTlvInt32Arr(buffer, 115, QiRenMax);
            WriteTlvInt32(buffer, 116, QiRenLevel);
            WriteTlvInt32Arr(buffer, 117, QirenSpeed);
            WriteTlvInt32Arr(buffer, 118, LV1QiRenTime);
            WriteTlvInt32Arr(buffer, 119, LV2QiRenTime);
            WriteTlvInt32Arr(buffer, 120, LV3QiRenTime);
            WriteTlvInt32Arr(buffer, 121, QirenMaxTime);
            WriteTlvInt32(buffer, 122, RejuFlag);
            WriteTlvInt32Arr(buffer, 123, AmmoMax);
            WriteTlvInt32(buffer, 124, AmmoCount);
            WriteTlvInt32(buffer, 125, AmmoID);
            WriteTlvInt32(buffer, 126, NextAmmoID);
            WriteTlvInt32(buffer, 127, SubAmmoID);
            WriteTlvInt32(buffer, 128, TeamID);
            WriteTlvFloatArr(buffer, 130, StaReduce);
            WriteTlvInt32Arr(buffer, 132, RageSpeed);
            WriteTlvFloatArr(buffer, 138, CurStaReduce);
            WriteTlvInt32(buffer, 139, PerfAmmoPos);
            WriteTlvInt32(buffer, 140, PowerAmmoPos);
            WriteTlvFloatArr(buffer, 141, PerfectReloadTime);
            WriteTlvInt32Arr(buffer, 142, PerfectReloadAmount);
            WriteTlvInt32(buffer, 143, WindPressureDef);
            WriteTlvInt32(buffer, 144, QuakeDef);
            WriteTlvInt32(buffer, 145, RoarDef);
            WriteTlvInt32(buffer, 146, PalsyDef);
            WriteTlvInt32(buffer, 147, SnowManDef);
            WriteTlvInt32(buffer, 148, TiredDef);
            WriteTlvInt32Arr(buffer, 149, AttackLevelDef);
            WriteTlvInt32(buffer, 150, Region);
            WriteTlvInt32(buffer, 151, Adult);
            WriteTlvInt32(buffer, 152, AASStatus);
            WriteTlvInt32(buffer, 153, EquipFoundDay);
            WriteTlvInt32Arr(buffer, 154, ReloadLevel);
            WriteTlvInt32Arr(buffer, 155, RecoilLevel);
            WriteTlvInt32Arr(buffer, 156, BulletSpeed);
            WriteTlvInt32Arr(buffer, 157, RecoilCameraShake);
            WriteTlvInt32(buffer, 173, UnderClothes);
            WriteTlvInt32(buffer, 175, StateFlag);
            WriteTlvInt32Arr(buffer, 176, CharRejuPer);
            WriteTlvInt32(buffer, 177, PetCarryNum);
            WriteTlvInt32(buffer, 178, PetHomeNum);
            WriteTlvInt32(buffer, 179, PetOwendNumMax);
            WriteTlvInt32(buffer, 180, CharContribution);
            WriteTlvInt32Arr(buffer, 181, BowStringTenacity);
            WriteTlvInt32Arr(buffer, 182, MeleeBottleMax);
            WriteTlvInt32Arr(buffer, 183, StrikeBottleMax);
            WriteTlvInt32Arr(buffer, 184, PoisonBottleMax);
            WriteTlvInt32Arr(buffer, 185, SleepyBottleMax);
            WriteTlvInt32Arr(buffer, 186, ParaBottleMax);
            WriteTlvInt32(buffer, 187, MeleeBottleUsed);
            WriteTlvInt32(buffer, 188, StrikeBottleUsed);
            WriteTlvInt32(buffer, 189, PoisonBottleUsed);
            WriteTlvInt32(buffer, 190, SleepyBottleUsed);
            WriteTlvInt32(buffer, 191, ParaBottleUsed);
            WriteTlvInt32(buffer, 195, SecurityStatus);
            WriteTlvInt32(buffer, 196, BowShootCount);
            WriteTlvInt32(buffer, 199, BowBlastType);
            WriteTlvInt32(buffer, 200, CharRemainsExp);
            WriteTlvInt32(buffer, 202, FarmExp);
            WriteTlvInt32(buffer, 203, FarmEvaluation);
            WriteTlvInt32(buffer, 205, SkinColor);
            WriteTlvInt32(buffer, 206, HairColor);
            WriteTlvInt32(buffer, 207, InnerColor);
            WriteTlvInt32(buffer, 208, FaceTattooIndex);
            WriteTlvInt32(buffer, 209, EyeBall);
            WriteTlvFloatArr(buffer, 211, PlayerAtk);
            WriteTlvInt32(buffer, 212, FarmFriendGatherCount);
            WriteTlvFloatArr(buffer, 213, BackBossRunStaReduce);
            WriteTlvFloatArr(buffer, 214, PlayerCrit);
            WriteTlvInt32(buffer, 220, FaceTattooColor);
            WriteTlvInt32(buffer, 221, EyeColor);
            WriteTlvInt32Arr(buffer, 222, AttrAtkFlag);
            WriteTlvInt32(buffer, 224, BattleState);
            WriteTlvInt32(buffer, 225, HammerMode);
            WriteTlvInt32Arr(buffer, 226, HammerModeTime);
            WriteTlvInt16(buffer, 227, (short)(HideFashion ? 1 : 0));
            WriteTlvInt16(buffer, 228, (short)(HideSuite ? 1 : 0));
            WriteTlvInt16(buffer, 229, (short)(HideHelm ? 1 : 0));
            WriteTlvInt32(buffer, 230, CharCatCredit);
            WriteTlvInt32(buffer, 231, CharReviveCredit);
            WriteTlvInt32(buffer, 232, JinLiValue);
            WriteTlvInt32Arr(buffer, 233, JinLiStep1MaxValue);
            WriteTlvInt32Arr(buffer, 234, JinLiStep2MaxValue);
            WriteTlvInt32(buffer, 235, JinLiStep1ReduceValue);
            WriteTlvInt32(buffer, 236, JinLiStep2ReduceValue);
            WriteTlvInt32(buffer, 237, EquipTitleID);
            WriteTlvInt32(buffer, 238, TitleExp);
            WriteTlvInt32(buffer, 239, TitleLevel);
            WriteTlvInt32(buffer, 240, EquipTitleBuff);
            WriteTlvInt32(buffer, 241, (int)SystemUnlockData);
            WriteTlvInt32(buffer, 242, GuildContribution);
            WriteTlvInt32(buffer, 243, ExtDailyExp);
            WriteTlvInt32(buffer, 244, GuildId);
            WriteTlvInt16(buffer, 245, TeamPasswordFlag);
            WriteTlvFloat(buffer, 246, LeftGhostValue);
            WriteTlvFloat(buffer, 247, RightGhostValue);
            WriteTlvFloatArr(buffer, 248, GhostMax);
            WriteTlvInt32(buffer, 249, GhostLevel);
            WriteTlvFloatArr(buffer, 250, GhostDdctSpeed);
            WriteTlvInt32Arr(buffer, 251, GhostStaminaDdctSpeed);

            for (int i = 0; i < 25; i++)
                WriteTlvInt16(buffer, 252 + i, FacialInfo[i]);

            WriteTlvInt32(buffer, 280, DistArrowUsed);
            WriteTlvInt32(buffer, 281, ExplodeArrowUsed);
            WriteTlvInt32(buffer, 282, TiringArrowUsed);
            WriteTlvInt32(buffer, 283, CharScore);
            WriteTlvInt16(buffer, 284, VIP);
            WriteTlvInt32(buffer, 287, EntrustMoney1);
            WriteTlvInt32(buffer, 288, EntrustMoney2);
            WriteTlvInt32(buffer, 289, CharmFoundTimes);
            WriteTlvInt32(buffer, 290, WeapSysUnlockFlag);
            WriteTlvInt32(buffer, 291, CharRemainsDoubleExp);
            WriteTlvInt32(buffer, 292, ExtDailyDoubleExp);
            WriteTlvInt32(buffer, 293, VIPLevel);
            WriteTlvInt32(buffer, 294, VIPExp);
            WriteTlvInt32(buffer, 295, VIPBaseEndTime);
            WriteTlvInt32(buffer, 296, VIPGrowthEndTime);
            WriteTlvInt32(buffer, 297, VIPProfitEndTime);
            WriteTlvInt32(buffer, 298, BanChatEndTime);
            WriteTlvInt16(buffer, 299, VIPVASFrozen);
            WriteTlvInt16(buffer, 300, VIPBaseCanUse);
            WriteTlvInt16(buffer, 301, VIPGrowthCanUse);
            WriteTlvInt16(buffer, 302, VIPProfitCanUse);
            WriteTlvInt32(buffer, 303, (int)SystemUnlockExtData1);
            WriteTlvInt32Arr(buffer, 304, BowChargeLevelMax);
            WriteTlvInt16(buffer, 305, IsVIPBaseExpireNtf);
            WriteTlvInt32(buffer, 306, VIPBaseExpireLastNtfTime);
            WriteTlvInt16(buffer, 307, GameVIP);
            WriteTlvInt16(buffer, 308, QQVIP);
            WriteTlvInt16(buffer, 309, YearQQVIP);
            WriteTlvInt16(buffer, 310, SuperQQVIP);
            WriteTlvInt16(buffer, 311, NetbarLevel);
            WriteTlvInt32(buffer, 312, VIPVASFrozenTime);
            WriteTlvInt32(buffer, 313, ClanScore);
            WriteTlvInt32(buffer, 314, ClanScoreMax);
            WriteTlvInt32(buffer, 315, ClanMoney);
            WriteTlvFloat(buffer, 316, GunLanceValue);
            WriteTlvFloatArr(buffer, 317, GunLanceMax);
            WriteTlvFloatArr(buffer, 318, GunLanceDdctSpeed);
            WriteTlvInt32(buffer, 319, GunLanceAmmoCount);
            WriteTlvInt32Arr(buffer, 320, GunLanceMaxAmmoCount);
            WriteTlvInt32Arr(buffer, 321, RapidShootAdd);
            WriteTlvInt32(buffer, 322, CharHRLevel);
            WriteTlvInt32(buffer, 323, CharHRPoint);
            WriteTlvInt32(buffer, 324, ClanMoneyPVP);
            WriteTlvInt32Arr(buffer, 325, CharDefenseSuperArmorLevel);
            WriteTlvInt32(buffer, 326, ClanLeaveTime);
            WriteTlvInt32Arr(buffer, 327, CharLanceDefenseChargeLv);
            WriteTlvInt16(buffer, 328, XYVIP);

            for (int i = 0; i < 22; i++)
                WriteTlvInt16(buffer, 329 + i, FacialInfo[25 + i]);

            WriteTlvInt32(buffer, 351, SanctionPunishEndTime);
            WriteTlvInt16(buffer, 352, TGPVIP);
            WriteTlvInt32(buffer, 353, MVMMonsterFirst);
            WriteTlvInt32(buffer, 354, MVMMonsterSecond);
            WriteTlvInt32(buffer, 355, MVMMonsterThird);
            WriteTlvInt32Arr(buffer, 356, LuckValule);
            WriteTlvInt32(buffer, 357, SanctionPunishRatio);
            WriteTlvInt32(buffer, 358, CheatSanctionPunishRatio);
            WriteTlvInt32(buffer, 359, FluteTune);
            WriteTlvFloatArr(buffer, 360, DefenseReduceHPModifyRate);
            WriteTlvFloatArr(buffer, 361, DefenseReduceStaModifyRate);
            WriteTlvFloatArr(buffer, 362, GunLanceSkillZAngleSpeed);
            WriteTlvInt32(buffer, 363, DeadTime);
            WriteTlvInt32(buffer, 364, PersonalELO);
            WriteTlvInt32(buffer, 365, PVPMoney);
            WriteTlvInt32(buffer, 366, CharCatMoney);
            WriteTlvInt32(buffer, 367, CharCatMoneyWeek);
            WriteTlvInt32(buffer, 368, CharCatMoneyWeekMax);
            WriteTlvInt32(buffer, 369, HuntSoul);
            WriteTlvInt32(buffer, 370, WildHuntCamp);
            WriteTlvInt32(buffer, 371, WildHuntPhase);
            WriteTlvInt64(buffer, 372, WildHuntGuild);
            WriteTlvFloat(buffer, 373, TotalCredit);
            WriteTlvInt32(buffer, 374, BattleCount);
            WriteTlvInt32(buffer, 375, FirstLoginTime);
            WriteTlvInt32(buffer, 376, LastHuntSoul);
            WriteTlvInt32(buffer, 377, TotalHRPoint);
            WriteTlvInt32(buffer, 378, LikeHunterOfficer);
            WriteTlvInt32(buffer, 381, HuntingCreditExchange);
            WriteTlvInt32Arr(buffer, 382, PvPAttackLevelDef);
            WriteTlvInt32(buffer, 383, LevelShowType);
            WriteTlvInt32Arr(buffer, 384, CharEvadeTime);
            WriteTlvInt32(buffer, 385, MonolopyRoundCount);
            WriteTlvInt32(buffer, 386, MonolopyActivity);
            WriteTlvInt32(buffer, 387, MonolopyCurGrid);
            WriteTlvInt32Arr(buffer, 388, ZhanJiValue);
            WriteTlvInt32(buffer, 389, ShouHunPoint);
            WriteTlvInt32(buffer, 390, LieHunPoint);
            WriteTlvInt32(buffer, 391, MVPPoint);
            WriteTlvInt32(buffer, 392, XHunterScore);
            WriteTlvFloat(buffer, 393, BowBlastSpeedVeryFast);
            WriteTlvFloat(buffer, 394, BowBlastSpeedFast);
            WriteTlvFloat(buffer, 395, BowBlastSpeedNormal);
            WriteTlvFloat(buffer, 396, BowBlastSpeedSlow);
            WriteTlvFloat(buffer, 397, BowBlastSP);
            WriteTlvFloatArr(buffer, 398, BowBlastSPMax);
            WriteTlvFloatArr(buffer, 399, CurrentBowBlastSpeed);
            WriteTlvInt32(buffer, 400, MonolopyAccStep);
            WriteTlvInt32(buffer, 401, XHunterHighScore);
            WriteTlvInt32(buffer, 402, SoulStoneLevel);
            WriteTlvInt32(buffer, 404, WeekMoneyGain);
            WriteTlvInt32(buffer, 405, SoulStoneAtkLevel);
            WriteTlvInt32(buffer, 406, XHunterActivity);
            WriteTlvInt16(buffer, 407, IsNewbie);
            WriteTlvInt32(buffer, 408, LevelHuntSoul);
            WriteTlvInt32(buffer, 409, TaskHuntSoul);
            WriteTlvInt32(buffer, 410, CampHuntSoul);
            WriteTlvInt32(buffer, 411, PrivateRealHuntSoul);
            WriteTlvInt32(buffer, 412, HideWeaponBreakEffect);
            WriteTlvInt32(buffer, 413, IllustratePoint);
            WriteTlvInt32Arr(buffer, 414, DynamiteAttack);
            WriteTlvInt32Arr(buffer, 415, DynamiteThrsh);
            WriteTlvInt32(buffer, 416, DynamiteAccum);
            WriteTlvInt32(buffer, 417, DynamiteDef);
            WriteTlvInt32Arr(buffer, 418, DynamiteBottleMax);
            WriteTlvInt32(buffer, 419, DynamiteBottleUsed);
            WriteTlvInt32(buffer, 420, GuildBanChatEndTime);
        }
        #endregion


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

        public void SetCharMaxHP(int val)
        {
            SetProp(CharMaxHP, val, 1 , 1);
        }

        public void SetCharReju(int val)
        {
            SetProp(CharReju, val, 1, 2);
        }

        public void SetCharMaxReju(int val)
        {
            SetProp(CharMaxReju, val, 1, 3);
        }

        public void SetCharMaxSta(int val)
        {
            SetProp(CharMaxSta, val);
        }

        private void SetProp(int[] prop, int val, int debug = 0, int type = 0)
        {
            if (debug == 0)
            {
                for (int i = 0; i < prop.Length; i++)
                {
                    prop[i] = val;
                }
            }
            else
            {
                if (type == 1) // max hp (length of the hp bar)
                {
                    prop[0] = 0;
                    prop[1] = 0;
                    prop[2] = 0;
                    prop[3] = 0;
                    prop[4] = 0;
                    prop[5] = 0;
                    prop[6] = 90;
                } else if (type == 2) // reju (maybe it is the hp regen per tick, did not see any changes playing with this)
                {
                    prop[0] = 0;
                    prop[1] = 0;
                    prop[2] = 0;
                    prop[3] = 0;
                    prop[4] = 0;
                    prop[5] = 0;
                    prop[6] = 50;
                }
                else if (type == 3) // max reju (red bar, needs to be in between hp and maxhp)
                {
                    prop[0] = 0;
                    prop[1] = 0;
                    prop[2] = 0;
                    prop[3] = 0;
                    prop[4] = 0;
                    prop[5] = 0;
                    prop[6] = 90;
                }
            }
        }
    }
}
