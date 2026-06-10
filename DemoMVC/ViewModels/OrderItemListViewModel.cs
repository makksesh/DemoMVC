namespace DemoMVC.ViewModels
{
    public class OrderItemListViewModel
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }

        public string OrderCode { get; set; } = string.Empty;
        public string ProductArticle { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public int Quantity { get; set; }
    }
}
