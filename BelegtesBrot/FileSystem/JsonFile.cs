using System.Text.Json;

namespace BelegtesBrot.FileSystem;

public abstract class JsonFile : IFile
{
    private const string Extension = ".json";

    protected JsonFile(DirectoryInfo baseFolder)
    {
        var absolutePath = Path.Combine(baseFolder.FullName, Name);
        FileInfo = new FileInfo(absolutePath);
    }

    public abstract string Name { get; }
    public FileInfo FileInfo { get; init; }

    public async Task<T?> LoadAsync<T>()
    {
        if (!FileInfo.Exists) return default;
        await using var fs = new FileStream(FileInfo.FullName, FileMode.Open, FileAccess.Read);
        try
        {
            return await JsonSerializer.DeserializeAsync<T>(fs);
        }
        catch (JsonException e)
        {
            return default;
        }
    }

    public async Task SaveAsync(object data)
    {
        var absolutePath = Path.ChangeExtension(FileInfo.FullName, Extension);

        await using var sw = new FileStream(absolutePath, FileMode.Create, FileAccess.Write);

        await JsonSerializer.SerializeAsync(sw, data);
    }
}