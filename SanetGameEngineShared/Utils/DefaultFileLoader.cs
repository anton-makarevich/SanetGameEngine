using System.Xml.Linq;

namespace Sanet.XNAEngine
{
    public class DefaultFileLoader : IFileLoader
    {
        public XDocument LoadDocument(string path)
        {
            return XDocument.Load(path);
        }
    }
}
