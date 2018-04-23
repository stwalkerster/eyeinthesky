namespace EyeInTheSky.Services
{
    using System.IO;
    using EyeInTheSky.Services.Interfaces;

    public class FileService : IFileService
    {
        public bool FileExists(string path)
        {
            return new FileInfo(path).Exists;
        }

        public Stream GetWritableStream(string path)
        {
            return new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        }

        public Stream GetReadableStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}