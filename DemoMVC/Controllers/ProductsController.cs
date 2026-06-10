using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.ViewModels;

namespace DemoMVC.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IAuthorizationService _authService;

        public ProductsController(AppDbContext context, IWebHostEnvironment env, IAuthorizationService authService)
        {
            _context = context;
            _env     = env;
            _authService = authService;
        }

        // ── Список товаров ────────────────────────────────────────────────────

        public async Task<IActionResult> Index(string? searchQuery, string? sortBy, int? supplierId)
        {
            var staffCheck = await _authService.AuthorizeAsync(User, "StaffOnly");
            var isStaff = staffCheck.Succeeded;

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .Include(p => p.UnitOfMeasure)
                .AsQueryable();

            // Поиск, фильтр и сортировка — только для менеджера/администратора
            if (isStaff)
            {
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    var s = searchQuery.ToLower();
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(s)         ||
                        p.Article.ToLower().Contains(s)      ||
                        (p.Description != null && p.Description.ToLower().Contains(s)) ||
                        p.Category.Name.ToLower().Contains(s)     ||
                        p.Supplier.Name.ToLower().Contains(s)     ||
                        p.Manufacturer.Name.ToLower().Contains(s));
                }

                if (supplierId > 0)
                    query = query.Where(p => p.SupplierId == supplierId);

                query = sortBy switch
                {
                    "price_asc"  => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "qty_asc"    => query.OrderBy(p => p.Quantity),
                    "qty_desc"   => query.OrderByDescending(p => p.Quantity),
                    _            => query.OrderBy(p => p.Id),
                };
            }

            // Список поставщиков для фильтра 
            var suppliers = await _context.Suppliers
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToListAsync();
            suppliers.Insert(0, new SelectListItem { Value = "", Text = "Все поставщики" });

            var vm = new ProductFilterViewModel
            {
                Products    = await query.ToListAsync(),
                SearchQuery = searchQuery,
                SortBy      = sortBy,
                SupplierId  = supplierId,
                Suppliers   = suppliers,
            };

            return View(vm);
        }

        // ── Создание товара (только Администратор) ────────────────────────────

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Product());
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(
            [Bind("Article,Name,Description,MeasurementId,Price,Quantity,Discount,CategoryId,SupplierId,ManufacturerId")]
            Product product,
            IFormFile? imageFile)
        {
            ValidatePriceAndQuantity(product);

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(product);
                return View(product);
            }

            if (imageFile?.Length > 0)
                product.ImagePath = await SaveImageAsync(imageFile, null);

            _context.Add(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Товар успешно добавлен.";
            return RedirectToAction(nameof(Index));
        }

        // ── Редактирование товара (только Администратор) ──────────────────────

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();

            PopulateDropdowns(product);
            return View(product);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Article,Name,Description,MeasurementId,Price,Quantity,Discount,ImagePath,CategoryId,SupplierId,ManufacturerId")]
            Product product,
            IFormFile? imageFile)
        {
            if (id != product.Id) return NotFound();

            ValidatePriceAndQuantity(product);

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(product);
                return View(product);
            }

            try
            {
                // Заменяем фото если загружено новое (старое удаляется)
                if (imageFile?.Length > 0)
                    product.ImagePath = await SaveImageAsync(imageFile, product.ImagePath);

                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Товар успешно обновлён.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ── Удаление товара (только Администратор) ────────────────────────────

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();

            // Товар в заказе удалять нельзя
            if (await _context.OrderItems.AnyAsync(oi => oi.ProductId == id))
            {
                TempData["ErrorMessage"] = "Нельзя удалить товар, который присутствует в заказе.";
                return RedirectToAction(nameof(Index));
            }

            DeleteImageFile(product.ImagePath);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Товар успешно удалён.";
            return RedirectToAction(nameof(Index));
        }

        // ── Вспомогательные методы ────────────────────────────────────────────

        private bool ProductExists(int id) =>
            _context.Products.Any(e => e.Id == id);

        /// <summary>Заполняет ViewData для всех выпадающих списков формы.</summary>
        private void PopulateDropdowns(Product? product = null)
        {
            ViewData["CategoryId"]     = new SelectList(_context.Categories,    "Id", "Name", product?.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product?.ManufacturerId);
            ViewData["SupplierId"]     = new SelectList(_context.Suppliers,     "Id", "Name", product?.SupplierId);
            ViewData["UnitOfMeasure"]  = new SelectList(_context.Measurements,  "Id", "Name", product?.MeasurementId);
        }

        /// <summary>Добавляет ошибки ModelState если цена или количество отрицательны.</summary>
        private void ValidatePriceAndQuantity(Product product)
        {
            if (product.Price < 0)
                ModelState.AddModelError("Price", "Цена не может быть отрицательной.");
            if (product.Quantity < 0)
                ModelState.AddModelError("Quantity", "Количество не может быть отрицательным.");
        }

        /// <summary>
        /// Сохраняет изображение в wwwroot/img/products, удаляет старый файл.
        /// Возвращает имя нового файла.
        /// </summary>
        private async Task<string> SaveImageAsync(IFormFile imageFile, string? oldFileName)
        {
            var folder = Path.Combine(_env.WebRootPath, "img", "products");
            Directory.CreateDirectory(folder);

            DeleteImageFile(oldFileName);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            await using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return fileName;
        }

        /// <summary>Удаляет файл изображения с диска если он существует.</summary>
        private void DeleteImageFile(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var path = Path.Combine(_env.WebRootPath, "img", "products", fileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
