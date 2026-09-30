using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saigon_delivery.Data;
using Saigon_delivery.Models.ViewModels;

namespace Saigon_delivery.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly SaigonDeliveryContext _context;

        public AdminController(SaigonDeliveryContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.ShipFee),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending"),
                CompletedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed"),
            };
            return View(vm);
        }

        public async Task<IActionResult> Orders(string? status)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            ViewBag.CurrentStatus = status;
            ViewBag.Shippers = await _context.Users
                .Where(u => u.Role == "Shipper")
                .ToListAsync();
            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> AssignShipper(int orderId, Guid shipperId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Orders");
            }

            order.ShipperId = shipperId;
            order.Status = "Pending";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã gán shipper cho đơn #{orderId}.";
            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Report(string groupBy = "day")
        {
            var shipperStats = await _context.Users
                .Where(u => u.Role == "Shipper")
                .Select(u => new ShipperStatViewModel
                {
                    ShipperName = u.FullName,
                    TotalOrders = u.OrderShippers.Count(),
                    CompletedOrders = u.OrderShippers.Count(o => o.Status == "Completed"),
                    TotalRevenue = u.OrderShippers.Where(o => o.Status == "Completed").Sum(o => o.ShipFee),
                    AvgRating = u.ShipperRatingShippers.Any()
                                     ? Math.Round(u.ShipperRatingShippers.Average(r => (double)r.Stars), 1)
                                     : 0
                })
                .ToListAsync();

            List<ChartDataPoint> chartData;

            if (groupBy == "month")
            {
                chartData = await _context.Orders
                   .Where(o => o.Status == "Completed")
                   .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                   .Select(g => new ChartDataPoint
                   {
                       Label = g.Key.Month.ToString() + "/" + g.Key.Year.ToString(),
                       Revenue = g.Sum(o => o.ShipFee)
                   })
                   .OrderBy(x => x.Label)
                   .ToListAsync();
            }
            else
            {
                chartData = await _context.Orders
                    .Where(o => o.Status == "Completed")
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
                    .Select(g => new ChartDataPoint
                    {
                        Label = g.Key.Day.ToString() + "/" + g.Key.Month.ToString(),
                        Revenue = g.Sum(o => o.ShipFee)
                    })
                    .OrderBy(x => x.Label)
                    .ToListAsync();
            }

            ViewBag.GroupBy = groupBy;
            ViewBag.ChartData = chartData;

            return View(shipperStats);
        }
    }
}