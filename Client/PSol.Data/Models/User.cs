using System.Collections.Generic;

namespace PSol.Data.Models
{
    public class User
    {
        public string Id { get; set; }
        // Account
        public string Login { get; set; }
        public string Password { get; set; }

        // General
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int MaxShield { get; set; }
        public int Shield { get; set; }
        public int Sprite { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Credits { get; set; }

        // Inventory
        public virtual ICollection<Inventory> Inventory { get; set; }

        // Position
        public int Map { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Dir { get; set; }
        public float Rotation { get; set; }
    }
}
