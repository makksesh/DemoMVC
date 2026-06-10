namespace DemoMVC.Models
{
    /// <summary>
    /// Статус заказа для <see cref="Models.Order"/>
    /// </summary>
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
