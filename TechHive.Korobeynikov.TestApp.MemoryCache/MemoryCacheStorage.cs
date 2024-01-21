using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Core;
using TechHive.Korobeynikov.TestApp.Models.Contracts;

namespace TechHive.Korobeynikov.TestApp.MemoryCache;

public class MemoryCacheStorage : IStorage
{
    private readonly ILogger _logger;
    protected string _listOfKeys = "DefaultMemoryCacheStorage";
    private IMemoryCache _cache { get; set; }

    public MemoryCacheStorage(ILogger logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _cache = memoryCache;
    }

    public bool TrySetValue(string key, object value, string? listOfKeys = null)
    {
        try
        {
            _logger.Information($"TrySetValue: {key} {value}");
            _cache.Set(PrepareKey(key, listOfKeys), value);
        }
        catch (Exception ex)
        {
            _logger.Error($"TrySetValue: {ex.Message}");
            return false;
        }
        return true;
    }

    public void Remove(string key, string? listOfKeys = null)
    {
        _logger.Information($"Remove: {key}");
        _cache.Remove(PrepareKey(key, listOfKeys));
    }

    public object? TryGetValue(string key, string? listOfKeys = null)
    {
        _logger.Information($"TryGetValue: {key}");
        return _cache.TryGetValue(PrepareKey(key, listOfKeys), out var result) ? result : null;
    }
    private string PrepareKey(string key, string? listOfKeys = null) => $"{listOfKeys ?? _listOfKeys}_{key.Trim().ToLower()}";
}
