using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class NewIndexToTablePostModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_Language",
                table: "Posts",
                column: "Language");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Language",
                table: "Posts");
        }
    }
}
