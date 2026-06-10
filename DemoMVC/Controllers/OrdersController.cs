using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    // Доступ только для Менеджера и Администратора
    [Authorize(Roles = "Модератор,Администратор")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context) => _context = context;

        // ── Список заказов ────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.PickupPoint)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // ── Создание заказа (только Администратор) ──────────────────────

        [Authorize(Roles = "Администратор")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Order { CreatedAt = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create(
            [Bind("Code,OrderStatusId,PickupPointId,CreatedAt,DeliveryDate")] Order order)
        {
            // UserId — привязываем заказ к текущему пользователю
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                order.UserId = userId;

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(order);
                return View(order);
            }

            _context.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заказ успешно добавлен.";
            return RedirectToAction(nameof(Index));
        }

        // ── Редактирование заказа (только Администратор) ──────────────────

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order is null) return NotFound();

            PopulateDropdowns(order);
            return View(order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Code,OrderStatusId,PickupPointId,CreatedAt,DeliveryDate,UserId")] Order order)
        {
            if (id != order.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(order);
                return View(order);
            }

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Заказ успешно обновлён.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ── Удаление заказа (только Администратор) ───────────────────────

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
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
        private void PopulateDropdowns(Order? order = null)
        {
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses,  "Id", "Name",    order?.OrderStatusId);
            ViewData["PickupPointId"] = new SelectList(_context.PickupPoints,   "Id", "Address", order?.PickupPointId);
        }
    }
}
