using System.ComponentModel.DataAnnotations;

namespace UserManagmentApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        [Required]
        [StringLength(100)]
        public string UserType { get; set; } = "User";
        [Required]
        [StringLength(12)]
        public string NIP { get; set; }
        [Required]
        [StringLength(6)]
        public string KodPocztowy { get; set; }
        [Required]
        [StringLength(100)]
        public string Miejscowosc { get; set; }
        [Required]
        [StringLength(100)]
        public string Adres { get; set; }
        [Required]
        [StringLength(12)]
        public String Telefon { get; set; }
        [Required]
        [StringLength(100)]
        public string NazwaFirmy { get; set; }
    }
}
