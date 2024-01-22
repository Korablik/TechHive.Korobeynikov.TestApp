using TechHive.Korobeynikov.TestApp.Models.Entities;

namespace TechHive.Korobeynikov.TestApp.Models.Contracts;

public interface IDB
{
    Player? GetPlayerById(Guid playerId);
    Player? GetPlayerByUDID(string udid);

    void SetOrUpdatePlayer(Player player);
}
