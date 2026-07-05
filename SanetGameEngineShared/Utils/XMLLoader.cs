using System.Xml.Linq;

namespace Sanet.XNAEngine
{
    public static class XMLLoader
    {
        public static IFileLoader FileLoader { get; set; } = new DefaultFileLoader();

        public static XDocument LoadDocument(string path)
        {
            return FileLoader.LoadDocument(path);
        }
    }
}
