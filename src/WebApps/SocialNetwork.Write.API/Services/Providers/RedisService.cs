using System.Text.Json;
using SocialNetwork.Write.API.Services.Interfaces;
using StackExchange.Redis;

namespace SocialNetwork.Write.API.Services.Providers;

public class RedisService(IDatabase db): IRedisService
{
    private readonly TimeSpan _defaultTtl = TimeSpan.FromMinutes(5);
    
    public async Task<bool> CreateAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        RedisValue json = (RedisValue) JsonSerializer.Serialize(value);
        return await db.StringSetAsync(key, json, ttl ?? _defaultTtl);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        RedisValue json = await db.StringGetAsync(key);
        if (json.IsNullOrEmpty) return default;

        return JsonSerializer.Deserialize<T>((byte[]) json!);
    }
    
    public Task<bool> ExistsAsync(string key)
        => db.KeyExistsAsync(key);

    public Task<bool> DeleteAsync(string key)
        => db.KeyDeleteAsync(key);

    public Task<bool> RefreshTtlAsync(string key, TimeSpan? ttl = null)
        => db.KeyExpireAsync(key, ttl ?? _defaultTtl);
}