using Microsoft.EntityFrameworkCore;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<NumberSequence> NumberSequences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= PRODUCT =================
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryId)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.VatRate)
                .HasPrecision(5, 2);

            // ================= PRODUCT CATEGORY =================
            modelBuilder.Entity<ProductCategory>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<ProductCategory>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(64);

            modelBuilder.Entity<ProductCategory>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // ================= QUOTATION =================
            modelBuilder.Entity<Quotation>()
                .HasMany(q => q.Items)
                .WithOne(i => i.Quotation)
                .HasForeignKey(i => i.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.VatRate)
                .HasPrecision(5, 2);

            // ================= ORDER =================
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.VatRate)
                .HasPrecision(5, 2);

            // ================= INVOICE =================
            modelBuilder.Entity<Invoice>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Invoice)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.VatRate)
                .HasPrecision(5, 2);

            // ================= PAYMENT =================
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================= NUMBER SEQUENCE =================

            modelBuilder.Entity<NumberSequence>()
                .HasIndex(x => new
                {
                    x.DocumentType,
                    x.Year
                })
                .IsUnique();

            // ================= DOCUMENT NUMBERS =================

            modelBuilder.Entity<Invoice>()
                .HasIndex(x => x.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(x => x.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Quotation>()
                .HasIndex(x => x.QuotationNumber)
                .IsUnique();
        }
    }
}