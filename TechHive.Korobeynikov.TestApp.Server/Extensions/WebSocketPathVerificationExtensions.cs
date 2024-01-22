using TechHive.Korobeynikov.TestApp.Infrastructure.Verification;

namespace TechHive.Korobeynikov.TestApp.Server.Extensions;

public static class WebSocketPathVerificationExtensions
{
    /// <summary>
    /// Reject all NOT WebSocket requests to WebSocket endpoint
    /// </summary>
    /// <param name="path">WebSocket endpoint</param>
    /// <returns></returns>
    public static IApplicationBuilder UseWebSocketPathVerification(this IApplicationBuilder app, PathString path)
    {
        return app.Use(async (context, next) =>
        {
            RequestVerification.VerifyPath(context, path);
            
            await next(context);
        });
    }
}
