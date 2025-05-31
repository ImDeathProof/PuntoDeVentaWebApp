using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoDeVentaWeb.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateToSmallDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AlterColumn<DateTime>(
                name: "Date",                  // Nombre de la columna
                table: "Purchases",            // Nombre de la tabla
                type: "smalldatetime",         // Nuevo tipo de dato
                nullable: false,               // ¿Acepta nulos?
                oldClrType: typeof(DateTime),  // Tipo anterior en C#
                oldType: "datetime2(7)");      // Tipo anterior en SQL
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Purchases",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");
        }
    }
}
