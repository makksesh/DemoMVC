using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DemoMVC.Models
{
    /// <summary>
    /// Номенклатура заказа
    /// </summary>
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DeliveryDate { get; set; }
        [MaxLength(4)]
        public string Code { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new();

        public int UserId { get; set; }
        public int OrderStatusId { get; set; }
        public int PickupPointId { get; set; }

        public User User { get; set; } = null!;
        public OrderStatus OrderStatus { get; set; } = null!;
        public PickupPoint PickupPoint { get; set; } = null!;


    }
}
