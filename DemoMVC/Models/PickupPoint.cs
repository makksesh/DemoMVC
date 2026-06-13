using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Пункт выдачи заказов для <see cref="Models.Order"/>
    /// </summary>
    public class PickupPoint
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(6)]
        public string PostalCode { get; set; } = null!;
        [MaxLength(20)]
        public string City { get; set; } = null!;
        [MaxLength(100)]
        public string Address { get; set; } = null!;
    }
}
