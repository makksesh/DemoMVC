namespace DemoMVC.Models
{
    /// <summary>
    /// Категория для <see cref="Models.Product"/>
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Product> Product { get; set; } = new();

    }
}
