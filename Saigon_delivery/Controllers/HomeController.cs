using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Saigon_delivery.Controllers
{
    [Authorize(Roles = "Customer")]
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}