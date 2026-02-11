namespace BelegtesBrot.FileSystem;

public interface IFile
{
    string Name { get; }
    FileInfo FileInfo { get; }
    Task<T?> LoadAsync<T>();
    Task SaveAsync(object data);
}