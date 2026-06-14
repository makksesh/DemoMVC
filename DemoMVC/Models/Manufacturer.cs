using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Производитель для <see cref="Models.Product"/>
    /// </summary>
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        //public List<Product> Product { get; set; } = new();
    }
}
