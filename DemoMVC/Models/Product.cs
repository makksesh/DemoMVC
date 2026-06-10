using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Номенклатура товара
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Article { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }


        [Precision(18,2)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal Discount { get; set; }
        public string? ImagePath { get; set; }

        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int ManufacturerId { get; set; }
        public int MeasurementId { get; set; } 

        public Category? Category { get; set; } 
        public Supplier? Supplier { get; set; } 
        public Manufacturer? Manufacturer { get; set; } 
        public Measurement? Measurement { get; set; } 
    }
}
