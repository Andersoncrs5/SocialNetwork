using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class NewTableCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    IconName = table.Column<string>(type: "VARCHAR(800)", nullable: true),
                    Color = table.Column<string>(type: "VARCHAR(6)", nullable: true),
                    Slug = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<uint>(type: "int unsigned", nullable: true),
                    ParentId = table.Column<string>(type: "varchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryModel_CategoryModel_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CategoryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryModel_Name",
                table: "CategoryModel",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryModel_ParentId",
                table: "CategoryModel",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryModel_Slug",
                table: "CategoryModel",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryModel");
        }
    }
}
