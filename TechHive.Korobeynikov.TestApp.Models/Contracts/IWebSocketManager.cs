using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace TechHive.Korobeynikov.TestApp.Models.Contracts;

public interface IWebSocketManager
{
    string? GetWebSocketSubProtocol(HttpContext context);

    string GetUDID(HttpContext context);

    bool IsWebSocketClosed(WebSocket webSocket);

    short GetReceiveBufferSize();
}
