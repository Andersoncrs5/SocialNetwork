using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SocialNetwork.Write.API.Services.Interfaces;
using StackExchange.Redis;

namespace SocialNetwork.Write.API.Services.Providers;

public class RedisService(IDistributedCache cache) : IRedisService
{
    private readonly TimeSpan _defaultTtl = TimeSpan.FromMinutes(8);

    public async Task<bool> CreateAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? _defaultTtl
            };

            var jsonData = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, jsonData, options);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await cache.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(jsonData)) 
            return default;

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var data = await cache.GetAsync(key);
        return data != null;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            await cache.RemoveAsync(key);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RefreshTtlAsync(string key, TimeSpan? ttl = null)
    {
        try
        {
            await cache.RefreshAsync(key);
            return true;
        }
        catch
        {
            return false;
        }
    }
}