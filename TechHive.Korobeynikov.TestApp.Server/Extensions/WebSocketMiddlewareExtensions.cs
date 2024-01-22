using TechHive.Korobeynikov.TestApp.Infrastructure.Middleware;

namespace TechHive.Korobeynikov.TestApp.Server.Extensions;

public static class WebSocketMiddlewareExtensions
{
    public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder app, PathString path)
    {
        return app.UseWhen(context => context.Request.Path.StartsWithSegments(path), appBuilder =>
        {
            appBuilder.UseMiddleware<WebSocketMiddleware>();
        });
    }
}