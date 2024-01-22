using TechHive.Korobeynikov.TestApp.Infrastructure.Verification;

namespace TechHive.Korobeynikov.TestApp.Server.Extensions;

public static class WebSocketsBasicAuthorizationExtensions
{
    /// <summary>
    /// Reject all WebSocket requests without Authorization Header
    /// </summary>
    /// <param name="isIgnore">Ignore authorization header checks</param>
    /// <returns></returns>
    public static IApplicationBuilder UseWebSocketsBasicAuthorization(this IApplicationBuilder app, bool isIgnore = false)
    {
        return app.Use(async (context, next) =>
        {
            RequestVerification.VerifyAuthorizationHeader(context, isIgnore);

            await next(context);
        });
    }    
}
