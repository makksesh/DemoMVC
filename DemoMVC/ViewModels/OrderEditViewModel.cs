using System.ComponentModel.DataAnnotations;

namespace DemoMVC.ViewModels
{
    public class OrderEditViewModel
    {
        public int? Id { get; set; }

        public int UserId { get; set; }
        public int OrderStatusId { get; set; }
        public int PickupPointId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public List<OrderItemEditViewModel> Items { get; set; } = new();
    }

    public class OrderItemEditViewModel
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
