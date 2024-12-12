using Microsoft.AspNetCore.Mvc;

namespace BookStoreSystem.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
