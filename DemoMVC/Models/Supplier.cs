namespace DemoMVC.Models
{
    /// <summary>
    /// Поставщик для для <see cref="Models.Product"/>
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Product> Product { get; set; } = new();
    }
}
