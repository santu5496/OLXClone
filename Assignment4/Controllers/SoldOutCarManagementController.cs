using Microsoft.AspNetCore.Mvc;

namespace Assignment4.Controllers
{
    public class SoldOutCarManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
