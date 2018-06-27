using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSol.Data.Models
{
    public class Mob
    {
        public string Id { get; set; }
        public string MobTypeId { get; set; }
        public MobType MobType { get; set; }
        public float Health { get; set; }
        public float Shield { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public DateTime SpawnDate { get; set; }
        public DateTime KilledDate { get; set; }
        public bool Alive { get; set; } = true;
    }
}
