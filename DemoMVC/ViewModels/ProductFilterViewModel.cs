using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.ViewModels
{
    public class ProductFilterViewModel
    {
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
        public int? SupplierId { get; set; }
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
    }
}
