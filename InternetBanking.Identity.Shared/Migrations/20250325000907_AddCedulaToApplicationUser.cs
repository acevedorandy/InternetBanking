using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetBanking.Identity.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddCedulaToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cedula",
                schema: "Identity",
                table: "Users");
        }
    }
}
