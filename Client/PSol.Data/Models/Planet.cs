namespace PSol.Data.Models
{
    public class Planet
    {
        public string Id { get; set; }
        public string StarId { get; set; }

        // General
        public string Name { get; set; }
        public int Sprite { get; set; }
        public int Color { get; set; }
        public string Belligerence { get; set; }
        public string Class { get; set; }

        // Position
        public float X { get; set; }
        public float Y { get; set; }
        public float Orbit { get; set; }
    }
}
