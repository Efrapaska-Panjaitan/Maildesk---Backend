using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MailDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDitujukanKeIdToSurat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nama_role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nama = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "surat",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    no_surat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    nomor_agenda = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    jenis_surat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    kategori_surat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tanggal_surat = table.Column<DateOnly>(type: "date", nullable: false),
                    pengirim = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    penerima = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    perihal = table.Column<string>(type: "text", nullable: false),
                    isi_teks_ocr = table.Column<string>(type: "text", nullable: true),
                    file_lampiran = table.Column<byte[]>(type: "bytea", nullable: true),
                    nama_file = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    ditujukan_ke_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_surat", x => x.id);
                    table.CheckConstraint("CK_surat_jenis", "jenis_surat IN ('Masuk', 'Keluar')");
                    table.ForeignKey(
                        name: "FK_surat_users_ditujukan_ke_id",
                        column: x => x.ditujukan_ke_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_surat_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "template_surat",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nama_template = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isi_template = table.Column<string>(type: "text", nullable: false),
                    dibuat_oleh = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_surat", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_surat_users_dibuat_oleh",
                        column: x => x.dibuat_oleh,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "disposisi",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    surat_id = table.Column<int>(type: "integer", nullable: false),
                    pemberi_id = table.Column<int>(type: "integer", nullable: false),
                    penerima_id = table.Column<int>(type: "integer", nullable: false),
                    tanggal_disposisi = table.Column<DateOnly>(type: "date", nullable: false),
                    sifat_disposisi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    instruksi = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    JenisAksi = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    waktu_diterima = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disposisi", x => x.id);
                    table.CheckConstraint("CK_disposisi_sifat", "sifat_disposisi IN ('Biasa', 'Penting', 'Mendesak', 'Rahasia')");
                    table.CheckConstraint("CK_disposisi_status", "status IN ('Pending', 'Accepted', 'Completed')");
                    table.ForeignKey(
                        name: "FK_disposisi_surat_surat_id",
                        column: x => x.surat_id,
                        principalTable: "surat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_disposisi_users_pemberi_id",
                        column: x => x.pemberi_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_disposisi_users_penerima_id",
                        column: x => x.penerima_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inbox",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    surat_id = table.Column<int>(type: "integer", nullable: true),
                    pengirim_id = table.Column<int>(type: "integer", nullable: true),
                    penerima_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    catatan_pengantar = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox", x => x.id);
                    table.ForeignKey(
                        name: "FK_inbox_surat_surat_id",
                        column: x => x.surat_id,
                        principalTable: "surat",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_inbox_users_penerima_id",
                        column: x => x.penerima_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_inbox_users_pengirim_id",
                        column: x => x.pengirim_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "disposisi_relation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    child_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disposisi_relation", x => x.id);
                    table.ForeignKey(
                        name: "FK_disposisi_relation_disposisi_child_id",
                        column: x => x.child_id,
                        principalTable: "disposisi",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_disposisi_relation_disposisi_parent_id",
                        column: x => x.parent_id,
                        principalTable: "disposisi",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_disposisi_pemberi_id",
                table: "disposisi",
                column: "pemberi_id");

            migrationBuilder.CreateIndex(
                name: "IX_disposisi_penerima_id",
                table: "disposisi",
                column: "penerima_id");

            migrationBuilder.CreateIndex(
                name: "IX_disposisi_surat_id",
                table: "disposisi",
                column: "surat_id");

            migrationBuilder.CreateIndex(
                name: "IX_disposisi_relation_child_id",
                table: "disposisi_relation",
                column: "child_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_disposisi_relation_parent_id",
                table: "disposisi_relation",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_inbox_penerima_id",
                table: "inbox",
                column: "penerima_id");

            migrationBuilder.CreateIndex(
                name: "IX_inbox_pengirim_id",
                table: "inbox",
                column: "pengirim_id");

            migrationBuilder.CreateIndex(
                name: "IX_inbox_surat_id",
                table: "inbox",
                column: "surat_id");

            migrationBuilder.CreateIndex(
                name: "IX_surat_ditujukan_ke_id",
                table: "surat",
                column: "ditujukan_ke_id");

            migrationBuilder.CreateIndex(
                name: "IX_surat_nomor_agenda",
                table: "surat",
                column: "nomor_agenda",
                unique: true,
                filter: "nomor_agenda IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_surat_user_id",
                table: "surat",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_surat_dibuat_oleh",
                table: "template_surat",
                column: "dibuat_oleh");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "disposisi_relation");

            migrationBuilder.DropTable(
                name: "inbox");

            migrationBuilder.DropTable(
                name: "template_surat");

            migrationBuilder.DropTable(
                name: "disposisi");

            migrationBuilder.DropTable(
                name: "surat");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
