using BudgetService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BudgetService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BudgetActivity> BudgetActivities => Set<BudgetActivity>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // InventoryService'te de kullandığımız aynı pattern:
        // Npgsql, "timestamp with time zone" kolonlarına Kind=Unspecified DateTime yazılmasına izin vermiyor.
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BudgetActivity>(entity =>
        {
            entity.HasKey(a => a.Id);

            // UserId, ItemId, CategoryId hiçbiri gerçek FK değil -
            // mikroservis sınırı: AuthService ve InventoryService'e referans yok, sadece bilgi alanları.
            entity.Property(a => a.UserId).IsRequired();

            entity.Property(a => a.Amount).HasColumnType("decimal(10,2)");
            entity.Property(a => a.Currency).IsRequired().HasMaxLength(3);
            entity.Property(a => a.Platform).HasMaxLength(100);
            entity.Property(a => a.Description).HasMaxLength(2000);

            // Enum veritabanında okunabilir string olarak saklansın (diğer servislerle tutarlı).
            entity.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);

            // OccurredAt DateOnly tipinde - Npgsql "date" kolonuna doğal olarak eşleniyor.
            entity.Property(a => a.OccurredAt).HasColumnType("date");
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