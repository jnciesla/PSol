using System.Collections.Generic;

namespace PSol.Data.Models
{
    public class Star
    {
        public string Id { get; set; }

        // General
        public string Name { get; set; }
        public ICollection<Planet> Planets { get; set; }
        public string Belligerence { get; set; }
        public string Class { get; set; }

        // Position
        public float X { get; set; }
        public float Y { get; set; }

        public Star()
        {
            Planets = new List<Planet>();
        }
    }
}
