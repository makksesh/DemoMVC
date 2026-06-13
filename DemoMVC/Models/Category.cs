using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Категория для <see cref="Models.Product"/>
    /// </summary>
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; } = null!;
        public List<Product> Product { get; set; } = new();
    }
}
