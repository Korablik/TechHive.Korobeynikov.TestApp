using Serilog;
using System.Net.WebSockets;
using TechHive.Korobeynikov.TestApp.Models;
using TechHive.Korobeynikov.TestApp.Models.Contracts;
using TechHive.Korobeynikov.TestApp.Models.DTO;
using TechHive.Korobeynikov.TestApp.Models.Entities;
using TechHive.Korobeynikov.TestApp.Models.Helper;

namespace TechHive.Korobeynikov.TestApp.BL.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IDB _db;
        private readonly ILogger _logger;
        private readonly IStorage _storage;
        public MessageHandler(IDB db, ILogger logger, IStorage storage)
        {
            _db = db;
            _logger = logger;
            _storage = storage;
        }

#warning Spaghetti code because of time limit

        public async Task<string> HandleMessage(string message, WebSocket webSocket)
        {
            _logger.Information("HandleMessage: {0}", message);

            if (string.IsNullOrWhiteSpace(message))
                return string.Empty;

            var _responseMessage = new Message();

            var _request = ParseMessage(message);
            if (_request == null)
            {
                _responseMessage.Type = "Error";
                _responseMessage.ErrorCode = "ProtocolError";
                return _responseMessage.ToString();
            }

            _responseMessage.Type = _request.Type;
            switch (_request.Type)
            {
                case "Login":
                    _responseMessage = Login(_request, webSocket, _responseMessage.Type);
                    break;
                case "Logout":
                    _responseMessage = Logout(_request, _responseMessage.Type);
                    break;
                case "UpdateResources":
                    _responseMessage = UpdateResources(_request, _responseMessage.Type);
                    break;
                case "GetResources":
                    _responseMessage = GetResources(_request, _responseMessage.Type);
                    break;
                case "SendGift":
                    _responseMessage = SendGift(_request, _responseMessage.Type);
                    break;
                default:
                    _responseMessage.Type = "Error";
                    _responseMessage.ErrorCode = "NotSupported";
                    return _responseMessage.ToString();
            }

            var _response = _responseMessage.ToString();
            _logger.Information("HandleMessage response: {0}", _response);

            return _response;
        }

        private Message ErrorMessage(string errorCode, string? info = null) => new Message
        {
            Type = "Error",
            ErrorCode = errorCode,
            Info = info
        };

        private Message? ParseMessage(string message) => SerializeHelper.TryDeserialize<Message>(message);

        private Message Login(Message request, WebSocket webSocket, string type)
        {
            var _player = SerializeHelper.TryDeserialize<PlayerDTO>(request?.Payload ?? string.Empty);
            if (_player == null)
                return ErrorMessage("FormationViolation", "Payload is not a valid");

            if (_player.UDID.Length != 40)
                return ErrorMessage("FormationViolation", "UDID is not a valid");

            var _playerEntity = _db.GetPlayerByUDID(_player.UDID);
            if (_playerEntity == null)
            {
                _playerEntity = new Player();
                _playerEntity.UDIDs.Add(_player.UDID);

                _db.SetOrUpdatePlayer(_playerEntity);
            }
            _player.PlayerId = _playerEntity.Id;

            var connection = (WebSocket?)_storage.TryGetValue($"{_player.PlayerId}-Connection");
            if (connection?.State == WebSocketState.Open)
            {
                return new Message
                {
                    Type = "Error",
                    ErrorCode = "AlreadyLoggedIn"
                };
            }
            else
            { 
                _storage.TrySetValue($"{_player.PlayerId}-Connection", webSocket);
                return new Message() 
                { 
                    Type = type, 
                    Payload = _player.ToString() 
                };
            }
        }

        private Message Logout(Message request, string type)
        {
            var _player = SerializeHelper.TryDeserialize<PlayerDTO>(request?.Payload ?? string.Empty);
            if (_player == null)
                return ErrorMessage("FormationViolation", "Payload is not a valid");

            _storage.Remove($"{_player.PlayerId}-Connection");

            return new Message() { Type = type };
        }

        private Message UpdateResources(Message request, string type)
        {
            var _resources = SerializeHelper.TryDeserialize<PlayerDTO>(request?.Payload ?? string.Empty);
            if (_resources == null)
                return ErrorMessage("FormationViolation", "Payload is not a valid");

#warning TODO: Add transaction
            foreach (var resource in _resources.Resources ?? new List<ResourceDTO>())
            {
                if (resource.Value < 0)
                    return ErrorMessage("FormationViolation", "Resource value is not a valid");

                _storage?.TrySetValue($"{_resources.PlayerId}-{resource.Type}", resource);
            }

            return new Message() { Type = type };
        }

        private Message GetResources(Message request, string type)
        {
            var _player = SerializeHelper.TryDeserialize<PlayerDTO>(request?.Payload ?? string.Empty);
            if (_player == null)
                return ErrorMessage("FormationViolation", "Payload is not a valid");

            if (_player.Resources == null)
                _player.Resources = new List<ResourceDTO>();

            var coins = (ResourceDTO?)_storage?.TryGetValue($"{_player.PlayerId}-{EResourceType.Coins}");
            if (coins == null)
            {
                coins = new ResourceDTO() { Type = EResourceType.Coins, Value = 0 };
                _storage?.TrySetValue($"{_player.PlayerId}-{EResourceType.Coins}", coins);
            }
            _player.Resources.Add(coins);

            var rolls = (ResourceDTO?)_storage?.TryGetValue($"{_player.PlayerId}-{EResourceType.Coins}");
            if (rolls == null)
            {
                rolls = new ResourceDTO() { Type = EResourceType.Rolls, Value = 0 };
                _storage?.TrySetValue($"{_player.PlayerId}-{EResourceType.Rolls}", rolls);
            }
            _player.Resources.Add(rolls);

            return new Message()
            {
                Type = type,
                Payload = _player.ToString()
            };
        }

        private Message SendGift(Message request, string type)
        {
            var _gift = SerializeHelper.TryDeserialize<GiftDTO>(request?.Payload ?? string.Empty);
            if (_gift == null)
                return ErrorMessage("FormationViolation", "Payload is not a valid");

            var giverResource = (ResourceDTO?)_storage?.TryGetValue($"{_gift.PlayerId}-{_gift.Resource.Type}");
            if (giverResource == null)
                return ErrorMessage("InternalError", "Giver resource not found");

            var receiverResource = (ResourceDTO?)_storage?.TryGetValue($"{_gift.FriendId}-{_gift.Resource.Type}");
            if (receiverResource == null)
                return ErrorMessage("InternalError", "Receiver resource not found");

            if (giverResource.Value < _gift.Resource.Value)
                return ErrorMessage("NotEnoughResources", "Giver has not enough resources");

            giverResource.Value -= _gift.Resource.Value;
            receiverResource.Value += _gift.Resource.Value;

#warning TODO: Add transaction
            _storage?.TrySetValue($"{_gift.PlayerId}-{_gift.Resource.Type}", giverResource);
            _storage?.TrySetValue($"{_gift.FriendId}-{_gift.Resource.Type}", receiverResource);

            return new Message() { Type = type };
        }
    }
}
