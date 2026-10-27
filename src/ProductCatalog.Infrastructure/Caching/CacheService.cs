using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Abstractions;

namespace ProductCatalog.Infrastructure.Caching;

internal sealed class CacheService(
    IDistributedCache cache,
    ILogger<CacheService> logger) : ICacheService
{
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Cache read failed for {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? timeToLive,
        CancellationToken cancellationToken)
    {
        try
        {
            await cache.SetAsync(
                key,
                JsonSerializer.SerializeToUtf8Bytes(value),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive },
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Cache write failed for {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Cache remove failed for {CacheKey}", key);
        }
    }
}
