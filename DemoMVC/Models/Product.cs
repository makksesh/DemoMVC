using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Product
    {
        public int Id { get; set; }                 // PK в БД

        [MaxLength(100)]
        public string Article { get; set; } = null!;// Артикул из файла
        public string Name { get; set; } = null!;   // Наименование
        public string? Description { get; set; }    // Описание (может быть пустым)


        [Precision(18,2)]
        public decimal Price { get; set; }          // Цена с копейками
        public int Quantity { get; set; }           // Кол-во на складе
        [Precision(18, 2)]
        public decimal Discount { get; set; }       // Действующая скидка, %
        public string? ImagePath { get; set; }      // Путь к изображению

        // Внешние ключи
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int ManufacturerId { get; set; }
        public int MeasurementId { get; set; } 

        // Навигационные свойства
        public Category? Category { get; set; } 
        public Supplier? Supplier { get; set; } 
        public Manufacturer? Manufacturer { get; set; } 
        public Measurement? UnitOfMeasure { get; set; } 
    }
}
