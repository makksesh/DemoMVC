namespace DemoMVC.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public List<Product> ProductId { get; set; } = new();
    }
}
