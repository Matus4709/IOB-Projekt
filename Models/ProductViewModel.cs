namespace UserManagmentApp.Models
{
    public class ProductViewModel
    {
        public User User { get; set; }
        public IEnumerable<Products> Products { get; set; }

        // Paginacja
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public string SearchQuery { get; set; }

        public string SelectedCategory { get; set; } // Przechowuje wybraną kategorię
        public List<string> Categories { get; set; } // Przechowuje dostępne kategorie

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
