using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net.WebSockets;
using System.Text;
using TechHive.Korobeynikov.TestApp.Models.Contracts;

namespace TechHive.Korobeynikov.TestApp.Infrastructure.Middleware;

public class WebSocketMiddleware
{
    private readonly ILogger _logger;
    private readonly IMessageHandler _messageHandler;
    private readonly IWebSocketManager _webSocketManager;
    public WebSocketMiddleware(RequestDelegate next, ILogger logger, IMessageHandler messageHandler, IWebSocketManager webSocketManager)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _webSocketManager = webSocketManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync(_webSocketManager.GetWebSocketSubProtocol(context));

        _logger.Information("WebSocket request from {0}; IP: {1}: Headers: Keys: {2}; Values: {3}",
            context.Request.Path, context.Connection.RemoteIpAddress, context.Request.Headers.Keys, context.Request.Headers.Values);

        var buffer = new byte[_webSocketManager.GetReceiveBufferSize()];
        var buffMessage = string.Empty;
        WebSocketReceiveResult receiveResult;

        while (webSocket?.State == WebSocketState.Open)
        {
            try
            {
                do
                {
                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    buffMessage += Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                }
                while (!receiveResult.EndOfMessage);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                var response = await _messageHandler.HandleMessage(buffMessage, webSocket);

                if (!string.IsNullOrWhiteSpace(response))
                    await webSocket.SendAsync(Encoding.UTF8.GetBytes(response), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            finally
            {
                buffer = new byte[_webSocketManager.GetReceiveBufferSize()];
                buffMessage = string.Empty;

                if (_webSocketManager.IsWebSocketClosed(webSocket)) 
                { 
                    // remove WebSocket from memory cache
                    _logger.Information("WebSocket Connection {0} Closed", context.Request.Path);
                }
            }
        }
    }
}