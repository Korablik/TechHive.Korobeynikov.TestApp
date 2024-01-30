using System.Data.SQLite;
using TechHive.Korobeynikov.TestApp.Models.Contracts;
using TechHive.Korobeynikov.TestApp.Models.Entities;

namespace TechHive.Korobeynikov.TestApp.SQLite;

public class SQLiteDB : IDB
{
    private readonly string _connectionString;
    public SQLiteDB(string connectionString)
    {
        _connectionString = connectionString;
        Seed();
    }

    public Player? GetPlayerById(Guid playerId)
    {
        Player? player = null;
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT * FROM Players WHERE PlayerId = @PlayerId";
                command.Parameters.AddWithValue("@PlayerId", playerId.ToString());
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if(reader.HasRows)
                        player = new Player() { Id = playerId };

                    while (reader.Read())
                    {
                        var _udid = reader["UDID"]?.ToString();
                        if (!string.IsNullOrEmpty(_udid))
                            player.UDIDs.Add(_udid.ToString());
                    }
                }
            }
            connection.Close();
        }
        return player;
    }

    public Player? GetPlayerByUDID(string udid)
    {
        Player? player = null;
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT * FROM Players WHERE UDID = @UDID";
                command.Parameters.AddWithValue("@UDID", udid.ToString());
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    { 
                        player = new Player();
                        player.UDIDs.Add(udid); 
                        
                        while (reader.Read())
                        {
                            var _id = reader.GetString(1);
                            if (!string.IsNullOrEmpty(_id))
                                player.Id = Guid.Parse(_id);
                        }
                    }
                }
            }
            connection.Close();
        }
        return player;
    }

    public void SetOrUpdatePlayer(Player player)
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            foreach (var item in player.UDIDs)
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT INTO Players (UDID, PlayerId) VALUES (@UDID, @PlayerId)";
                    command.Parameters.AddWithValue("@PlayerId", player.Id.ToString());
                    command.Parameters.AddWithValue("@UDID", item.ToString());
                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }
    }

    private void Seed()
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Players (UDID TEXT PRIMARY KEY, PlayerId TEXT)";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
