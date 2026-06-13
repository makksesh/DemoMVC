namespace DemoMVC.ViewModels
{
    public class OrderListViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;

        public string ProductArticles { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
