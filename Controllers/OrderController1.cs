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
    public class OrderController1 : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController1(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }


        [Route("/order/cart")]
        // GET: order/cart
        [HttpGet]
        public IActionResult OrderCart()
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var products = _context.Products.AsQueryable();
            var viewModel = new ProductViewModel
            {
                User = user,
                Products = products
            };
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(viewModel);
        }
        [Route("order/submit")]
        // POST: order/cart
        [HttpPost]
        public async Task<IActionResult> OrderCart([FromBody] List<ProductOrder> cartItems)
        {
            // Sprawdzenie, czy cartItems nie jest null ani puste
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("Koszyk jest pusty lub nieprawidłowy.");
            }

            try
            {
                // Sprawdzenie poprawności każdego elementu w cartItems
                foreach (var item in cartItems)
                {
                    if (item.ProductId <= 0 || item.Quantity <= 0 || item.Price <= 0)
                    {
                        return BadRequest($"Błąd w danych produktu: {item.ProductId}");
                    }
                }
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                // Tworzenie nowego zamówienia
                var order = new Order
                {
                    UserId = userId.Value, // Zmienna userId powinna być pobierana z sesji
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cartItems.Sum(item => item.Price * item.Quantity), // Korzystamy z PriceDecimal
                    Items = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList()
                };

                // Dodaj zamówienie do bazy
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Zamówienie zostało złożone!" });
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                return StatusCode(500, $"Wystąpił błąd: {ex.Message}");
            }
        }
        [Route("order/success")]
        // GET: order/success
        [HttpGet]
        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}

