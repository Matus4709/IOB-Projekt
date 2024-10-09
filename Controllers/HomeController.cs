using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UserManagmentApp.Models;


namespace UserManagmentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var welcomeMessage = HttpContext.Session.GetString("WelcomeMessage");
            ViewBag.WelcomeMessage = welcomeMessage; // Przekazanie komunikatu do widoku

            // Wyczyszczenie komunikatu po wyœwietleniu, aby nie by³ widoczny przy odœwie¿eniu
            HttpContext.Session.Remove("WelcomeMessage");

            return View();
        }

        // GET: Products
        [HttpGet]
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
