using CodeVaultMVC.Models;
using CodeVaultMVC.Models.MVC_Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Bu namespace gerekli
using Microsoft.EntityFrameworkCore;

namespace CodeVaultAPI.Models.Data
{
    // IdentityDbContext<Users> sınıfından türetiyoruz
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Tasks ve Comments tabloların aynen kalıyor
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Comments> Comments { get; set; }

        public DbSet<Projects> Projects { get; set; }
        public DbSet<Technologies> Technologies { get; set; }
        public DbSet<Developers> Developers { get; set; } // Varsa

        // Admin tablosu artık Identity ile yönetilecek
        public DbSet<Admin> Admins { get; set; }
    }
}