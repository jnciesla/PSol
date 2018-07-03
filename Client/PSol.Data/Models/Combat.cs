using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PSol.Data.Models
{
    public class Combat
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public string WeaponId { get; set; }
        public int WeaponDamage { get; set; }
    }
}
