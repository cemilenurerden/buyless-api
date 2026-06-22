using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;
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
    }
}