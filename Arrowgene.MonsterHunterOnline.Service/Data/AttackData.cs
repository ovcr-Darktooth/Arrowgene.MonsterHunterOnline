namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class AttackData
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string AttackName { get; set; }

        public float DamageNumber { get; set; }
        public int Piyo { get; set; }
        public int Stamina { get; set; }
        public int DamageLevel { get; set; }
        public int DamageLevelNumber { get; set; }
        public int DamagePower { get; set; }
        public int AttackLevel { get; set; }
        public int DamageDirUse { get; set; }
        public string DamageDir { get; set; }

        public int FireAtk { get; set; }
        public int WaterAtk { get; set; }
        public int DragonAtk { get; set; }
        public int ElectricAtk { get; set; }
        public int IceAtk { get; set; }

        public int StateBuff { get; set; }
    }
}
