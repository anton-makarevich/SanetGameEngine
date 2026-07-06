using System.Xml.Linq;
using Sanet.Polygame.Interfaces;

namespace Sanet.Polygame.Utils;

public class DefaultFileLoader : IFileLoader
{
    public XDocument LoadDocument(string path)
    {
        return XDocument.Load(path);
    }
}