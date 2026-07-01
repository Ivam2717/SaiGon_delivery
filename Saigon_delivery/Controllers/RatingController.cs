using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saigon_delivery.Data;
using Saigon_delivery.Models;
using Saigon_delivery.Models.ViewModels;
using System.Security.Claims;

namespace Saigon_delivery.Controllers
{
    [Authorize(Roles = "Customer")]
    public class RatingController : Controller
    {
        private readonly SaigonDeliveryContext _context;

        public RatingController(SaigonDeliveryContext context)
        {
            _context = context;
        }

        // GET: /Rating/Create?orderId=5
        public async Task<IActionResult> Create(int orderId)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _context.Orders
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(o => o.Id == orderId
                                       && o.CustomerId == customerId
                                       && o.Status == "Completed");

            if (order == null)
            {
                TempData["Error"] = "Không thể đánh giá đơn hàng này.";
                return RedirectToAction("Index", "Order");
            }

            // Kiểm tra đã đánh giá chưa
            var existed = await _context.ShipperRatings
                .AnyAsync(r => r.OrderId == orderId);
            if (existed)
            {
                TempData["Error"] = "Bạn đã đánh giá đơn hàng này rồi.";
                return RedirectToAction("Detail", "Order", new { id = orderId });
            }

            var model = new RatingViewModel
            {
                OrderId = order.Id,
                ShipperId = order.ShipperId!.Value,
                ShipperName = order.Shipper!.FullName
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RatingViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existed = await _context.ShipperRatings
                .AnyAsync(r => r.OrderId == model.OrderId);
            if (existed)
            {
                TempData["Error"] = "Bạn đã đánh giá đơn hàng này rồi.";
                return RedirectToAction("Detail", "Order", new { id = model.OrderId });
            }

            var rating = new ShipperRating
            {
                OrderId = model.OrderId,
                CustomerId = customerId,
                ShipperId = model.ShipperId,
                Stars = model.Stars,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.ShipperRatings.Add(rating);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction("Detail", "Order", new { id = model.OrderId });
        }
    }
}