using System.Collections.Generic;
using Bindings;
using PSol.Data.Models;

namespace PSol.Server
{
    internal class Globals
    {
        /// <summary>
        /// "Static" data has changed.  Set this bool to broadcast data not in normal broadcast
        /// </summary>
        public static bool FullData;

        public static ICollection<Star> Galaxy;
        public static ICollection<Item> Items;
        public static ICollection<Inventory> Inventory = new List<Inventory>();
        public static List<Loot> Loot = new List<Loot>();
    }
}
