using Maildesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Data;

public class MaildeskDbContext : DbContext
{
    public MaildeskDbContext(DbContextOptions<MaildeskDbContext> options) : base(options)
    {
    }

    public DbSet<SuratMasuk> SuratMasuk => Set<SuratMasuk>();
    public DbSet<SuratKeluar> SuratKeluar => Set<SuratKeluar>();

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
            entity.HasIndex(x => x.NomorAgenda);
            entity.HasIndex(x => x.TanggalDiterima);
            entity.HasIndex(x => new { x.TanggalDiterima, x.NomorAgenda });
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.JenisSumber);
        });
                modelBuilder.Entity<SuratKeluar>(entity =>
        {
            entity.ToTable("surat_keluar");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.NomorAgenda).HasColumnName("nomor_agenda");
            entity.Property(x => x.NomorSurat).HasColumnName("nomor_surat");
            entity.Property(x => x.TanggalSurat).HasColumnName("tanggal_surat");
            entity.Property(x => x.TujuanSurat).HasColumnName("tujuan_surat");
            entity.Property(x => x.InstansiTujuan).HasColumnName("instansi_tujuan");
            entity.Property(x => x.Perihal).HasColumnName("perihal");
            entity.Property(x => x.IsiRingkas).HasColumnName("isi_ringkas");
            entity.Property(x => x.KodeKlasifikasi).HasColumnName("kode_klasifikasi");
            entity.Property(x => x.TingkatPrioritas).HasColumnName("tingkat_prioritas");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.DibuatOleh).HasColumnName("dibuat_oleh");
            entity.Property(x => x.DiperbaruiOleh).HasColumnName("diperbarui_oleh");
            entity.Property(x => x.DibuatPada).HasColumnName("dibuat_pada");
            entity.Property(x => x.DiperbaruiPada).HasColumnName("diperbarui_pada");

            entity.HasIndex(x => x.NomorAgenda);
            entity.HasIndex(x => x.NomorSurat);
            entity.HasIndex(x => x.TanggalSurat);
            entity.HasIndex(x => new { x.TanggalSurat, x.NomorAgenda });
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.TujuanSurat);
        });
    }
}