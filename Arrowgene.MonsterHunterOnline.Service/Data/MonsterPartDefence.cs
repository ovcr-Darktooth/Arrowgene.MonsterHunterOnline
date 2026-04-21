namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    /// <summary>
    /// Per-part damage multipliers applied when a weapon hits this part.
    /// Row from <c>monsterdata.dat_PartDefence.csv</c>.
    /// </summary>
    public class MonsterPartDefence
    {
        public int MonsterId { get; set; }
        public string PartId { get; set; }
        public string StateId { get; set; }

        public int BreakLevel { get; set; }
        public int DefenceLevel { get; set; }
        public float Faint { get; set; }

        public float Cut { get; set; }
        public float Hammer { get; set; }
        public float Shoot { get; set; }
        public float Fire { get; set; }
        public float Water { get; set; }
        public float Electric { get; set; }
        public float Ice { get; set; }
        public float Dragon { get; set; }
    }
}
