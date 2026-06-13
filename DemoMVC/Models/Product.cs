using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Номенклатура товара
    /// </summary>
    [Index(nameof(Article), IsUnique =true)]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Article { get; set; } = null!;
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [MaxLength(250)]
        public string? Description { get; set; }

        [Precision(18,2)]
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal Discount { get; set; }
        [MaxLength(50)]
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
