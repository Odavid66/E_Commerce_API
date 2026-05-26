using Microsoft.EntityFrameworkCore;
using E_Commerce_API.Entities;

namespace E_Commerce_API.Data
{
    // Inheriting from DbContext is what gives this class its database superpowers
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        // Define your DbSets (tables) here
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        

        // You can configure relationships and constraints here if needed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Example: modelBuilder.Entity<User>().HasKey(u => u.Id);

            // Configure decimal properties with precision and scale
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(pr => pr.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //// ── NEW: Order → User ──────────────────────────────
            //modelBuilder.Entity<Order>()
            //    .HasOne(o => o.User)
            //    // one order belongs to one user

            //    .WithMany(u => u.Orders)
            //    // one user has many orders

            //    .HasForeignKey(o => o.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);
            //// if user is deleted → their orders are deleted

            //// ── NEW: Order → TotalAmount precision ────────────
            //modelBuilder.Entity<Order>()
            //    .Property(o => o.TotalAmount)
            //    .HasColumnType("decimal(18,2)");

            //// ── NEW: OrderItem → Order ─────────────────────────
            //modelBuilder.Entity<OrderItem>()
            //    .HasOne(oi => oi.Order)
            //    // one order item belongs to one order

            //    .WithMany(o => o.OrderItems)
            //    // one order has many order items

            //    .HasForeignKey(oi => oi.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade);
            //// if order is deleted → its items are deleted

            //// ── NEW: OrderItem → Product ───────────────────────
            //modelBuilder.Entity<OrderItem>()
            //    .HasOne(oi => oi.Product)
            //    .WithMany()
            //    // ↑
            //    // Product does not need a navigation property back
            //    // to OrderItems — we use WithMany() with no argument
            //    .HasForeignKey(oi => oi.ProductId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //// ↑
            //// Restrict — cannot delete a product that has order history
            //// important for keeping order records intact

            //// ── NEW: OrderItem UnitPrice precision ────────────
            //modelBuilder.Entity<OrderItem>()
            //    .Property(oi => oi.UnitPrice)
            //    .HasColumnType("decimal(18,2)");

            //// ── NEW: Payment → Order (one to one) ─────────────
            //modelBuilder.Entity<Payment>()
            //    .HasOne(p => p.Order)
            //    // one payment belongs to one order

            //    .WithOne(o => o.Payment)
            //    // one order has one payment

            //    .HasForeignKey<Payment>(p => p.OrderId)
            //    // ↑
            //    // foreign key is on the Payment side
            //    .OnDelete(DeleteBehavior.Cascade);
            //// if order is deleted → its payment is deleted

            //// ── NEW: Payment Amount precision ─────────────────
            //modelBuilder.Entity<Payment>()
            //    .Property(p => p.Amount)
            //    .HasColumnType("decimal(18,2)");

            //base.OnModelCreating(modelBuilder);
        }
    }
}
