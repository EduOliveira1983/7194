using Microsoft.EntityFrameworkCore;
using Shop.Model;

namespace Shop.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {   
        }

        public DbSet<Product> Products {get; set;}
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        
    }
}