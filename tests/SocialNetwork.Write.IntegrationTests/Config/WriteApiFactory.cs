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
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            
            var redisDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null) services.Remove(redisDescriptor);

            var redisConn = _redisContainer.GetConnectionString();
            var multiplexer = ConnectionMultiplexer.Connect(redisConn);
            
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddScoped<IDatabase>(s => multiplexer.GetDatabase());
            
            var connectionString = _dbContainer.GetConnectionString();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate(); 
        });
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _dbContainer.StartAsync(), 
            _redisContainer.StartAsync()
        );
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _dbContainer.StopAsync(),
            _redisContainer.StopAsync()
        );
    }
}