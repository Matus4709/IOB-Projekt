namespace UserManagmentApp.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }          // Zamówienie
        public User User { get; set; }            // Użytkownik
        public List<OrderItem> OrderItems { get; set; }  // Produkty w zamówieniu
    }
}
