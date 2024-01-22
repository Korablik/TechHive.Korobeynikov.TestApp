namespace TechHive.Korobeynikov.TestApp.Models.Entities;

public class Player
{
    public Player()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public List<string> UDIDs { get; private set; } = new();
}
