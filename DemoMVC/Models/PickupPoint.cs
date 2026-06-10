namespace DemoMVC.Models
{
    /// <summary>
    /// Пункт выдачи заказов для <see cref="Models.Order"/>
    /// </summary>
    public class PickupPoint
    {
        public int Id { get; set; }
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
