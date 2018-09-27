using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Name { get; set; }
        public bool Special { get; set; }
        [NotMapped]
        public string TargettingId { get; set; }
        [NotMapped]
        public float? NavToX { get; set; }
        [NotMapped]
        public float? NavToY { get; set; }
    }
}
