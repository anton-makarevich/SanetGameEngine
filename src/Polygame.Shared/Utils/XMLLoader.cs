using System.Xml.Linq;

namespace Sanet.Polygame
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
