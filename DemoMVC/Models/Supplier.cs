using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Поставщик для для <see cref="Models.Product"/>
    /// </summary>
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public List<Product> Product { get; set; } = new();
    }
}
