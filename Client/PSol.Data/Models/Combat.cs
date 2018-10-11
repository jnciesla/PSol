namespace PSol.Data.Models
{
    public class Combat
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public Item Weapon { get; set; }
        public int WeaponDamage { get; set; }
    }
}
