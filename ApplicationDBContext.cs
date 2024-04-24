using Microsoft.EntityFrameworkCore;
using NETCORE.Models;
namespace NETCORE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<PBI_CATEGORY> PBI_CATEGORY { get; set; }
    }
}
