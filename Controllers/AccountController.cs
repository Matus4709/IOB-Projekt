using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UserManagmentApp.Data;
using UserManagmentApp.Models;

namespace UserManagmentApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return View();
            }
            return RedirectToAction("Dashboard");
        }
        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Użycie PasswordHasher do zahashowania hasła
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                // Dodaj użytkownika do bazy danych
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return View();
            }
            return RedirectToAction("Dashboard");
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken] // Dodaj walidację CSRF
        public IActionResult Login(string userName, string password)
        {
            // Znajdź użytkownika po nazwie użytkownika
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                // Weryfikacja hasła hashowanego
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Ustaw sesję użytkownika
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return RedirectToAction("Dashboard", "Account");
                }
            }

            ViewBag.Error = "Invalid login attempt";
            return View();
        }
        // GET: Dashboard
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return View(user);
        }
        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
        [Route("/products")]
        // GET: Products
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            User user = null;

            // Jeśli użytkownik jest zalogowany, pobierz jego dane
            if (userId != null)
            {
                user = _context.Users.FirstOrDefault(u => u.Id == userId);
            }

            // Pobierz listę produktów
            var products = await _context.Products.ToListAsync();

            // Tworzymy obiekt ViewModel zawsze, niezależnie od tego, czy użytkownik jest zalogowany
            var viewModel = new ProductViewModel
            {
                User = user,  // Może być null, jeśli użytkownik nie jest zalogowany
                products = products
            };

            return View(viewModel);
        }


        private IActionResult View(User user, List<Products> products)
        {
            throw new NotImplementedException();
        }

        [Route("/products/AddProducts")]
        // GET: Products/AddProductsGet
        [HttpGet]

        public IActionResult AddProducts()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [Route("/products/AddProducts")]
        // POST: Products/AddProducts
        [HttpPost]
        public async Task<IActionResult> AddProducts(Products products)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
                await _context.AddAsync(products);
                await _context.SaveChangesAsync();
                return View();
        }
    }
}
