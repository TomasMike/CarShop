namespace CarShop.Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public required Car Car { get; set; }
        public int CustomerId { get; set; }
        public required Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}




