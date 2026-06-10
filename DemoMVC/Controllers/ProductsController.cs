using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DemoMVC.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Index(
            string? searchQuery,
            string? sortBy,
            int? supplierId)
        {
            var isManagerOrAdmin = User.IsInRole("Менеджер") || User.IsInRole("Администратор");

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .Include(p => p.UnitOfMeasure)
                .AsQueryable();

            // Поиск (только для менеджера и администратора)
            if (isManagerOrAdmin && !string.IsNullOrWhiteSpace(searchQuery))
            {
                var s = searchQuery.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    p.Article.ToLower().Contains(s) ||
                    (p.Description != null && p.Description.ToLower().Contains(s)) ||
                    p.Category.Name.ToLower().Contains(s) ||
                    p.Supplier.Name.ToLower().Contains(s) ||
                    p.Manufacturer.Name.ToLower().Contains(s));
            }

            // Фильтрация по поставщику
            if (isManagerOrAdmin && supplierId.HasValue && supplierId.Value > 0)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            // Сортировка
            if (isManagerOrAdmin)
            {
                query = sortBy switch
                {
                    "price_asc"  => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "qty_asc"    => query.OrderBy(p => p.Quantity),
                    "qty_desc"   => query.OrderByDescending(p => p.Quantity),
                    _            => query.OrderBy(p => p.Id)
                };
            }

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
                Suppliers   = suppliers
            };

            return View(vm);
        }

        // GET: Products/Create  (только Администратор)
        [Authorize(Roles = "Администратор")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Product());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create(
            [Bind("Article,Name,Description,MeasurementId,Price,Quantity,Discount,CategoryId,SupplierId,ManufacturerId")] Product product,
            IFormFile? imageFile)
        {
            if (product.Price < 0)
                ModelState.AddModelError("Price", "Цена не может быть отрицательной.");
            if (product.Quantity < 0)
                ModelState.AddModelError("Quantity", "Количество не может быть отрицательным.");

            if (ModelState.IsValid)
            {
                // Id генерируется автоматически через IDENTITY — не задаём вручную

                if (imageFile != null && imageFile.Length > 0)
                    product.ImagePath = await SaveImageAsync(imageFile, null);

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Товар успешно добавлен.";
                return RedirectToAction(nameof(Index));
            }

            // Возвращаем форму с ошибками — asp-validation-summary покажет их на странице Create
            PopulateDropdowns(product);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            PopulateDropdowns(product);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Article,Name,Description,MeasurementId,Price,Quantity,Discount,ImagePath,CategoryId,SupplierId,ManufacturerId")] Product product,
            IFormFile? imageFile)
        {
            if (id != product.Id)
                return NotFound();

            if (product.Price < 0)
                ModelState.AddModelError("Price", "Цена не может быть отрицательной.");
            if (product.Quantity < 0)
                ModelState.AddModelError("Quantity", "Количество не может быть отрицательным.");

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Удаляем старое фото и сохраняем новое
                        product.ImagePath = await SaveImageAsync(imageFile, product.ImagePath);
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Товар успешно обновлён.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(product);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            bool inOrder = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (inOrder)
            {
                TempData["ErrorMessage"] = "Нельзя удалить товар, который присутствует в заказе.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var filePath = Path.Combine(_env.WebRootPath, "img", "products", product.ImagePath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Товар успешно удалён.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Вспомогательные методы ──────────────────────────────────────────

        private bool ProductExists(int id) =>
            _context.Products?.Any(e => e.Id == id) ?? false;

        private void PopulateDropdowns(Product? product = null)
        {
            ViewData["CategoryId"]     = new SelectList(_context.Categories, "Id", "Name", product?.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product?.ManufacturerId);
            ViewData["SupplierId"]     = new SelectList(_context.Suppliers, "Id", "Name", product?.SupplierId);
            ViewData["UnitOfMeasure"]  = new SelectList(_context.Measurements, "Id", "Name", product?.MeasurementId);
        }

        /// <summary>
        /// Сохраняет загруженное изображение в wwwroot/img/products,
        /// удаляет старый файл если он был. Возвращает имя нового файла.
        /// </summary>
        private async Task<string> SaveImageAsync(IFormFile imageFile, string? oldFileName)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "img", "products");
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var newFilePath = Path.Combine(uploadsFolder, newFileName);

            using var stream = new FileStream(newFilePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return newFileName;
        }
    }
}
