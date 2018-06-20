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
        public MobType Type { get; set; }
        public float Health { get; set; }
        public float Shield { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Dir { get; set; }
        public float Rotation { get; set; }
        public DateTime SpawnDate { get; set; }
    }
}
