using Microsoft.EntityFrameworkCore;
using UserManagmentApp.Models;

namespace UserManagmentApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; } 
    }
}
