namespace NotificationService
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}