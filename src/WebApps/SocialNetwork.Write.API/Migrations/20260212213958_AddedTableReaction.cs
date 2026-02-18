using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedTableReaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    EmojiUrl = table.Column<string>(type: "TEXT", nullable: true),
                    EmojiUnicode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    DisplayOrder = table.Column<long>(type: "bigint", nullable: true),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Visible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_Name",
                table: "Reactions",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reactions");
        }
    }
}
