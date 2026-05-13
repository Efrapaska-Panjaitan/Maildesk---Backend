using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveJenisAksiFromDisposisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JenisAksi",
                table: "disposisi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JenisAksi",
                table: "disposisi",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
