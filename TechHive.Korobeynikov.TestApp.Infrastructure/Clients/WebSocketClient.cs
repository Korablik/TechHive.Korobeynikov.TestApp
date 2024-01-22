using Serilog;
using System.Net.WebSockets;
using System.Text;
using TechHive.Korobeynikov.TestApp.Infrastructure.Models;

namespace TechHive.Korobeynikov.TestApp.Infrastructure.Clients;

public class WebSocketClient
{
    private readonly ILogger _logger;

    private Uri? _serverUri { get; set; }

    private ClientWebSocket _webSocket = new ClientWebSocket();

    public WebSocketClient(ILogger logger)
    {
        _logger = logger;
    }

    public void SetEndpoint(string endpoint)
    {
        _serverUri = new Uri(endpoint);
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        _webSocket = new ClientWebSocket();
        _webSocket.Options.SetRequestHeader(Constants.AUTHORIZATION_HEADER, "Authorization Token");
        await _webSocket.ConnectAsync(_serverUri, cancellationToken);
        _logger.Information("Connected!");
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
        _logger.Information("Message sent: " + message);
    }

    public async Task<string> ReceiveMessages(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];
        var message = string.Empty;
        
        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            _logger.Information("Connection closing");
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
        }
        else
        {
            message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            _logger.Information("Message received: " + message);
        }
        return message;
    }

    public async Task CloseConnectionAsync(CancellationToken cancellationToken)
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", cancellationToken);
        _logger.Information("Connection closed");
    }
}
