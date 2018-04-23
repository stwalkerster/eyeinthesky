namespace EyeInTheSky.Services.Interfaces
{
    using System.IO;

    public interface IFileService
    {
        bool FileExists(string path);
        Stream GetWritableStream(string path);
        Stream GetReadableStream(string path);
    }
}