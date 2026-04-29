using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maildesk.Api.Migrations
{
    public partial class AddSuratKeluarSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE surat_keluar
                ADD COLUMN IF NOT EXISTS nomor_agenda text NOT NULL DEFAULT '';
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_surat_keluar_nomor_agenda"
                ON surat_keluar (nomor_agenda);
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_surat_keluar_nomor_surat"
                ON surat_keluar (nomor_surat);
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_surat_keluar_tanggal_surat"
                ON surat_keluar (tanggal_surat);
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_surat_keluar_tanggal_surat_nomor_agenda"
                ON surat_keluar (tanggal_surat, nomor_agenda);
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_surat_keluar_status"
                ON surat_keluar (status);
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_surat_keluar_tujuan_surat"
                ON surat_keluar (tujuan_surat);
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_surat_keluar_tujuan_surat";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_surat_keluar_status";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_surat_keluar_tanggal_surat_nomor_agenda";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_surat_keluar_tanggal_surat";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_surat_keluar_nomor_surat";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_surat_keluar_nomor_agenda";""");

            migrationBuilder.Sql("""
                ALTER TABLE surat_keluar
                DROP COLUMN IF EXISTS nomor_agenda;
            """);
        }
    }
}