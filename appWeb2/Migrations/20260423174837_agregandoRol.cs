using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appWeb2.Migrations
{
    /// <inheritdoc />
    public partial class agregandoRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Rol_idRol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IdRol",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "idRol",
                table: "Usuarios",
                newName: "IdRol");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_idRol",
                table: "Usuarios",
                newName: "IX_Usuarios_IdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Rol_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "Rol",
                principalColumn: "IdRol",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Rol_IdRol",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "Usuarios",
                newName: "idRol");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                newName: "IX_Usuarios_idRol");

            migrationBuilder.AddColumn<int>(
                name: "IdRol",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Rol_idRol",
                table: "Usuarios",
                column: "idRol",
                principalTable: "Rol",
                principalColumn: "IdRol",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
