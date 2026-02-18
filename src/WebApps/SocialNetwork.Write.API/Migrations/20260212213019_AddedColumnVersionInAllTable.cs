using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnVersionInAllTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Tags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "PostTags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "PostFavorites",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "PostCategory",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Comments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "CommentFavorites",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Categories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PostFavorites");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PostCategory");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CommentFavorites");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Categories");
        }
    }
}
