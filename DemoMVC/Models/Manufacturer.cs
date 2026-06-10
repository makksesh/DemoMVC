namespace DemoMVC.Models
{
    /// <summary>
    /// Производитель для <see cref="Models.Product"/>
    /// </summary>
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Product> Product { get; set; } = new();
    }
}
