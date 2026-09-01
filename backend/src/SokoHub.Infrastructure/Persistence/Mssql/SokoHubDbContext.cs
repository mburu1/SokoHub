using Microsoft.EntityFrameworkCore;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Domain.Modules.Customers;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Modules.Payments;

namespace SokoHub.Infrastructure.Persistence.Mssql;

public class SokoHubDbContext : DbContext
{
    public SokoHubDbContext(DbContextOptions<SokoHubDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<OAuthAccount> OAuthAccounts => Set<OAuthAccount>();

    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorStore> VendorStores => Set<VendorStore>();
    public DbSet<VendorWallet> VendorWallets => Set<VendorWallet>();
    public DbSet<VendorKyc> VendorKycs => Set<VendorKyc>();
    public DbSet<VendorDocument> VendorDocuments => Set<VendorDocument>();
    public DbSet<VendorSettlement> VendorSettlements => Set<VendorSettlement>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<CustomerPreference> CustomerPreferences => Set<CustomerPreference>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<VendorOrder> VendorOrders => Set<VendorOrder>();
    public DbSet<OrderPayment> OrderPayments => Set<OrderPayment>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<OrderNote> OrderNotes => Set<OrderNote>();

    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
    public DbSet<PaymentCallback> PaymentCallbacks => Set<PaymentCallback>();
    public DbSet<PaymentRefund> PaymentRefunds => Set<PaymentRefund>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentIntent> PaymentIntents => Set<PaymentIntent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Domain Base Configuration
        modelBuilder.Entity<AggregateRoot>().Property(x => x.Version).IsRequired();

        // Identity
        modelBuilder.Entity<User>(builder => {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(120);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.SecurityStamp).IsRequired().HasMaxLength(100);
        });

        // Vendors
        modelBuilder.Entity<Vendor>(builder => {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.BusinessName).IsRequired().HasMaxLength(200);
            builder.Property(v => v.TaxId).IsRequired();
        });

        // Customers
        modelBuilder.Entity<Customer>(builder => {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Email).IsRequired();
            builder.Property(c => c.Phone).IsRequired();
        });

        // Products
        modelBuilder.Entity<Product>(builder => {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Slug).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(8000);
        });

        modelBuilder.Entity<ProductVariant>(builder => {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Sku).IsRequired();
        });

        modelBuilder.Entity<ProductImage>(builder => {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Url).IsRequired();
        });

        modelBuilder.Entity<Category>(builder => {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Slug).IsRequired();
        });

        modelBuilder.Entity<Brand>(builder => {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Slug).IsRequired();
        });
    }
}
