using System;
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
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("jwt"));

var jwtSettings = builder.Configuration.GetSection("jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing");

builder.Services.AddIdentity<UserModel, RoleModel>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

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
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            var response = new ResponseHttp<IEnumerable<string>>(
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
    options.SwaggerDoc("v1", new() { Title = "Social Network API", Version = "v1" });
    options.EnableAnnotations();

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    };
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

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// ===================================================================================
// AUTO MAPPER
// ===================================================================================

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddMaps(typeof(UserMapper).Assembly); 
    mc.AddMaps(typeof(CategoryMapper).Assembly); 
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