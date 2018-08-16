namespace PSol.Data.Models
{
    public class Inventory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ItemId { get; set; }

        // General
        public int Quantity { get; set; }
    }
}
