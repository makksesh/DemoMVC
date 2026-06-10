using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Controllers
{
    // HomeController оставлен только для обработки маршрута по умолчанию — перенаправляет на товары
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Products");
    }
}
