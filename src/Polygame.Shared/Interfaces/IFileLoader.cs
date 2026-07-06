using System.Xml.Linq;

namespace Sanet.Polygame
{
    public interface IFileLoader
    {
        XDocument LoadDocument(string path);
    }
}
