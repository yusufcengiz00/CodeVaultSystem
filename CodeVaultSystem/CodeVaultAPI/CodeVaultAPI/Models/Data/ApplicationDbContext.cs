using Microsoft.EntityFrameworkCore;

namespace CodeVaultAPI.Models.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
         {
            
        }

        public DbSet<Developers> Developers { get; set; }

        public DbSet<Projects> Projects { get; set; }

        public DbSet<Technologies> Technologies { get; set; }


    }
}
