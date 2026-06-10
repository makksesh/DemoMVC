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

        public OrdersController(AppDbContext context) => _context = context;

        // ── Список заказов ────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var items = await _context.OrderItems
                .Select(oi => new OrderItemListViewModel
                {
                    OrderItemId = oi.Id,
                    OrderId = oi.OrderId,
                    OrderCode = oi.Order.Code,
                    ProductArticle = oi.Product.Article,
                    ProductName = oi.Product.Name,
                    StatusName = oi.Order.OrderStatus.Name,
                    PickupAddress = oi.Order.PickupPoint.Address,
                    CreatedAt = oi.Order.CreatedAt,
                    DeliveryDate = oi.Order.DeliveryDate,
                    Quantity = oi.Quantity
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(items);
        }

        // ── Создание заказа (только Администратор) ──────────────────────

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new OrderEditViewModel { CreatedAt = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(OrderEditViewModel model)
        {
            // Привязываем заказ к текущему пользователю
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                model.UserId = userId;

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.ProductId, model.OrderStatusId, model.PickupPointId);
                return View(model);
            }

            var order = new Order
            {
                Code = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                UserId = model.UserId,
                OrderStatusId = model.OrderStatusId,
                PickupPointId = model.PickupPointId,
                CreatedAt = model.CreatedAt,
                DeliveryDate = model.DeliveryDate
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = model.ProductId,
                Quantity = 1
            };

            _context.OrderItems.Add(orderItem);
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
                ProductId = order.OrderItems.FirstOrDefault()?.ProductId ?? 0,
                OrderStatusId = order.OrderStatusId,
                PickupPointId = order.PickupPointId,
                CreatedAt = order.CreatedAt,
                DeliveryDate = order.DeliveryDate
            };

            PopulateDropdowns(vm.ProductId, vm.OrderStatusId, vm.PickupPointId);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id, OrderEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.ProductId, model.OrderStatusId, model.PickupPointId);
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

            var orderItem = order.OrderItems.FirstOrDefault();
            if (orderItem != null)
            {
                orderItem.ProductId = model.ProductId;
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
            var order = await _context.Orders.FindAsync(id);
            if (order is null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заказ успешно удалён.";
            return RedirectToAction(nameof(Index));
        }

        // ── Вспомогательные методы ─────────────────────────────────────────

        /// <summary>Заполняет ViewData для выпадающих списков формы.</summary>
        private void PopulateDropdowns(int? productId = null, int? statusId = null, int? pickupPointId = null)
        {
            ViewData["ProductId"] = new SelectList(
                _context.Products
                    .OrderBy(p => p.Article)
                    .Select(p => new
                    {
                        p.Id,
                        Name = p.Article + " — " + p.Name
                    }),
                "Id",
                "Name",
                productId);

            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Name", statusId);
            ViewData["PickupPointId"] = new SelectList(_context.PickupPoints, "Id", "Address", pickupPointId);
        }
    }
}
