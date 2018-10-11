using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSol.Data.Models
{
    public class User
    {
        public string Id { get; set; }
        // Account
        public string Login { get; set; }
        public string Password { get; set; }

        // General
        public string Rank { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int MaxShield { get; set; }
        public int Shield { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Credits { get; set; }

        // Attributes
        public int Armor { get; set; }
        public int Thrust { get; set; }
        public int Power { get; set; }
        public int Recharge { get; set; }
        public int Repair { get; set; }
        public int Defense { get; set; }
        public int Offense { get; set; }
        public int Capacity { get; set; }
        public int Weapons { get; set; }

        // Charges
        public int Weap1Charge { get; set; }
        public int Weap2Charge { get; set; }
        public int Weap3Charge { get; set; }
        public int Weap4Charge { get; set; }
        public int Weap5Charge { get; set; }
        public int Weap1ChargeRate { get; set; }
        public int Weap2ChargeRate { get; set; }
        public int Weap3ChargeRate { get; set; }
        public int Weap4ChargeRate { get; set; }
        public int Weap5ChargeRate { get; set; }

        // Inventory
        public virtual ICollection<Inventory> Inventory { get; set; }

        // Position
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
    }
}
