namespace Byway.Application.Interfaces.Services;

/// <summary>
/// Abstraction over "where images live" so Application never depends on
/// wwwroot/disk details directly. Only a local-disk implementation exists
/// today (see Infrastructure), but Application is insulated from that
/// choice — swapping to S3/Blob storage later means adding a new
/// implementation of this interface, not touching any service that uses it.
/// </summary>
public interface IFileStorageService
{
    /// <summary>Saves the file and returns the string path to store on the entity (per the requirement: path, not binary).</summary>
    Task<string> SaveAsync(Stream content, string fileName, string containerName, CancellationToken ct = default);

    void Delete(string path);
}
