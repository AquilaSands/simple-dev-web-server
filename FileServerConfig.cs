using System.IO;

namespace DevWebServer
{
    public class FileServerConfig
    {
        public string BaseDirectory { get; set; } = Directory.GetCurrentDirectory();
        public bool DirectoryBrowsing { get; set; }
    }
}
