using Microsoft.EntityFrameworkCore;
using ProductCategoryCrud.Models;
using Microsoft.AspNetCore.Mvc;
using ProductCategoryCrud.Data;

namespace ProductCategoryCrud.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(p => p.Category);
        }*/
    }
}


