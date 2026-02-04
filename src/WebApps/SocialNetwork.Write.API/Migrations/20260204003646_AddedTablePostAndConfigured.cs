using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedTablePostAndConfigured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "VARCHAR(700)", maxLength: 700, nullable: false),
                    Summary = table.Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true),
                    FeaturedImageUrl = table.Column<string>(type: "VARCHAR(800)", maxLength: 800, nullable: true),
                    Visibility = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ReadingTime = table.Column<int>(type: "int", nullable: true),
                    RankingScore = table.Column<double>(type: "double", nullable: false, defaultValue: 0.0),
                    IsCommentsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    HighlightStatus = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ModerationStatus = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ReadingLevel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PostType = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
