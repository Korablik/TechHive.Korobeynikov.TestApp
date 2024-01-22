namespace TechHive.Korobeynikov.TestApp.Models.Entities;

public class Resource
{
    public long Id { get; set; }

    public Guid PlayerId { get; set; }

    public EResourceType Type { get; set; }

    public int Value { get; set; }
}