using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SocialNetwork.Write.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_roles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    Bio = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "VARCHAR(800)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsPrivate = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Language = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true),
                    Country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ImageProfileUrl = table.Column<string>(type: "VARCHAR(800)", nullable: true),
                    RefreshToken = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    IconName = table.Column<string>(type: "VARCHAR(800)", nullable: true),
                    Color = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Slug = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<uint>(type: "int unsigned", nullable: true),
                    ParentId = table.Column<string>(type: "varchar(450)", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsSystem = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_app_role_claims_app_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "app_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_app_user_claims_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_app_user_logins_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_roles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_app_user_roles_app_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "app_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_app_user_roles_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_tokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_app_user_tokens_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true),
                    FeaturedImageUrl = table.Column<string>(type: "VARCHAR(800)", maxLength: 800, nullable: true),
                    Visibility = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ReadingTime = table.Column<int>(type: "int", nullable: true),
                    RankingScore = table.Column<double>(type: "double", nullable: false, defaultValue: 0.0),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IsCommentsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    Pinned = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    HighlightStatus = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ModerationStatus = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ReadingLevel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PostType = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    Language = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Undefined"),
                    ParentId = table.Column<string>(type: "varchar(450)", nullable: true),
                    UserId = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    PostId = table.Column<string>(type: "varchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ParentId = table.Column<string>(type: "varchar(450)", nullable: true),
                    SentimentScore = table.Column<double>(type: "double", nullable: false),
                    IsEdited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModerationStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    PostId = table.Column<string>(type: "varchar(450)", nullable: false),
                    CategoryId = table.Column<string>(type: "varchar(450)", nullable: false),
                    Order = table.Column<uint>(type: "INT UNSIGNED", nullable: false, defaultValue: 0u),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostCategory_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostFavorites",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    PostId = table.Column<string>(type: "varchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    PostModelId = table.Column<string>(type: "varchar(450)", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostFavorites_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostFavorites_Posts_PostModelId",
                        column: x => x.PostModelId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostFavorites_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostReactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ReactionId = table.Column<string>(type: "varchar(450)", nullable: false),
                    PostId = table.Column<string>(type: "varchar(450)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReactions_Post_Post",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReactions_Reaction_Post",
                        column: x => x.ReactionId,
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReactions_User_Post",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostTags",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    PostId = table.Column<string>(type: "varchar(450)", nullable: false),
                    TagId = table.Column<string>(type: "varchar(450)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostTags_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostVotes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    PostId = table.Column<string>(type: "varchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostVotes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostVotes_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommentFavorites",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    CommentId = table.Column<string>(type: "varchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    CommentModelId = table.Column<string>(type: "varchar(450)", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentFavorites_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentFavorites_Comments_CommentModelId",
                        column: x => x.CommentModelId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentFavorites_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommentReactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ReactionId = table.Column<string>(type: "varchar(450)", nullable: false),
                    CommentId = table.Column<string>(type: "varchar(450)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Reactions_ReactionId",
                        column: x => x.ReactionId,
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReactions_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_app_role_claims_RoleId",
                table: "app_role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "app_roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_app_user_claims_UserId",
                table: "app_user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_logins_UserId",
                table: "app_user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_roles_RoleId",
                table: "app_user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "app_users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "app_users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentFavorites_CommentId",
                table: "CommentFavorites",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentFavorites_CommentModelId",
                table: "CommentFavorites",
                column: "CommentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentFavorites_UserId_CommentId",
                table: "CommentFavorites",
                columns: new[] { "UserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId",
                table: "CommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactionId",
                table: "CommentReactions",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_UserId_CommentId",
                table: "CommentReactions",
                columns: new[] { "UserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_CategoryId",
                table: "PostCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_PostId",
                table: "PostCategory",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostFavorites_PostId",
                table: "PostFavorites",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostFavorites_PostModelId",
                table: "PostFavorites",
                column: "PostModelId");

            migrationBuilder.CreateIndex(
                name: "UK_PostFavorites_User_Post",
                table: "PostFavorites",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId",
                table: "PostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_ReactionId",
                table: "PostReactions",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "UK_PostReactions_UserId_PostId",
                table: "PostReactions",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Language",
                table: "Posts",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ParentId",
                table: "Posts",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_PostId",
                table: "PostTags",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PostVotes_PostId",
                table: "PostVotes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "UK_PostVotes_User_Post",
                table: "PostVotes",
                columns: new[] { "UserId", "PostId" },
                unique: true);

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
                name: "app_role_claims");

            migrationBuilder.DropTable(
                name: "app_user_claims");

            migrationBuilder.DropTable(
                name: "app_user_logins");

            migrationBuilder.DropTable(
                name: "app_user_roles");

            migrationBuilder.DropTable(
                name: "app_user_tokens");

            migrationBuilder.DropTable(
                name: "CommentFavorites");

            migrationBuilder.DropTable(
                name: "CommentReactions");

            migrationBuilder.DropTable(
                name: "PostCategory");

            migrationBuilder.DropTable(
                name: "PostFavorites");

            migrationBuilder.DropTable(
                name: "PostReactions");

            migrationBuilder.DropTable(
                name: "PostTags");

            migrationBuilder.DropTable(
                name: "PostVotes");

            migrationBuilder.DropTable(
                name: "app_roles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "app_users");
        }
    }
}
