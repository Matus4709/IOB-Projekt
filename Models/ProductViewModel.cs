namespace UserManagmentApp.Models
{
    public class ProductViewModel
{
        public User User {  get; set; }
        public IEnumerable<Products> products { get; set; }
}
}
