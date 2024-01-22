
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using TechHive.Korobeynikov.TestApp.Infrastructure.Models;
using TechHive.Korobeynikov.TestApp.Models.Contracts;

namespace TechHive.Korobeynikov.TestApp.Infrastructure.Managers;

public class WebSocketsManager : IWebSocketManager
{
    public short _bufferSize { get; private set; }

    public WebSocketsManager(short bufferSize)
    {
        _bufferSize = bufferSize;
    }

    public string? GetWebSocketSubProtocol(HttpContext context) => GetHeader(context, Constants.WEB_SOCKET_PROTOCOL_HEADER);

    public string GetUDID(HttpContext context) => context.Request.Path.ToString().Split("/").LastOrDefault(string.Empty).ToLower().Trim();

    public bool IsWebSocketClosed(WebSocket webSocket)
    {
        return webSocket?.State == WebSocketState.Closed || webSocket?.State == WebSocketState.CloseReceived || webSocket?.State == WebSocketState.CloseSent 
            || webSocket?.CloseStatus != null || webSocket?.State == WebSocketState.Aborted;
    }

    public short GetReceiveBufferSize() => _bufferSize;

    private static string? GetHeader(HttpContext context, string key)
    {
        string? value = null;
        if (!context.Request.Headers.TryGetValue(key, out var webSocketProtocol))
            return value;

        value = webSocketProtocol.FirstOrDefault()?.Split(',', ';').LastOrDefault();
        if (string.IsNullOrWhiteSpace(value))
            value = null;

        return value;
    }
}
