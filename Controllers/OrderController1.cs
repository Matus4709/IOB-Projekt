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
                        return BadRequest($"Błąd w danych produktu: {item.ProductId}. Upewnij się, że wszystkie dane są prawidłowe.");
                    }
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return BadRequest("Nie znaleziono użytkownika w bazie danych.");
                }

                // Tworzenie nowego zamówienia
                var order = new Order
                {
                    UserId = userId.Value,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cartItems.Sum(item => item.Price * item.Quantity),
                    Items = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList()
                };

                // Sprawdzenie dostępności produktów i aktualizacja stanu magazynowego
                foreach (var item in cartItems)
                {
                    // Znajdź produkt w bazie danych
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                    if (product == null)
                    {
                        return BadRequest($"Produkt o ID {item.ProductId} nie istnieje.");
                    }

                    // Sprawdzenie, czy stan magazynowy jest wystarczający
                    if (product.StockQuantity < item.Quantity)
                    {
                        return BadRequest($"Produkt o ID {item.ProductId} ma niewystarczający stan magazynowy. Dostępna ilość: {product.StockQuantity}, zamówiona ilość: {item.Quantity}.");
                    }

                    // Zmniejsz stan magazynowy o zamówioną ilość
                    product.StockQuantity -= item.Quantity;
                    product.LastUpdated = DateTime.UtcNow; // Aktualizacja daty modyfikacji
                }

                // Dodaj zamówienie do bazy
                _context.Orders.Add(order);

                // Zapisz zmiany w tabeli Products i Orders
                await _context.SaveChangesAsync();

                return Ok(new { message = "Zamówienie zostało złożone!" });
            }
            catch (Exception ex)
            {
                // Logowanie szczegółów błędu
                Console.WriteLine($"Błąd: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Zwrócenie szczegółów błędu w odpowiedzi
                return StatusCode(500, $"Wystąpił błąd: {ex.Message} {ex.InnerException?.Message}");
            }
        }
        [Route("order/success")]
        // GET: order/success
        [HttpGet]
        public IActionResult OrderSuccess()
        {
            return View();
        }
        [Route("order/list")]
        public IActionResult OrderList()
        {
            // Pobranie wszystkich zamówień z bazy danych
            var orders = _context.Orders.ToList();

            // Przekazanie listy zamówień do widoku
            return View(orders);
        }
        [Route("order/details/{orderId}")]
        public IActionResult OrderDetails(int orderId)
        {
            // Pobranie szczegółów zamówienia
            var order = _context.Orders
                                .Where(o => o.OrderId == orderId)
                                .FirstOrDefault();

            if (order == null)
            {
                return NotFound(); // Zwrócenie 404, jeśli zamówienie nie istnieje
            }

            // Pobranie danych użytkownika powiązanego z zamówieniem
            var user = _context.Users
                               .Where(u => u.Id == order.UserId)
                               .FirstOrDefault();

            // Pobranie szczegółów produktów w zamówieniu
            var orderItems = _context.OrderItems
                                     .Where(oi => oi.OrderId == orderId)
                                     .Include(oi => oi.Products) // Pobieranie danych o produktach
                                     .ToList();

            // Przygotowanie modelu widoku z wszystkimi danymi
            var viewModel = new OrderDetailsViewModel
            {
                Order = order,               // Zamówienie
                User = user,                 // Użytkownik
                OrderItems = orderItems      // Produkty w zamówieniu
            };

            return View(viewModel);
        }

    }
}

