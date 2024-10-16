using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagmentApp.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; }
        public string Description { get; set; }
        [MaxLength(255)]
        public string Category { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastUpdated { get; set; }

        public bool IsDiscontinued { get; set; } = false;
        [MaxLength(255)]
        public string ImagePath { get; set; } 
    }
}
