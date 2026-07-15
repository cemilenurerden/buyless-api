using MarketplaceService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketplaceService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ListingImage> ListingImages => Set<ListingImage>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SellerReview> SellerReviews => Set<SellerReview>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Listing ---
        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Title).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Description).HasMaxLength(2000);
            entity.Property(l => l.Price).HasColumnType("decimal(10,2)");
            entity.Property(l => l.Currency).IsRequired().HasMaxLength(3);
            entity.Property(l => l.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(l => l.Condition).HasConversion<string>().HasMaxLength(20);

            // SellerId, ItemId gerçek FK değil — mikroservis sınırı
            entity.Property(l => l.SellerId).IsRequired();

            // _images backing field üzerinden Images koleksiyonu
            entity.Navigation(l => l.Images)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            
        });

        // --- ListingImage ---
        modelBuilder.Entity<ListingImage>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ImageUrl).IsRequired().HasMaxLength(2000);

            entity.HasOne(i => i.Listing)
                .WithMany(l => l.Images)
                .HasForeignKey(i => i.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Offer ---
        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OfferedPrice).HasColumnType("decimal(10,2)");
            entity.Property(o => o.Message).HasMaxLength(1000);
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(o => o.Listing)
                .WithMany(l => l.Offers)
                .HasForeignKey(o => o.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Order ---
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Amount).HasColumnType("decimal(10,2)");
            entity.Property(o => o.Currency).IsRequired().HasMaxLength(3);
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(o => o.ShippingAddress).HasMaxLength(500);
            entity.Property(o => o.TrackingNumber).HasMaxLength(100);

            entity.HasOne(o => o.Listing)
                .WithMany(l => l.Orders)
                .HasForeignKey(o => o.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // --- Payment (placeholder) ---
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Amount).HasColumnType("decimal(10,2)");
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(p => p.Method).HasConversion<string>().HasMaxLength(20);
            entity.Property(p => p.IyzicoPaymentId).HasMaxLength(200);
            entity.Property(p => p.IyzicoToken).HasMaxLength(500);

            entity.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                    });


        // --- SellerReview ---
        modelBuilder.Entity<SellerReview>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Rating).IsRequired();
            entity.Property(r => r.Comment).HasMaxLength(1000);

            entity.HasOne(r => r.Order)
                .WithOne(o => o.Review)
                .HasForeignKey<SellerReview>(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Message ---
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).IsRequired().HasMaxLength(2000);

            entity.HasOne(m => m.Listing)
                .WithMany()
                .HasForeignKey(m => m.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter() : base(
        v => v.HasValue
            ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc))
            : v,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
    {
    }
}