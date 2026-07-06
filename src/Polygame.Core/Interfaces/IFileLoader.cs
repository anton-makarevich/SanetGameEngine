using System.Xml.Linq;

namespace Sanet.Polygame.Interfaces;

public interface IFileLoader
{
    XDocument LoadDocument(string path);
}