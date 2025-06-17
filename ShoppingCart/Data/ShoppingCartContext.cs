using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
// No explicit using Microsoft.EntityFrameworkCore.Metadata.Builders; needed for this simpler approach
// using Microsoft.EntityFrameworkCore.Relational; // Potentially for ToTable if it were used directly

namespace ShoppingCart.Data
{
    public class ShoppingCartContext : DbContext
    {
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship between Cart and Product.
            // EF Core will create a join table. We can suggest a name.
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Products)
                .WithMany() // No navigation property back from Product to Cart is specified in Product.cs
                .UsingEntity("CartProducts"); // Simplified: Just name the join table.
                                            // EF Core will define PKs for the join table based on conventions.
        }
    }
}
