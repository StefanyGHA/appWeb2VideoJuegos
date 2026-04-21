using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appWeb2.Migrations
{
    /// <inheritdoc />
    public partial class DetalleVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_VideoJuegos_VideoJuegosId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_VideoJuegosId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "VideoJuegosId",
                table: "Compras");

            migrationBuilder.CreateTable(
                name: "detalle_compra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoJuegosId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    estadoCompra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechaHoraTransaccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    codigoTransaccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idCompra = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_compra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detalle_compra_Compras_idCompra",
                        column: x => x.idCompra,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_compra_VideoJuegos_VideoJuegosId",
                        column: x => x.VideoJuegosId,
                        principalTable: "VideoJuegos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_idCompra",
                table: "detalle_compra",
                column: "idCompra");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_VideoJuegosId",
                table: "detalle_compra",
                column: "VideoJuegosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_compra");

            migrationBuilder.AddColumn<int>(
                name: "VideoJuegosId",
                table: "Compras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_VideoJuegosId",
                table: "Compras",
                column: "VideoJuegosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_VideoJuegos_VideoJuegosId",
                table: "Compras",
                column: "VideoJuegosId",
                principalTable: "VideoJuegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
