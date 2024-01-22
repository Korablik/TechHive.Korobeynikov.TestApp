
using Serilog;
using TechHive.Korobeynikov.TestApp.Infrastructure.Clients;
using TechHive.Korobeynikov.TestApp.Models;
using TechHive.Korobeynikov.TestApp.Models.DTO;
using TechHive.Korobeynikov.TestApp.Models.Helper;

var _logger = new LoggerConfiguration();
#if DEBUG
_logger.WriteTo.File($"Logs/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now:dd}-logs.txt")
    .WriteTo.Console(Serilog.Events.LogEventLevel.Information);
#endif
Log.Logger = _logger.CreateLogger();

string _endpoint = "ws://localhost:5219/ws";
string _deviceId = "UDIDshouldbea40-characteralphanumeric112";
string _friendDeviceId = "shouldbea40-characteralphanumeric223UDID";
Guid _playerId = Guid.Empty;
Guid _friendId = Guid.Empty;

Log.Logger.Information($"Connecting to a server endpoint: {_endpoint}");
Log.Logger.Information($"With Device Id: {_deviceId}");

WebSocketClient client = new WebSocketClient(Log.Logger);
client.SetEndpoint(_endpoint);

client.ConnectAsync(CancellationToken.None).Wait();

Login(_deviceId, ref _playerId);

Console.WriteLine();
Console.WriteLine($"PlayerId: {_playerId}");
Console.WriteLine();
Task.Delay(2000).Wait();

Login(_friendDeviceId, ref _friendId);
Console.WriteLine();
Console.WriteLine($"FriendId: {_friendId}");
Console.WriteLine();
Task.Delay(2000).Wait();

UpdateResources(_playerId, EResourceType.Coins, 100);
Task.Delay(2000).Wait();
UpdateResources(_playerId, EResourceType.Rolls, 10);
Task.Delay(2000).Wait();

UpdateResources(_friendId, EResourceType.Coins, 0);
Task.Delay(2000).Wait();
UpdateResources(_friendId, EResourceType.Rolls, 0);
Task.Delay(2000).Wait();

GetResources(_playerId);
Console.WriteLine();
Task.Delay(2000).Wait();
GetResources(_friendId);
Console.WriteLine();
Task.Delay(2000).Wait();

SendGift(_playerId, _friendId, EResourceType.Coins, 20);
Task.Delay(2000).Wait();
SendGift(_playerId, _friendId, EResourceType.Rolls, 3);
Task.Delay(2000).Wait();

GetResources(_playerId);
Console.WriteLine();
Task.Delay(2000).Wait();
GetResources(_friendId);
Console.WriteLine();
Task.Delay(2000).Wait();

Console.ReadLine();

client.ConnectAsync(CancellationToken.None).Wait();

void Login(string deviceId, ref Guid playerId)
{
    var login = new Message()
    {
        Type = "Login",
        Payload = new PlayerDTO()
        {
            UDID = deviceId
        }.ToString()
    }.ToString();

    client.SendMessageAsync(login, CancellationToken.None).Wait();

    var response = client.ReceiveMessages(CancellationToken.None).GetAwaiter().GetResult();

    Log.Logger.Information(response);

    if (!string.IsNullOrWhiteSpace(response))
    {
        var _response = SerializeHelper.TryDeserialize<Message>(response);
        if (_response?.Type == "Login")
        {
            var _player = SerializeHelper.TryDeserialize<PlayerDTO>(_response.Payload);
            if (_player != null)
            {
                playerId = _player.PlayerId ?? Guid.Empty;
            }
        }
    }
}

void UpdateResources(Guid playerId, EResourceType type, int value)
{
    var updateResources = new Message()
    {
        Type = "UpdateResources",
        Payload = new PlayerDTO()
        {
            PlayerId = playerId,
            Resources = new List<ResourceDTO>()
            {
                new ResourceDTO()
                {
                    Type = type,
                    Value = value
                }
            }
        }.ToString()
    }.ToString();

    client.SendMessageAsync(updateResources, CancellationToken.None).Wait();

    var response = client.ReceiveMessages(CancellationToken.None).GetAwaiter().GetResult();

    Log.Logger.Information(response);
}

void SendGift(Guid playerId, Guid _friendId, EResourceType type, int value)
{
    var updateResources = new Message()
    {
        Type = "SendGift",
        Payload = new GiftDTO()
        {
            PlayerId = playerId,
            FriendId = _friendId,
            Resource = new ResourceDTO()
            {
                Type = type,
                Value = value
            }
        }.ToString()
    }.ToString();

    client.SendMessageAsync(updateResources, CancellationToken.None).Wait();

    var response = client.ReceiveMessages(CancellationToken.None).GetAwaiter().GetResult();

    Log.Logger.Information(response);
}

void GetResources(Guid playerId)
{
    var updateResources = new Message()
    {
        Type = "GetResources",
        Payload = new PlayerDTO()
        {
            PlayerId = playerId
        }.ToString()
    }.ToString();

    client.SendMessageAsync(updateResources, CancellationToken.None).Wait();

    var response = client.ReceiveMessages(CancellationToken.None).GetAwaiter().GetResult();
    if (!string.IsNullOrWhiteSpace(response))
    {
        var _response = SerializeHelper.TryDeserialize<Message>(response);
        if (_response?.Type == "GetResources")
        {
            var _player = SerializeHelper.TryDeserialize<PlayerDTO>(_response.Payload);
            if (_player != null)
            {
                Log.Logger.Information($"Player: {_player.PlayerId}");
                _player.Resources.ForEach(x =>
                {
                    Log.Logger.Information($"{x.Type}: {x.Value}");
                }); 
            }
        }
    }


    Log.Logger.Information(response);
}
