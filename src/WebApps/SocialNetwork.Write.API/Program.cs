using System;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Write.API.Configs.Exception;
using SocialNetwork.Write.API.Models; // Ajuste para seu UserModel/RoleModel
using SocialNetwork.Write.API.Configs.DB;
using Microsoft.AspNetCore.RateLimiting;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using SocialNetwork.Contracts.configs.jwt;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.InfoApp;
using SocialNetwork.Write.API.Modules.Category.Repository.Interface;
using SocialNetwork.Write.API.Modules.Category.Repository.Provider;
using SocialNetwork.Write.API.Modules.Category.Service.Interface;
using SocialNetwork.Write.API.Modules.Category.Service.Provider;
using SocialNetwork.Write.API.Modules.Comment.Repository.Interface;
using SocialNetwork.Write.API.Modules.Comment.Repository.Provider;
using SocialNetwork.Write.API.Modules.Comment.Service.Interface;
using SocialNetwork.Write.API.Modules.Comment.Service.Provider;
using SocialNetwork.Write.API.Modules.CommentFavorite.Repository.Interface;
using SocialNetwork.Write.API.Modules.CommentFavorite.Repository.Provider;
using SocialNetwork.Write.API.Modules.CommentFavorite.Service.Interface;
using SocialNetwork.Write.API.Modules.CommentFavorite.Service.Provider;
using SocialNetwork.Write.API.Modules.CommentReactions.Repository.Interface;
using SocialNetwork.Write.API.Modules.CommentReactions.Repository.Provider;
using SocialNetwork.Write.API.Modules.CommentReactions.Service.Interface;
using SocialNetwork.Write.API.Modules.CommentReactions.Service.Provider;
using SocialNetwork.Write.API.Modules.CommentVote.Repository.Interface;
using SocialNetwork.Write.API.Modules.CommentVote.Service.Interface;
using SocialNetwork.Write.API.Modules.CommentVote.Service.provider;
using SocialNetwork.Write.API.Modules.Post.Repository.Interface;
using SocialNetwork.Write.API.Modules.Post.Repository.Provider;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;
using SocialNetwork.Write.API.Modules.Post.Service.Provider;
using SocialNetwork.Write.API.Modules.PostCategory.Repository.Interface;
using SocialNetwork.Write.API.Modules.PostCategory.Repository.Provider;
using SocialNetwork.Write.API.Modules.PostCategory.Service.Interface;
using SocialNetwork.Write.API.Modules.PostCategory.Service.Provider;
using SocialNetwork.Write.API.Modules.PostFavorite.Repository.Interface;
using SocialNetwork.Write.API.Modules.PostFavorite.Repository.Provider;
using SocialNetwork.Write.API.Modules.PostFavorite.Service.Interface;
using SocialNetwork.Write.API.Modules.PostFavorite.Service.Provider;
using SocialNetwork.Write.API.Modules.PostReactions.Repository.Interface;
using SocialNetwork.Write.API.Modules.PostReactions.Repository.Provider;
using SocialNetwork.Write.API.Modules.PostReactions.Service.Interface;
using SocialNetwork.Write.API.Modules.PostReactions.Service.Provider;
using SocialNetwork.Write.API.Modules.PostTag.Repository.Interface;
using SocialNetwork.Write.API.Modules.PostTag.Repository.Provider;
using SocialNetwork.Write.API.Modules.PostTag.Service.Interface;
using SocialNetwork.Write.API.Modules.PostTag.Service.Provider;
using SocialNetwork.Write.API.Modules.PostVote.Gateway;
using SocialNetwork.Write.API.Modules.PostVote.Repository.Interface;
using SocialNetwork.Write.API.Modules.PostVote.Repository.Provider;
using SocialNetwork.Write.API.Modules.PostVote.Service.Interface;
using SocialNetwork.Write.API.Modules.PostVote.Service.provider;
using SocialNetwork.Write.API.Modules.Reaction.Repository.Interface;
using SocialNetwork.Write.API.Modules.Reaction.Repository.Provider;
using SocialNetwork.Write.API.Modules.Reaction.Service.Interface;
using SocialNetwork.Write.API.Modules.Reaction.Service.Provider;
using SocialNetwork.Write.API.Modules.Role.Model;
using SocialNetwork.Write.API.Modules.Role.Repository.Interfaces;
using SocialNetwork.Write.API.Modules.Role.Repository.Provider;
using SocialNetwork.Write.API.Modules.Role.Service.Interface;
using SocialNetwork.Write.API.Modules.Role.Service.Provider;
using SocialNetwork.Write.API.Modules.Tag.Repository.Interface;
using SocialNetwork.Write.API.Modules.Tag.Repository.Provider;
using SocialNetwork.Write.API.Modules.Tag.Service.Interface;
using SocialNetwork.Write.API.Modules.Tag.Service.Provider;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Modules.User.Repository.Interface;
using SocialNetwork.Write.API.Modules.User.Repository.Provider;
using SocialNetwork.Write.API.Modules.User.Service.Interface;
using SocialNetwork.Write.API.Modules.User.Service.Provider;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.Mappers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// ===================================================================================
// BANCAS DE DADOS (TiDB & Redis)
// ===================================================================================
var connectionString = builder.Configuration.GetConnectionString("TiDBConnection");

if (connectionString == null)
    throw new ArgumentNullException(nameof(connectionString));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySQL(connectionString).EnableSensitiveDataLogging().EnableDetailedErrors();
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "SocialNetwork_";
});

// ===================================================================================
// INFO APP
// ===================================================================================
builder.Services.Configure<InfoAppOptions>(builder.Configuration.GetSection("InfoApp"));

// ===================================================================================
// IDENTITY & JWT
// ===================================================================================


builder.Services.AddIdentity<UserModel, RoleModel>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("jwt"));

JwtOptions jwt = builder.Configuration
    .GetSection("jwt")
    .Get<JwtOptions>()!;

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey!));

Console.WriteLine($"SECRET LENGTH: {jwt.SecretKey?.Length}");
Console.WriteLine($"SECRET : {jwt.SecretKey}");
Console.WriteLine($"ISSUER: {jwt.ValidIssuer}");
Console.WriteLine($"AUDIENCE: {jwt.ValidAudience}");

Console.WriteLine($"Key Bytes: {key.Key.ToArray().Select(x => x)}");
Console.WriteLine($"Key Id: {key.KeyId}");
Console.WriteLine($"Key Size: {key.KeySize}");
Console.WriteLine($"Key Crypto: {key.CryptoProviderFactory}");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.ValidIssuer,

        ValidateAudience = true,
        ValidAudience = jwt.ValidAudience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        RoleClaimType = ClaimTypes.Role
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("=== FAIL DEBUG ===");
            Console.WriteLine(context.Exception.ToString());
            Console.WriteLine($"KEY: {options.TokenValidationParameters.IssuerSigningKey != null}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("\n\n\n\n======================================================================");
            Console.WriteLine("Token validado com sucesso!");
            Console.WriteLine("======================================================================\n\n\n\n");
            return Task.CompletedTask;
        },
        OnMessageReceived = context => 
        {
            Console.WriteLine("=== JWT DEBUG ===");
            Console.WriteLine($"IssuerSigningKey: {options.TokenValidationParameters.IssuerSigningKey}");
            Console.WriteLine($"ValidIssuer: {options.TokenValidationParameters.ValidIssuer}");
            Console.WriteLine($"ValidAudience: {options.TokenValidationParameters.ValidAudience}");
            return Task.CompletedTask;
        },
        
    };
    
    
});

// ===================================================================================
// API VERSIONING
// ===================================================================================
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ===================================================================================
// SWAGGER & CONTROLLERS
// ===================================================================================

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

// ===================================================================================
// SWAGGER CONFIGURATION
// ===================================================================================
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new ResponseHttp<IDictionary<string, string[]>>(
                Data: errors,
                Message: "Validation failed",
                TraceId: context.HttpContext.TraceIdentifier,
                Success: false,
                Timestamp: DateTime.UtcNow
            );

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Social Network API",
        Version = "v1"
    });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    }); 
});

// ===================================================================================
// RATE LIMITER
// ===================================================================================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("authSystemPolicy", opt => {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromSeconds(5);
    });
});

// ===================================================================================
// INFRA & EXCEPTION
// ===================================================================================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPostCategoryRepository, PostCategoryRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPostTagRepository, PostTagRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPostFavoriteRepository, PostFavoriteRepository>();
builder.Services.AddScoped<ICommentFavoriteRepository, CommentFavoriteRepository>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
builder.Services.AddScoped<ICommentReactionRepository, CommentReactionRepository>();
builder.Services.AddScoped<IPostVoteRepository, PostVoteRepository>();
builder.Services.AddScoped<IPostReactionRepository, PostReactionRepository>();
builder.Services.AddScoped<ICommentVoteRepository, ICommentVoteRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPostCategoryService, PostCategoryService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostTagService, PostTagService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPostFavoriteService, PostFavoriteService>();
builder.Services.AddScoped<ICommentFavoriteService, CommentFavoriteService>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<ICommentReactionService, CommentReactionService>();
builder.Services.AddScoped<IPostVoteService, PostVoteService>();
builder.Services.AddScoped<IPostReactionService, PostReactionService>();
builder.Services.AddScoped<ICommentVoteService, CommentVoteService>();

builder.Services.AddScoped<PostVoteModuleGateway>();

// ===================================================================================
// AUTO MAPPER
// ===================================================================================

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddMaps(typeof(UserMapper).Assembly); 
    mc.AddMaps(typeof(CategoryMapper).Assembly); 
    mc.AddMaps(typeof(TagMapper).Assembly); 
    mc.AddMaps(typeof(PostMapper).Assembly); 
    mc.AddMaps(typeof(PostCategoryMapper).Assembly); 
    mc.AddMaps(typeof(PostTagMapper).Assembly); 
    mc.AddMaps(typeof(CommentMapper).Assembly); 
    mc.AddMaps(typeof(PostFavoriteMapper).Assembly); 
    mc.AddMaps(typeof(CommentFavoriteMapper).Assembly); 
    mc.AddMaps(typeof(ReactionMapper).Assembly); 
    mc.AddMaps(typeof(CommentReactionMapper).Assembly); 
}, builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>());

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    
    var userManager = services.GetRequiredService<UserManager<UserModel>>();
    var roleManager = services.GetRequiredService<RoleManager<RoleModel>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    
    var datasSystemSection = configuration.GetSection("DataSystem");
    
    string systemUserName = datasSystemSection["SystemName"] ?? throw new InvalidOperationException("System user name configuration is missing.");
    string systemUserEmail = datasSystemSection["systemUserEmail"] ?? throw new InvalidOperationException("System user email configuration is missing.");
    string systemUserPassword = datasSystemSection["SystemUserPassword"] ?? throw new InvalidOperationException("System user password configuration is missing.");
    bool isPrivate = configuration.GetSection("DataSystem:IsPrivate").Get<bool?>() ?? throw new InvalidOperationException("System user IsPrivate configuration is missing.");
    
    var datasRoles = configuration.GetSection("Roles");
    string masterRole = datasRoles["MasterRole"] ?? throw new InvalidOperationException("Master role configuration is missing.");
    string userRole = datasRoles["UserRole"] ?? throw new InvalidOperationException("User role configuration is missing.");
    string superAdmRole = datasRoles["SuperAdmRole"] ?? throw new InvalidOperationException("Super adm role configuration is missing.");
    var otherRoles = configuration.GetSection("Roles:OtherRoles").Get<string[]>() 
                     ?? throw new InvalidOperationException("OtherRoles configuration is missing.");

    var roles = new List<string> { userRole, masterRole, superAdmRole };
    roles.AddRange(otherRoles);
    
    try 
    {
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new RoleModel { Id = Guid.NewGuid().ToString(), Name = roleName });
            }
        }
        
        var systemUser = await userManager.FindByEmailAsync(systemUserEmail);
        if (systemUser == null)
        {
            var newSystemUser = new UserModel
            {
                UserName = systemUserName,
                Email = systemUserEmail,
                EmailConfirmed = true,
                IsPrivate = isPrivate
            };

            var createResult = await userManager.CreateAsync(newSystemUser, systemUserPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newSystemUser, masterRole);
                await userManager.AddToRoleAsync(newSystemUser, superAdmRole);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database (Migrations/Seed).");
    }
    
}

// ===================================================================================
// PIPELINE (MIDDLEWARES)
// ===================================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Gera endpoints para as versões descobertas
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseExceptionHandler(); 
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.Run();

public partial class Program { }