using Microsoft.EntityFrameworkCore;
using InvoiceEntry.Models;

namespace InvoiceEntry.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Record> Records { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
