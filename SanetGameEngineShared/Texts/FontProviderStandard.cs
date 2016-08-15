using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.XNAEngine
{
    public static class FontProviderStandard
    {
        static List<SpriteFont> _fonts = new List<SpriteFont>();
        static List<string> _fontNames = new List<string>();

        public static bool IsLoaded
        {
            get;
            private set;
        }

        public static void AddFont(string name)
        {
            _fontNames.Add(name);
        }

        public static void Load(ContentManager content)
        {
                foreach (var font in _fontNames)
                    _fonts.Add(content.Load<SpriteFont>(font));
            IsLoaded = true;
        }
                
        public static SpriteFont GetFont(string name)
        {
            var i = _fontNames.IndexOf(name);
            if (i > -1)
            {
                var rv = _fonts[i];
                    return rv;
            }
            return GetDefaultFont();
        }

        public static SpriteFont GetDefaultFont()
        {
            return _fonts.FirstOrDefault();
        }
    }
}