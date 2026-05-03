using Maildesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Data;

public class MaildeskDbContext : DbContext
{
    public MaildeskDbContext(DbContextOptions<MaildeskDbContext> options) : base(options) { }

    public DbSet<SuratMasuk> SuratMasuk => Set<SuratMasuk>();
    public DbSet<SuratKeluar> SuratKeluar => Set<SuratKeluar>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SuratMasuk>(entity =>
        {
            entity.ToTable("surat_masuk");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.NoSurat).HasColumnName("no_surat").HasMaxLength(100).IsRequired();
            entity.Property(x => x.NomorAgenda).HasColumnName("nomor_agenda").HasMaxLength(100);
            entity.Property(x => x.TanggalSurat).HasColumnName("tanggal_surat").IsRequired();
            entity.Property(x => x.AsalPengirim).HasColumnName("asal_pengirim").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Perihal).HasColumnName("perihal").HasColumnType("text").IsRequired();
            entity.Property(x => x.FileLampiran).HasColumnName("file_lampiran").HasColumnType("bytea");
            entity.Property(x => x.NamaFile).HasColumnName("nama_file").HasMaxLength(255);
            entity.Property(x => x.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(x => x.NomorAgenda).IsUnique();
            entity.HasIndex(x => x.TanggalSurat);
            entity.HasIndex(x => x.AsalPengirim);
            entity.HasIndex(x => x.IsArchived);
        });

        modelBuilder.Entity<SuratKeluar>(entity =>
        {
            entity.ToTable("surat_keluar");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.NoSurat).HasColumnName("no_surat").HasMaxLength(100);
            entity.Property(x => x.NomorAgenda).HasColumnName("nomor_agenda").HasMaxLength(100);
            entity.Property(x => x.TanggalSurat).HasColumnName("tanggal_surat");
            entity.Property(x => x.Kepada).HasColumnName("kepada").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Perihal).HasColumnName("perihal").HasColumnType("text").IsRequired();
            entity.Property(x => x.FileLampiran).HasColumnName("file_lampiran").HasColumnType("bytea");
            entity.Property(x => x.NamaFile).HasColumnName("nama_file").HasMaxLength(255);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(50).HasDefaultValue("Draft");
            entity.Property(x => x.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            entity.Property(x => x.PembuatId).HasColumnName("pembuat_id");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(x => x.NomorAgenda).IsUnique();
            entity.HasIndex(x => x.NoSurat);
            entity.HasIndex(x => x.TanggalSurat);
            entity.HasIndex(x => new { x.TanggalSurat, x.NomorAgenda });
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.Kepada);
            entity.HasIndex(x => x.IsArchived);
        });
    }
}