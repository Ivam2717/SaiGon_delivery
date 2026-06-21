using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Saigon_delivery.Data;
using Saigon_delivery.Models.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Saigon_delivery.Controllers
{
    public class AccountController : Controller
    {
        private readonly SaigonDeliveryContext _context;
        public AccountController(SaigonDeliveryContext context)
        {
            _context = context;
        }
        // GET: /Account/Login
        public IActionResult Login() => View();
        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var hashedPassword = HashPassword(model.Password);
            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email
                                  && u.PasswordHash == hashedPassword);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(model);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", principal);
            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Shipper" => RedirectToAction("Index", "Shipper"),
                _ => RedirectToAction("Index", "Order")
            };
        }
        // GET: /Account/Register
        public IActionResult Register() => View();
        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng");
                return View(model);
            }
            var user = new Saigon_delivery.Models.User
            {
                Id = Guid.NewGuid(),
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                PasswordHash = HashPassword(model.Password),
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đăng ký thành công, vui lòng đăng nhập";
            return RedirectToAction("Login");
        }
        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
        // Helper: hash mật khẩu bằng SHA256
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}