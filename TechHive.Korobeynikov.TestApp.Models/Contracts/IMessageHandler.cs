using System.Net.WebSockets;

namespace TechHive.Korobeynikov.TestApp.Models.Contracts;

public interface IMessageHandler
{
    Task<string> HandleMessage(string message, WebSocket webSocket);
}
