using System.Xml.Linq;

namespace Sanet.Polygame
{
    public class DefaultFileLoader : IFileLoader
    {
        public XDocument LoadDocument(string path)
        {
            return XDocument.Load(path);
        }
    }
}
