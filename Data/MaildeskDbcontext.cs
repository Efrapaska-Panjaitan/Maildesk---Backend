using Maildesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Data;

public class MaildeskDbContext : DbContext
{
    public MaildeskDbContext(DbContextOptions<MaildeskDbContext> options) : base(options)
    {
    }

    public DbSet<SuratMasuk> SuratMasuk => Set<SuratMasuk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SuratMasuk>(entity =>
        {
            entity.ToTable("surat_masuk");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.NomorAgenda).HasColumnName("nomor_agenda");
            entity.Property(x => x.NomorSurat).HasColumnName("nomor_surat");
            entity.Property(x => x.TanggalSurat).HasColumnName("tanggal_surat");
            entity.Property(x => x.TanggalDiterima).HasColumnName("tanggal_diterima");
            entity.Property(x => x.NamaPengirim).HasColumnName("nama_pengirim");
            entity.Property(x => x.InstansiPengirim).HasColumnName("instansi_pengirim");
            entity.Property(x => x.Perihal).HasColumnName("perihal");
            entity.Property(x => x.Deskripsi).HasColumnName("deskripsi");
            entity.Property(x => x.KodeKlasifikasi).HasColumnName("kode_klasifikasi");
            entity.Property(x => x.JenisSumber).HasColumnName("jenis_sumber");
            entity.Property(x => x.TingkatPrioritas).HasColumnName("tingkat_prioritas");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.DibuatPada).HasColumnName("dibuat_pada");
        });
    }
}