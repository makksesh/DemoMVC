using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Статус заказа для <see cref="Models.Order"/>
    /// </summary>
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; } = null!;
    }
}
