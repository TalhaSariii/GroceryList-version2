using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace GroceryList.Models
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=localhost\\SQLEXPRESS; database=TodoList; integrated security=true;TrustServerCertificate=True");
        }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
        }

    }
}
