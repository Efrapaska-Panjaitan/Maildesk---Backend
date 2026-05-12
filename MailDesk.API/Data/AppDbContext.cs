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

        //=========================================
        // Surat 
        //=========================================
        
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

        // Relasi Surat -> User (pencatat — TU/Sekretaris yang menginput)
        modelBuilder.Entity<Surat>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relasi Surat → User (ditujukan ke — Pimpinan tujuan disposisi)
        modelBuilder.Entity<Surat>()
            .HasOne(s => s.DitujukanKe)
            .WithMany()
            .HasForeignKey(s => s.DitujukanKeId)
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
        
         // Satu child hanya boleh punya satu parent
        modelBuilder.Entity<DisposisiRelation>()
            .HasIndex(dr => dr.ChildId)
            .IsUnique();
        
        //===========================================
        //Disposisi
        //===========================================

        // Check constraint sifat disposisi
        modelBuilder.Entity<Disposisi>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_disposisi_sifat",
                "sifat_disposisi IN ('Biasa', 'Penting', 'Mendesak', 'Rahasia')"));

        modelBuilder.Entity<Disposisi>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_disposisi_status",
                "status IN ('Pending', 'Accepted', 'Completed')"));

        // Relasi Disposisi → Surat
        modelBuilder.Entity<Disposisi>()
            .HasOne(d => d.Surat)
            .WithMany()
            .HasForeignKey(d => d.SuratId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Disposisi → Pemberi (User)
        modelBuilder.Entity<Disposisi>()
            .HasOne(d => d.Pemberi)
            .WithMany()
            .HasForeignKey(d => d.PemberiId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Disposisi → Penerima (User)
        modelBuilder.Entity<Disposisi>()
            .HasOne(d => d.Penerima)
            .WithMany()
            .HasForeignKey(d => d.PenerimaId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}