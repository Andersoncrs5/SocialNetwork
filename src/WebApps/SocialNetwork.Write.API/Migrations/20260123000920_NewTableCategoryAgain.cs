using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class NewTableCategoryAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryModel_CategoryModel_ParentId",
                table: "CategoryModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryModel",
                table: "CategoryModel");

            migrationBuilder.RenameTable(
                name: "CategoryModel",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryModel_Slug",
                table: "Categories",
                newName: "IX_Categories_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryModel_ParentId",
                table: "Categories",
                newName: "IX_Categories_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryModel_Name",
                table: "Categories",
                newName: "IX_Categories_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories",
                column: "ParentId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "CategoryModel");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Slug",
                table: "CategoryModel",
                newName: "IX_CategoryModel_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ParentId",
                table: "CategoryModel",
                newName: "IX_CategoryModel_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Name",
                table: "CategoryModel",
                newName: "IX_CategoryModel_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryModel",
                table: "CategoryModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryModel_CategoryModel_ParentId",
                table: "CategoryModel",
                column: "ParentId",
                principalTable: "CategoryModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
