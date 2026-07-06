using System.Xml.Linq;
using Android.App;
using Sanet.Polygame.Interfaces;

namespace Sanet.Polygame.Android.Engine.Utils;

public class AndroidFileLoader : IFileLoader
{
    private readonly Activity _activity;

    public AndroidFileLoader(Activity activity)
    {
        _activity = activity;
    }

    public XDocument LoadDocument(string path)
    {
        return XDocument.Load(_activity.Assets.Open(path));
    }
}