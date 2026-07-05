using Android.App;
using System.Xml.Linq;

namespace Sanet.XNAEngine
{
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
}
