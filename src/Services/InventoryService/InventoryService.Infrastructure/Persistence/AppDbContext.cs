using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace InventoryService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemWearLog> ItemWearLogs => Set<ItemWearLog>();
    public DbSet<BarcodeProductCache> BarcodeProductCaches => Set<BarcodeProductCache>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Tüm DateTime alanları otomatik olarak UTC kabul edilsin.
        // (Npgsql, "timestamp with time zone" kolonlarına Kind=Unspecified bir DateTime yazılmasına izin vermiyor;
        // bu converter sayesinde her DateTime/DateTime? property için ayrı ayrı SpecifyKind çağırmaya gerek kalmıyor.)
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Category ---
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.IconUrl).HasMaxLength(2000);

            // Self-referencing: bir kategori, başka bir kategoriyi parent olarak gösterebilir.
            // Restrict: parent kategori silinmek istenirse, altında kategori varsa silmeyi engelle
            // (Cascade olsaydı, bir ana kategoriyi silmek tüm alt kategorileri de silerdi - istemiyoruz).
            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Brand ---
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(100);
            entity.Property(b => b.LogoUrl).HasMaxLength(2000);
        });

        // --- Item ---
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(200);
            entity.Property(i => i.Color).HasMaxLength(50);
            entity.Property(i => i.Size).HasMaxLength(20);
            entity.Property(i => i.PurchasePrice).HasColumnType("decimal(10,2)");
            entity.Property(i => i.PurchaseSource).HasMaxLength(100);
            entity.Property(i => i.Barcode).HasMaxLength(100);
            entity.Property(i => i.ImageUrl).HasMaxLength(2000);
            entity.Property(i => i.Notes).HasMaxLength(2000);

            // Enum'lar veritabanında int olarak değil, okunabilir string olarak saklansın.
            // (Migration sonradan "0,1,2" yerine "New,Good,Fair" görmek debug'ı kolaylaştırır.)
            entity.Property(i => i.Condition).HasConversion<string>().HasMaxLength(20);
            entity.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

            // UserId'nin FK olmadığını context'te de açıkça belirtelim (sadece bir alan, AuthService'e referans yok).
            entity.Property(i => i.UserId).IsRequired();

            // CategoryId zorunlu FK; Brand opsiyonel FK.
            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);   // Kategori silinemesin, altında Item varsa

            entity.HasOne<Brand>()
                .WithMany()
                .HasForeignKey(i => i.BrandId)
                .OnDelete(DeleteBehavior.SetNull);     // Marka silinirse, Item'daki BrandId null'a düşsün
        });

        // --- ItemWearLog ---
        modelBuilder.Entity<ItemWearLog>(entity =>
        {
            entity.HasKey(w => w.Id);

            // UserId burada da FK değil, sadece bir alan (mikroservis sınırı - AuthService'e referans yok).
            entity.Property(w => w.UserId).IsRequired();

            // ItemId gerçek bir FK, Items tablosuna bağlı.
            // Cascade: bir Item silinirse (ileride silme özelliği eklenirse), o item'a ait
            // tüm giyilme kayıtları da silinsin - log'lar item'sız anlamsız kalır.
            entity.HasOne<Item>()
                .WithMany()
                .HasForeignKey(w => w.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- BarcodeProductCache ---
        modelBuilder.Entity<BarcodeProductCache>(entity =>
        {
            // Bu tabloda auto-increment bir Id yok; Barcode'un kendisi Primary Key.
            // Aynı barkod birden fazla kez cache'lenemez - varsa güncellenir (Refresh metodu ile).
            entity.HasKey(b => b.Barcode);

            entity.Property(b => b.Barcode).HasMaxLength(100);
            entity.Property(b => b.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(b => b.BrandName).HasMaxLength(100);
            entity.Property(b => b.CategoryHint).HasMaxLength(100);
            entity.Property(b => b.ImageUrl).HasMaxLength(2000);

            // Bu tablonun Items/Categories/Brands ile hiçbir FK ilişkisi yok - bağımsız bir cache tablosu.
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