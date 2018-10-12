
using System;

namespace PSol.Data.Models
{
    public class Loot
    {
        // Classification information
        public string Id { get; set; }
        public string Owner { get; set; }
        public string[] Items { get; set; }

        // General
        public int[] Quantities { get; set; }

        // Global dropped inventory data
        public DateTime Dropped { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
