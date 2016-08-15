using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.XNAEngine
{
    public static class FontsProvider
    {
        static List<CustomSpriteFont> _fonts = new List<CustomSpriteFont>();

        

        public static bool IsLoaded
        {
            get
            {
                return !_fonts.Any(f => !f.IsLoaded);
            }
        }

        public static void AddFont(string name)
        {
            _fonts.Add(new CustomSpriteFont(name));
        }

        public static void Load(ContentManager content, bool isLocal)
        {
            foreach (var font in _fonts)
                font.LoadContent(content,isLocal);
        }

        public static void LoadStep(ContentManager content, bool isLocal)
        {
            var font = _fonts.FirstOrDefault(f => !f.IsLoaded);
            if (font != null)
                font.LoadContent(content,isLocal);
        }

        public static CustomSpriteFont GetFont(string name)
        {
            return _fonts.FirstOrDefault(f=>f.FontName==name);
        }

        public static CustomSpriteFont GetDefaultFont()
        {
            return _fonts.FirstOrDefault();
        }
    }
}