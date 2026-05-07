using MailDesk.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Surat> Surats { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TemplateSurat> TemplateSurats { get; set; }
    public DbSet<Inbox> Inboxes { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Disposisi> Disposisis { get; set; }
    public DbSet<DisposisiRelation> DisposisiRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraint untuk nomor_agenda
        modelBuilder.Entity<Surat>()
            .HasIndex(s => s.NomorAgenda)
            .IsUnique()
            .HasFilter("nomor_agenda IS NOT NULL"); // partial index, null tetap boleh multiple

         // Check constraint jenis_surat
        modelBuilder.Entity<Surat>()
        .ToTable(t => t.HasCheckConstraint(
            "CK_surat_jenis",
            "jenis_surat IN ('Masuk', 'Keluar')"));
        
        // Unique email di users
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Relasi Surat -> User (pencatat)
        modelBuilder.Entity<Surat>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Relasi DisposisiRelation → Disposisi (parent)
        modelBuilder.Entity<DisposisiRelation>()
            .HasOne(dr => dr.Parent)
            .WithMany()
            .HasForeignKey(dr => dr.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi DisposisiRelation → Disposisi (child)
        modelBuilder.Entity<DisposisiRelation>()
            .HasOne(dr => dr.Child)
            .WithMany()
            .HasForeignKey(dr => dr.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}