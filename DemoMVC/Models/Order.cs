using System.Data;

namespace DemoMVC.Models
{
    /// <summary>
    /// Номенклатура заказа
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Code { get; set; } = null!;
        
        public int UserId { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public int OrderStatusId { get; set; }
        public int PickupPointId { get; set; }

        public User User { get; set; } = null!;
        public OrderStatus OrderStatus { get; set; } = null!;
        public PickupPoint PickupPoint { get; set; } = null!;

    }
}
