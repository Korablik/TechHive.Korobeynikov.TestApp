using Serilog;
using TechHive.Korobeynikov.TestApp.BL.Handlers;
using TechHive.Korobeynikov.TestApp.Infrastructure.Managers;
using TechHive.Korobeynikov.TestApp.MemoryCache;
using TechHive.Korobeynikov.TestApp.Models.Contracts;
using TechHive.Korobeynikov.TestApp.SQLite;

namespace TechHive.Korobeynikov.TestApp.Server.Extensions;

public static class ConfigureServicesExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        var _logger = new LoggerConfiguration();
#if DEBUG
        _logger.WriteTo.File($"Logs/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now:dd}-logs.txt")
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information);
#endif
        Log.Logger = _logger.CreateLogger();

        services.AddSingleton(Log.Logger);
        services.AddMemoryCache();

        services.AddTransient<IStorage, MemoryCacheStorage>();

        services.AddTransient<IDB>(_ => new SQLiteDB(config.GetValue<string>("AppSettings:ConnectionString")));
        
        services.AddTransient<IWebSocketManager>(_ => new WebSocketsManager(config.GetValue<short>("AppSettings:ReceiveBufferSize")));
        services.AddTransient<IMessageHandler, MessageHandler>();

        return services;
    }
}
