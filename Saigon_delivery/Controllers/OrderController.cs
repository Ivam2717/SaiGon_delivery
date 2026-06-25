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
    public class OrderController : Controller
    {
        private readonly SaigonDeliveryContext _context;

        public OrderController(SaigonDeliveryContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }
        public IActionResult Create() => View(new OrderViewModel());
        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = new Order
            {
                CustomerId = customerId,
                ReceiverName = model.ReceiverName,
                ReceiverPhone = model.ReceiverPhone,
                DeliveryAddress = model.DeliveryAddress,
                Distance = model.Distance,
                ShipFee = CalculateShipFee(model.Distance),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đặt hàng thành công! Chúng tôi sẽ sớm gán shipper cho bạn.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int id)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

            if (order == null) return NotFound();
            return View(order);
        }
        public static decimal CalculateShipFee(decimal distanceKm)
        {
            const decimal baseFee = 15000;
            const decimal baseDistance = 3;
            const decimal extraPerKm = 5000;
            if (distanceKm <= baseDistance) return baseFee;
            return baseFee + Math.Ceiling(distanceKm - baseDistance) * extraPerKm;
        }
        [HttpGet]
        public IActionResult GetShipFee(decimal distance)
        {
            var fee = CalculateShipFee(distance);
            return Json(new { fee, feeFormatted = fee.ToString("N0") + "đ" });
        }
    }
}