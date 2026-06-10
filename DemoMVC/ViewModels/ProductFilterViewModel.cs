using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.ViewModels
{
    public class ProductFilterViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        // Параметры поиска, сортировки и фильтрации
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }       // "price_asc", "price_desc", "qty_asc", "qty_desc"
        public int? SupplierId { get; set; }

        // Для выпадающего списка поставщиков
        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
    }
}
