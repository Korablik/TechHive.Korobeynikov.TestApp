using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Core;
using TechHive.Korobeynikov.TestApp.Models.Contracts;

namespace TechHive.Korobeynikov.TestApp.MemoryCache
{
    public class MemoryCacheStorage : IStorage
    {
        private readonly ILogger _logger;
        protected string listOfKeys = "DefaultMemoryCacheStorage";
        private IMemoryCache _cache { get; set; }

        public MemoryCacheStorage(ILogger logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
        }

        public bool TrySetValue(string key, object value)
        {
            try
            {
                _logger.Information($"TrySetValue: {key} {value}");
                _cache.Set(PrepareKey(key), value);
            }
            catch (Exception ex)
            {
                _logger.Error($"TrySetValue: {ex.Message}");
                return false;
            }
            return true;
        }

        public void Remove(string key)
        {
            _logger.Information($"Remove: {key}");
            _cache.Remove(PrepareKey(key));
        }

        public object? TryGetValue(string key)
        {
            _logger.Information($"TryGetValue: {key}");
            return _cache.TryGetValue(PrepareKey(key), out var result) ? result : null;
        }
        private string PrepareKey(string key) => $"{listOfKeys}_{key.Trim().ToLower()}";
    }
}
