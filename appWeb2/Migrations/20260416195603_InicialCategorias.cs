using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appWeb2.Migrations
{
    /// <inheritdoc />
    public partial class InicialCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoria",
                table: "VideoJuegos");

            migrationBuilder.AlterColumn<decimal>(
                name: "precio",
                table: "VideoJuegos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioAnterior",
                table: "VideoJuegos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idcategoria",
                table: "VideoJuegos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    idcategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.idcategoria);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoJuegos_idcategoria",
                table: "VideoJuegos",
                column: "idcategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoJuegos_categoria_idcategoria",
                table: "VideoJuegos",
                column: "idcategoria",
                principalTable: "categoria",
                principalColumn: "idcategoria",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoJuegos_categoria_idcategoria",
                table: "VideoJuegos");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropIndex(
                name: "IX_VideoJuegos_idcategoria",
                table: "VideoJuegos");

            migrationBuilder.DropColumn(
                name: "idcategoria",
                table: "VideoJuegos");

            migrationBuilder.AlterColumn<decimal>(
                name: "precio",
                table: "VideoJuegos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioAnterior",
                table: "VideoJuegos",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "categoria",
                table: "VideoJuegos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
