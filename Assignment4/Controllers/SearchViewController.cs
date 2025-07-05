using Microsoft.AspNetCore.Mvc;

namespace Assignment4.Controllers
{
    public class SearchViewController : Controller
    {
        public IActionResult SearchView()
        {
            return View();
        }
    }
}
