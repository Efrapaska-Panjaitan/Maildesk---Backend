using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Maildesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialErdSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "surat_keluar",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    no_surat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    nomor_agenda = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tanggal_surat = table.Column<DateOnly>(type: "date", nullable: true),
                    kepada = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    perihal = table.Column<string>(type: "text", nullable: false),
                    file_lampiran = table.Column<byte[]>(type: "bytea", nullable: true),
                    nama_file = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    pembuat_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_surat_keluar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "surat_masuk",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    no_surat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nomor_agenda = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tanggal_surat = table.Column<DateOnly>(type: "date", nullable: false),
                    asal_pengirim = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    perihal = table.Column<string>(type: "text", nullable: false),
                    file_lampiran = table.Column<byte[]>(type: "bytea", nullable: true),
                    nama_file = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_surat_masuk", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_is_archived",
                table: "surat_keluar",
                column: "is_archived");

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_kepada",
                table: "surat_keluar",
                column: "kepada");

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_no_surat",
                table: "surat_keluar",
                column: "no_surat");

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_nomor_agenda",
                table: "surat_keluar",
                column: "nomor_agenda",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_status",
                table: "surat_keluar",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_tanggal_surat",
                table: "surat_keluar",
                column: "tanggal_surat");

            migrationBuilder.CreateIndex(
                name: "IX_surat_keluar_tanggal_surat_nomor_agenda",
                table: "surat_keluar",
                columns: new[] { "tanggal_surat", "nomor_agenda" });

            migrationBuilder.CreateIndex(
                name: "IX_surat_masuk_asal_pengirim",
                table: "surat_masuk",
                column: "asal_pengirim");

            migrationBuilder.CreateIndex(
                name: "IX_surat_masuk_is_archived",
                table: "surat_masuk",
                column: "is_archived");

            migrationBuilder.CreateIndex(
                name: "IX_surat_masuk_nomor_agenda",
                table: "surat_masuk",
                column: "nomor_agenda",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_surat_masuk_tanggal_surat",
                table: "surat_masuk",
                column: "tanggal_surat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "surat_keluar");

            migrationBuilder.DropTable(
                name: "surat_masuk");
        }
    }
}
