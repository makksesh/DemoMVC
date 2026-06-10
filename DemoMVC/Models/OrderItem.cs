using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Единица одного заказа в <see cref="Models.Order"/>
    /// </summary>
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [Precision(18,2)]
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public Product Product { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
