using System.Collections.Generic;

namespace PSol.Data.Models
{
    public class Item
    {
        public string Id { get; set; }

        // General
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Image { get; set; }
        public int Color { get; set; }
        public int Mass { get; set; }
        public int Cost { get; set; }
        public bool Stack { get; set; }
        public int Level { get; set; }

        // Attributes
        public int Hull { get; set; }
        public int Shield { get; set; }
        public int Armor { get; set; }
        public int Thrust { get; set; }
        public int Power { get; set; }
        public int Damage { get; set; }
        public int Recharge { get; set; }
        public int Repair { get; set; }
        public int Defense { get; set; }
        public int Offense { get; set; }
        public int Capacity { get; set; }
        public int Weapons { get; set; }
        public int Special { get; set; }
    }
}
