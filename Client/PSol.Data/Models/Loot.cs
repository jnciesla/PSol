
using System;

namespace PSol.Data.Models
{
    public class Loot
    {
        // Classification information
        public string Id { get; set; }
        public string Owner { get; set; }

        // General
        public string[] Items { get; set; }
        public int[] Quantities { get; set; }
        public int credits;

        // Global dropped inventory data
        public DateTime Dropped { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
