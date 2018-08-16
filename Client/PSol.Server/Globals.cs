﻿using System.Collections.Generic;
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
    }
}
