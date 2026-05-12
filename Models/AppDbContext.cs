using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.Api.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Disposisi> Disposisis { get; set; }

    public virtual DbSet<DisposisiRelation> DisposisiRelations { get; set; }

    public virtual DbSet<Inbox> Inboxes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Surat> Surats { get; set; }

    public virtual DbSet<TemplateSurat> TemplateSurats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=maildesk_db;Username=postgres;Password=postgreselys29");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disposisi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("disposisi_pkey");

            entity.ToTable("disposisi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Instruksi).HasColumnName("instruksi");
            entity.Property(e => e.PemberiId).HasColumnName("pemberi_id");
            entity.Property(e => e.PenerimaId).HasColumnName("penerima_id");
            entity.Property(e => e.SifatDisposisi)
                .HasMaxLength(50)
                .HasColumnName("sifat_disposisi");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SuratId).HasColumnName("surat_id");
            entity.Property(e => e.TanggalDisposisi).HasColumnName("tanggal_disposisi");

            entity.HasOne(d => d.Pemberi).WithMany(p => p.DisposisiPemberis)
                .HasForeignKey(d => d.PemberiId)
                .HasConstraintName("disposisi_pemberi_id_fkey");

            entity.HasOne(d => d.Penerima).WithMany(p => p.DisposisiPenerimas)
                .HasForeignKey(d => d.PenerimaId)
                .HasConstraintName("disposisi_penerima_id_fkey");

            entity.HasOne(d => d.Surat).WithMany(p => p.Disposisis)
                .HasForeignKey(d => d.SuratId)
                .HasConstraintName("disposisi_surat_id_fkey");
        });

        modelBuilder.Entity<DisposisiRelation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("disposisi_relation_pkey");

            entity.ToTable("disposisi_relation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");

            entity.HasOne(d => d.Child).WithMany(p => p.DisposisiRelationChildren)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("disposisi_relation_child_id_fkey");

            entity.HasOne(d => d.Parent).WithMany(p => p.DisposisiRelationParents)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("disposisi_relation_parent_id_fkey");
        });

        modelBuilder.Entity<Inbox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inbox_pkey");

            entity.ToTable("inbox");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CatatanPengantar).HasColumnName("catatan_pengantar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PenerimaId).HasColumnName("penerima_id");
            entity.Property(e => e.PengirimId).HasColumnName("pengirim_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Belum Dibaca'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SuratId).HasColumnName("surat_id");

            entity.HasOne(d => d.Penerima).WithMany(p => p.InboxPenerimas)
                .HasForeignKey(d => d.PenerimaId)
                .HasConstraintName("inbox_penerima_id_fkey");

            entity.HasOne(d => d.Pengirim).WithMany(p => p.InboxPengirims)
                .HasForeignKey(d => d.PengirimId)
                .HasConstraintName("inbox_pengirim_id_fkey");

            entity.HasOne(d => d.Surat).WithMany(p => p.Inboxes)
                .HasForeignKey(d => d.SuratId)
                .HasConstraintName("inbox_surat_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NamaRole)
                .HasMaxLength(50)
                .HasColumnName("nama_role");
        });

        modelBuilder.Entity<Surat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("surat_pkey");

            entity.ToTable("surat");

            entity.HasIndex(e => e.NomorAgenda, "surat_nomor_agenda_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FileLampiran).HasColumnName("file_lampiran");
            entity.Property(e => e.IsArchived)
                .HasDefaultValue(false)
                .HasColumnName("is_archived");
            entity.Property(e => e.IsiTeksOcr).HasColumnName("isi_teks_ocr");
            entity.Property(e => e.JenisSurat)
                .HasMaxLength(20)
                .HasColumnName("jenis_surat");
            entity.Property(e => e.KategoriSurat)
                .HasMaxLength(50)
                .HasColumnName("kategori_surat");
            entity.Property(e => e.NamaFile)
                .HasMaxLength(255)
                .HasColumnName("nama_file");
            entity.Property(e => e.NoSurat)
                .HasMaxLength(100)
                .HasColumnName("no_surat");
            entity.Property(e => e.NomorAgenda)
                .HasMaxLength(100)
                .HasColumnName("nomor_agenda");
            entity.Property(e => e.Penerima)
                .HasMaxLength(150)
                .HasColumnName("penerima");
            entity.Property(e => e.Pengirim)
                .HasMaxLength(150)
                .HasColumnName("pengirim");
            entity.Property(e => e.Perihal).HasColumnName("perihal");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TanggalSurat).HasColumnName("tanggal_surat");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Surats)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("surat_user_id_fkey");
        });

        modelBuilder.Entity<TemplateSurat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("template_surat_pkey");

            entity.ToTable("template_surat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DibuatOleh).HasColumnName("dibuat_oleh");
            entity.Property(e => e.IsiTemplate).HasColumnName("isi_template");
            entity.Property(e => e.NamaTemplate)
                .HasMaxLength(100)
                .HasColumnName("nama_template");

            entity.HasOne(d => d.DibuatOlehNavigation).WithMany(p => p.TemplateSurats)
                .HasForeignKey(d => d.DibuatOleh)
                .HasConstraintName("template_surat_dibuat_oleh_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nama)
                .HasMaxLength(100)
                .HasColumnName("nama");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
