using TechHive.Korobeynikov.TestApp.Models.Helper;

namespace TechHive.Korobeynikov.TestApp.Models.DTO;

public class SerializedDTO
{
    public override string ToString() => SerializeHelper.SerializeObject(this);
}
