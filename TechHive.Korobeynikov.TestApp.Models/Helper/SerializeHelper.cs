using System.Text.Json;

namespace TechHive.Korobeynikov.TestApp.Models.Helper;

public static class SerializeHelper
{
    #region JSON
    private static JsonSerializerOptions? _jsonSerializerOptions;
    public static JsonSerializerOptions SerializerOptions
    {
        get
        {
            if (_jsonSerializerOptions == null)
                _jsonSerializerOptions = InitSerializerOptions();

            return _jsonSerializerOptions;
        }
        set
        {
            _jsonSerializerOptions = value;
        }
    }

    public static string SerializeObject(object obj) => JsonSerializer.Serialize(obj, SerializerOptions);
    /// <summary>
    /// Return empty string if not Serialize
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string TrySerializeObject(object obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, SerializerOptions);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static T? Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, SerializerOptions);
    /// <summary>
    /// Return null if not Deserialize
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T? TryDeserialize<T>(string value) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(value, SerializerOptions);
        }
        catch
        {
            return null;
        }
    }

    private static JsonSerializerOptions InitSerializerOptions() => new JsonSerializerOptions();
    #endregion
}
