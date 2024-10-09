using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UserManagmentApp.Data;
using UserManagmentApp.Models;

namespace UserManagmentApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
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
        [ValidateAntiForgeryToken]
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
                    HttpContext.Session.SetString("UserType", user.UserType);
                    HttpContext.Session.SetString("WelcomeMessage", $"Witamy {user.UserName}!"); // Ustawienie komunikatu powitalnego

                    return RedirectToAction("Index", "Home"); // Przekierowanie do głównej strony
                }
            }

            ViewBag.Error = "Nieprawidłowe dane logowania";
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
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Sprawdzamy typ użytkownika i przekierowujemy na odpowiedni widok
            if (user.UserType == "admin")
            {
                return View("AdminDashboard", user); // Widok dla admina
            }

            return View("UserDashboard", user); // Widok dla zwykłego użytkownika
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
}