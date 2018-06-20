using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSol.Data.Models
{
    public class MobType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int MaxShield { get; set; }
        public int Sprite { get; set; }
        public int Level { get; set; }
        public int BonusExp { get; set; }
        public int Credits { get; set; }
        public int MaxSpawned { get; set; }
        public int SpawnTimeMin { get; set; }
        public int SpawnTimeMax { get; set; }

        // Position
        public Star Star { get; set; }
        public float SpawnRadius { get; set; }
    }
}
