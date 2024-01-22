namespace TechHive.Korobeynikov.TestApp.Models.Contracts;

public interface IStorage
{
    /// <summary>
    /// Create or overwrite an entry in the storage
    /// </summary>
    /// <param name="key">An object identifying the entry</param>
    /// <param name="value">Object for storage</param>
    /// <returns>True if created or false</returns>
    bool TrySetValue(string key, object value, string? listOfKeys = null);

    /// <summary>
    /// Removes the object associated with the given key
    /// </summary>
    void Remove(string key, string? listOfKeys = null);

    /// <summary>
    /// Gets the item associated with this key if present
    /// </summary>
    /// <param name="key">An object identifying the requested entry</param>
    /// <returns>Object or Null if the key was not found.</returns>
    object? TryGetValue(string key, string? listOfKeys = null);
}
