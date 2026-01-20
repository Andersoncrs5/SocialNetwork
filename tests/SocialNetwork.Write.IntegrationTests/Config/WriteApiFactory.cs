using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Write.API.Configs.DB;
using StackExchange.Redis;
using Testcontainers.MySql;
using Testcontainers.Redis;

namespace SocialNetwork.Write.IntegrationTests.Config;

public class WriteApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _dbContainer = new MySqlBuilder()
        .WithImage("mysql:8.0") 
        .WithDatabase("social_network_test")
        .WithUsername("root")
        .WithPassword("123456")
        .Build();
    
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d => 
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbDescriptor != null) services.Remove(dbDescriptor);
            
            var redisDescriptor = services.SingleOrDefault(d => 
                d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null) services.Remove(redisDescriptor);

            var redisConn = _redisContainer.GetConnectionString();
            var multiplexer = ConnectionMultiplexer.Connect(redisConn);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddScoped<IDatabase>(s => s.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            
            var connectionString = _dbContainer.GetConnectionString();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_dbContainer.StartAsync(), _redisContainer.StartAsync());

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var retries = 5;
        while (retries > 0)
        {
            try
            {
                await db.Database.EnsureCreatedAsync();
                await db.Database.MigrateAsync();
                break; 
            }
            catch (Exception)
            {
                retries--;
                if (retries == 0) throw;
                await Task.Delay(2000); 
            }
        }
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(_dbContainer.StopAsync(), _redisContainer.StopAsync());
    }
}