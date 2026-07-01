using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saigon_delivery.Data;
using System.Security.Claims;

namespace Saigon_delivery.Controllers
{
    [Authorize(Roles = "Shipper")]
    public class ShipperController : Controller
    {
        private readonly SaigonDeliveryContext _context;

        public ShipperController(SaigonDeliveryContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var shipperId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.ShipperId == shipperId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            var shipperId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.ShipperId == shipperId);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }
            var validTransition = (order.Status, newStatus) switch
            {
                ("Pending", "Shipping") => true,
                ("Shipping", "Completed") => true,
                _ => false
            };
            if (!validTransition)
            {
                TempData["Error"] = "Cập nhật trạng thái không hợp lệ.";
                return RedirectToAction("Index");
            }
            order.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["Success"] = newStatus == "Shipping"
                ? "Đã xác nhận lấy hàng — đang giao."
                : "Đã xác nhận giao hàng thành công.";
            return RedirectToAction("Index");
        }
    }
}