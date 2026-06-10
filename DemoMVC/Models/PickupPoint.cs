namespace DemoMVC.Models
{
    public class PickupPoint
    {
        public int Id { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int HouseNumber { get; set; }
    }
}
