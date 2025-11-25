using System;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CleanArchitecture.Infrastructure.Services
{
  public class RedisCacheService : ICacheService
  {
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache distributedCache,
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
      _distributedCache = distributedCache;
      _connectionMultiplexer = connectionMultiplexer;
      _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
      try
      {
        var value = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(value))
          return null;

        return JsonSerializer.Deserialize<T>(value);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting cache key: {Key}", key);
        return null;
      }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
      try
      {
        var serializedValue = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
        {
          options.SetAbsoluteExpiration(expiration.Value);
        }

        await _distributedCache.SetStringAsync(key, serializedValue, options);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error setting cache key: {Key}", key);
        throw;
      }
    }

    public async Task RemoveAsync(string key)
    {
      try
      {
        await _distributedCache.RemoveAsync(key);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error removing cache key: {Key}", key);
        throw;
      }
    }

    public async Task<bool> ExistsAsync(string key)
    {
      try
      {
        var value = await _distributedCache.GetStringAsync(key);
        return !string.IsNullOrEmpty(value);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error checking if cache key exists: {Key}", key);
        return false;
      }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
      try
      {
        var database = _connectionMultiplexer.GetDatabase();
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);

        var keys = server.Keys(pattern: pattern);

        foreach (var key in keys)
        {
          await database.KeyDeleteAsync(key);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error removing cache keys by pattern: {Pattern}", pattern);
        throw;
      }
    }

    public async Task<bool> SetIfNotExistsAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
      try
      {
        var exists = await ExistsAsync(key);
        if (exists)
          return false;

        await SetAsync(key, value, expiration);
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error setting cache key if not exists: {Key}", key);
        throw;
      }
    }
  }
}