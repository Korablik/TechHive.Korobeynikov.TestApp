using Microsoft.AspNetCore.Http;
using System.Net;
using TechHive.Korobeynikov.TestApp.Infrastructure.Models;

namespace TechHive.Korobeynikov.TestApp.Infrastructure.Verification;

public static class RequestVerification
{
    /// <summary>
    /// Change status code to BadRequest if request is not WebSocket request
    /// </summary>
    /// <param name="context"></param>
    /// <param name="path"></param>
    public static void VerifyPath(HttpContext context, string path)
    {
        if (context.WebSockets?.IsWebSocketRequest ?? false)
        {
            if (!context.Request.Path.StartsWithSegments(new PathString(path), StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return; 
            }
        }
    }

    /// <summary>
    /// Change status code to Unauthorized if request is WebSocket request and Authorization Header is not exists
    /// </summary>
    /// <param name="context"></param>
    /// <param name="isIgnore"></param>
    public static void VerifyAuthorizationHeader(HttpContext context, bool isIgnore = false)
    {
        if (!isIgnore && (context.WebSockets?.IsWebSocketRequest ?? false))
        {
            if (!IsAuthorizationHeaderExists(context))
            { 
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }
    }

    private static bool IsAuthorizationHeaderExists(HttpContext context)
    {
        return context.Request.Headers.ContainsKey(Constants.AUTHORIZATION_HEADER) 
            || !string.IsNullOrWhiteSpace(context.Request.Query.FirstOrDefault(x => x.Key == Constants.AUTHORIZATION_HEADER).Value);
    }
}
