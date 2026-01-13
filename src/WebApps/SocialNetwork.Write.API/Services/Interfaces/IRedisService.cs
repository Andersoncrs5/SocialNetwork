namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IRedisService
{
    Task<bool> CreateAsync<T>(string key, T value, TimeSpan? ttl = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> DeleteAsync(string key);
    Task<bool> RefreshTtlAsync(string key, TimeSpan? ttl = null);
}