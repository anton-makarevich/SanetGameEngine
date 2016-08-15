using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sanet.XNAEngine
{
    public static class XMLLoader
    {
        public static XDocument LoadDocument(string path)
        {
#if ANDROID
            return XDocument.Load(Game.Activity.Assets.Open(path));
#else
            return XDocument.Load(path);
#endif
        }
    }
}
