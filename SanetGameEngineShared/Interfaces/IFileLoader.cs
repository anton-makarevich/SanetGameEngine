using System.Xml.Linq;

namespace Sanet.XNAEngine
{
    public interface IFileLoader
    {
        XDocument LoadDocument(string path);
    }
}
