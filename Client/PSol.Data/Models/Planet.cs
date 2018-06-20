namespace PSol.Data.Models
{
    public class Planet
    {
        public string Id { get; set; }
        public string StarId { get; set; }

        // General
        public string Name { get; set; }

        // Position
        public float X { get; set; }
        public float Y { get; set; }
    }
}
