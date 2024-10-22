using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
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
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);
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
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
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

            if (userId != null)
            {
                user = _context.Users.FirstOrDefault(u => u.Id == userId);
            }

            var products = await _context.Products.ToListAsync();

            var viewModel = new ProductViewModel
            {
                User = user,
                products = products
            };

            return View(viewModel);
        }

        [Route("/products/AddProducts")]
        // GET: Products/AddProducts
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
            return RedirectToAction("Products");
        }

        // GET: Products/Edit/5
        [HttpGet]
        [Route("/products/EditProduct/{productId}")]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [Route("/products/EditProduct/{productId}")]
        public async Task<IActionResult> EditProduct(int productId, Products updatedProduct)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (productId != updatedProduct.ProductId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(p => p.ProductId == productId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Products");
            }
            return View(updatedProduct);
        }

        // GET: Products/Delete/5
        [HttpGet]
        [Route("/products/DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("DeleteProduct")]
        [Route("/products/DeleteProductConfirmed/{productId}")]
        public async Task<IActionResult> DeleteProductConfirmed(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Products");
        }
        [HttpGet]
        [Route("/products/Details/{productId}")]
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product); // Otwiera widok ze szczegółami produktu
        }

    }
}
