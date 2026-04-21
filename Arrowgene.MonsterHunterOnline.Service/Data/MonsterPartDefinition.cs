using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class MonsterPartDefinition
    {
        public int MonsterId { get; set; }
        public string PartId { get; set; }     // English key, e.g. "Head", "Body", "LeftHand"
        public string PartName { get; set; }   // Localized name (Chinese in source CSV)
        public string StateId { get; set; }    // "Normal", "Rage", etc.

        public float Unbalance { get; set; }
        public float UnbalanceMulti { get; set; }
        public float Scar { get; set; }
        public float Fall { get; set; }
        public float FallMulti { get; set; }

        public float WaterAcc { get; set; }
        public float WaterAccMulti { get; set; }
        public float FireAcc { get; set; }
        public float FireAccMulti { get; set; }
        public float ElectricAcc { get; set; }
        public float ElectricAccMulti { get; set; }
        public float DragonAcc { get; set; }
        public float DragonAccMulti { get; set; }
        public float IceAcc { get; set; }
        public float IceAccMulti { get; set; }
        public float NoneAcc { get; set; }
        public float NoneAccMulti { get; set; }

        public List<PartBreakTier> BreakTiers { get; set; } = new();
    }

    public class PartBreakTier
    {
        public int Tier { get; set; }           // 1..5
        public float DmgVal { get; set; }       // Cumulative damage threshold
        public int ProcessLv { get; set; }
        public float Hammer { get; set; }
        public float Cut { get; set; }
        public float Shoot { get; set; }
        public float Water { get; set; }
        public float Fire { get; set; }
        public float Electric { get; set; }
        public float Dragon { get; set; }
        public float Ice { get; set; }
        public int LootSkillLv { get; set; }
    }
}
