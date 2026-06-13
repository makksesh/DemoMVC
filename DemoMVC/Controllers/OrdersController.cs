using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    // Доступ только для Менеджера и Администратора
    [Authorize(Policy = "StaffOnly")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private static readonly Random _random = new Random();
        private const bool SingleItemMode = false; // true = 1 заказ — 1 позиция

        public OrdersController(AppDbContext context) => _context = context;

        // ── Список заказов ────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.OrderStatus)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.PickupPoint)
                .Include(oi => oi.Product)
                .ToListAsync();

            var orders = orderItems
                .GroupBy(oi => new
                {
                    oi.OrderId,
                    oi.Order.Code,
                    StatusName = oi.Order.OrderStatus.Name,
                    PickupAddress = oi.Order.PickupPoint.Address,
                    oi.Order.CreatedAt,
                    oi.Order.DeliveryDate
                })
                .Select(g => new OrderListViewModel
                {
                    OrderId = g.Key.OrderId,
                    OrderCode = g.Key.Code,
                    ProductArticles = string.Join(", ",
                        g.Select(x => x.Product.Article).Distinct()),
                    StatusName = g.Key.StatusName,
                    PickupAddress = g.Key.PickupAddress,
                    CreatedAt = g.Key.CreatedAt,
                    DeliveryDate = g.Key.DeliveryDate
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(orders);
        }

        // ── Создание заказа (только Администратор) ──────────────────────

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            var model = new OrderEditViewModel
            {
                CreatedAt = DateTime.Today
            };

            model.Items.Add(new OrderItemEditViewModel
            {
                Quantity = 1
            });

            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(OrderEditViewModel model)
        {
            // Привязываем заказ к текущему пользователю
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                model.UserId = userId;

            if (SingleItemMode && model.Items.Count > 1)
                model.Items = model.Items.Take(1).ToList();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            var order = new Order
            {
                Code = _random.Next(0, 1000).ToString("D3"),
                UserId = model.UserId,
                OrderStatusId = model.OrderStatusId,
                PickupPointId = model.PickupPointId,
                CreatedAt = model.CreatedAt,
                DeliveryDate = model.DeliveryDate
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in model.Items.Where(i => i.ProductId > 0))
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity < 1 ? 1 : item.Quantity
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заказ успешно добавлен.";
            return RedirectToAction(nameof(Index));
        }

        // ── Редактирование заказа (только Администратор) ──────────────────

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return NotFound();

            var vm = new OrderEditViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderStatusId = order.OrderStatusId,
                PickupPointId = order.PickupPointId,
                CreatedAt = order.CreatedAt,
                DeliveryDate = order.DeliveryDate,
                Items = order.OrderItems
                    .Select(oi => new OrderItemEditViewModel
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity
                    })
                    .ToList()
            };

            // Гарантируем минимум одну строку в SingleItemMode
            if (SingleItemMode && vm.Items.Count == 0)
                vm.Items.Add(new OrderItemEditViewModel { Quantity = 1 });

            PopulateDropdowns(vm);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id, OrderEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (SingleItemMode && model.Items.Count > 1)
                model.Items = model.Items.Take(1).ToList();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return NotFound();

            order.OrderStatusId = model.OrderStatusId;
            order.PickupPointId = model.PickupPointId;
            order.CreatedAt = model.CreatedAt;
            order.DeliveryDate = model.DeliveryDate;

            // Удаляем старые позиции и записываем заново
            _context.OrderItems.RemoveRange(order.OrderItems);

            foreach (var item in model.Items.Where(i => i.ProductId > 0))
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity < 1 ? 1 : item.Quantity
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заказ успешно обновлён.";
            return RedirectToAction(nameof(Index));
        }

        // ── Удаление заказа (только Администратор) ───────────────────────

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return NotFound();

            if (order.OrderItems.Any())
                _context.OrderItems.RemoveRange(order.OrderItems);

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заказ успешно удалён.";
            return RedirectToAction(nameof(Index));
        }

        // ── Вспомогательные методы ─────────────────────────────────────────

        /// <summary>Заполняет ViewData для выпадающих списков формы.</summary>
        private void PopulateDropdowns(OrderEditViewModel model = null)
        {
            var products = _context.Products
                .OrderBy(p => p.Article)
                .Select(p => new { p.Id, Name = p.Article + " — " + p.Name })
                .ToList();

            // Храним общий список продуктов в ViewBag — он понадобится для каждой строки
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.SingleItemMode = SingleItemMode;

            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Name", model?.OrderStatusId);
            ViewData["PickupPointId"] = new SelectList(_context.PickupPoints, "Id", "Address", model?.PickupPointId);
        }
    }
}
