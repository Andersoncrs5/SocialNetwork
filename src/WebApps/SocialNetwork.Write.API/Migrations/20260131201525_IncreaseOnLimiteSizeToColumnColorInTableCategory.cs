using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseOnLimiteSizeToColumnColorInTableCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Categories",
                type: "VARCHAR(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(6)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Categories",
                type: "VARCHAR(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)",
                oldNullable: true);
        }
    }
}
