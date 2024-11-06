namespace UserManagmentApp.Models
{
    public class ProductOrder
    {
        public int ProductId { get; set; }   // Id produktu
        public string Name { get; set; }      // Nazwa produktu
        public int Quantity { get; set; }     // Ilość zamówiona
        public decimal Price { get; set; }    // Cena jednostkowa
    }
}
