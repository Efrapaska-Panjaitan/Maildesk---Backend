using MailDesk.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SuratMasuk> SuratMasuks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Disposisi> Disposisis { get; set; }
    public DbSet<TrackingDisposisi> TrackingDisposisis { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraint untuk nomor_agenda
        modelBuilder.Entity<SuratMasuk>()
            .HasIndex(s => s.NomorAgenda)
            .IsUnique()
            .HasFilter("nomor_agenda IS NOT NULL"); // partial index, null tetap boleh multiple

        // Relasi SuratMasuk -> User (pencatat)
        modelBuilder.Entity<SuratMasuk>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}