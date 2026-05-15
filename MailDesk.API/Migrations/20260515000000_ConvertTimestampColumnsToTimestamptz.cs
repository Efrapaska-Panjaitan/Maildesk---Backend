using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maildesk.Api.Migrations
{
    public partial class ConvertTimestampColumnsToTimestamptz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE users
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE surat
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE template_surat
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi
                ALTER COLUMN waktu_diterima TYPE timestamp with time zone
                USING waktu_diterima AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi
                ALTER COLUMN completed_at TYPE timestamp with time zone
                USING completed_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE inbox
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi_relation
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi_log
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE surat_masuk
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE surat_keluar
                ALTER COLUMN created_at TYPE timestamp with time zone
                USING created_at AT TIME ZONE 'UTC';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE surat_keluar
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE surat_masuk
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi_log
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi_relation
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE inbox
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi
                ALTER COLUMN completed_at TYPE timestamp without time zone
                USING completed_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi
                ALTER COLUMN waktu_diterima TYPE timestamp without time zone
                USING waktu_diterima AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE disposisi
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE template_surat
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE surat
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");

            migrationBuilder.Sql(@"ALTER TABLE users
                ALTER COLUMN created_at TYPE timestamp without time zone
                USING created_at AT TIME ZONE 'UTC';");
        }
    }
}