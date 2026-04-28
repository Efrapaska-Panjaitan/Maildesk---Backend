using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Maildesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexSuratMasukSearch : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateIndex(
        name: "IX_surat_masuk_nomor_agenda",
        table: "surat_masuk",
        column: "nomor_agenda");

    migrationBuilder.CreateIndex(
        name: "IX_surat_masuk_tanggal_diterima",
        table: "surat_masuk",
        column: "tanggal_diterima");

    migrationBuilder.CreateIndex(
        name: "IX_surat_masuk_tanggal_diterima_nomor_agenda",
        table: "surat_masuk",
        columns: new[] { "tanggal_diterima", "nomor_agenda" });

    migrationBuilder.CreateIndex(
        name: "IX_surat_masuk_status",
        table: "surat_masuk",
        column: "status");

    migrationBuilder.CreateIndex(
        name: "IX_surat_masuk_jenis_sumber",
        table: "surat_masuk",
        column: "jenis_sumber");
}
            
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropIndex(
        name: "IX_surat_masuk_nomor_agenda",
        table: "surat_masuk");

    migrationBuilder.DropIndex(
        name: "IX_surat_masuk_tanggal_diterima",
        table: "surat_masuk");

    migrationBuilder.DropIndex(
        name: "IX_surat_masuk_tanggal_diterima_nomor_agenda",
        table: "surat_masuk");

    migrationBuilder.DropIndex(
        name: "IX_surat_masuk_status",
        table: "surat_masuk");

    migrationBuilder.DropIndex(
        name: "IX_surat_masuk_jenis_sumber",
        table: "surat_masuk");
}
    }
}
