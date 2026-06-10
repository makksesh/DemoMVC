namespace DemoMVC.Models
{
    /// <summary>
    /// Единица измерения для <see cref="Models.Product"/>
    /// </summary>
    public class Measurement
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Product> Product { get; set; } = new();
    }
}
