using System.Xml.Linq;
using Sanet.Polygame.Interfaces;

namespace Sanet.Polygame.Utils;

public static class XMLLoader
{
    public static IFileLoader FileLoader { get; set; } = new DefaultFileLoader();

    public static XDocument LoadDocument(string path)
    {
        return FileLoader.LoadDocument(path);
    }
}