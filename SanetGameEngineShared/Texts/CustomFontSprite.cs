 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.XNAEngine
{
    /// <summary>
    /// Custom font sprite
    /// Bsed on output from FontSprite2 app:
    /// 1) font texture
    /// 2) xml description of chars positions
    /// </summary>
    public class CustomSpriteFont : GameObject2D
    {
        #region Fields
        Dictionary<char, Rectangle> _charPositions= new Dictionary<char, Rectangle>();
        GameSprite _sprite;
        #endregion

        #region Properties
        public bool IsLoaded { get; private set; }
        public string FontName { get; private set; }
        public float FontHeight
        {
            get
            {
                if (_charPositions.Count > 0)
                    return _charPositions.Values.First().Height;
                return 0;
            }
        }

        public GameSprite FontSprite
        {
            get
            {
                return _sprite;
            }
        }

        public Texture2D Texture
        {
            get
            {
                if (_sprite == null)
                    return null;
                return _sprite.Texture;
            }
        }
        #endregion

        public CustomSpriteFont(string name)
        {
            FontName = name;
            _sprite = new GameSprite(name);
            this.AddChild(_sprite);
        }

        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
          
            base.LoadContent(contentManager,isLocal);
            if (isLocal == IsLocalContent)
            {
                //load chars positions from xml file
                var charPositions = XMLLoader.LoadDocument(string.Format("Data/{0}.xml", FontName));
                var charsList = charPositions.Descendants("character").ToList();
                if (charsList != null)
                {
                    foreach (var query in charsList)
                    {
                        var key = int.Parse(query.Attribute("key").Value);
                        var x = int.Parse(query.Element("x").Value);
                        var y = int.Parse(query.Element("y").Value);
                        var w = int.Parse(query.Element("width").Value);
                        var h = int.Parse(query.Element("height").Value);
                        _charPositions.Add((char)key, new Rectangle(x, y, w, h));
                    }
                }
            }

            IsLoaded=true;
        }

        public void Draw(RenderContext renderContext,string line, Vector2 position, Color color, float rotation, Vector2 scale)
        {
            var pos = position;
            _sprite.WorldScale=scale;
            _sprite.Color = color;
            _sprite.WorldRotation=rotation;
            _sprite.Z = 1000;
            foreach (char ch in line)
            {
                _sprite.WorldPosition=pos;
                var rect = GetCharPosition(ch);
                _sprite.DrawRect = rect;
                _sprite.Draw(renderContext);
                pos.X += rect.Width*scale.X;
            }
        }

        public Rectangle GetCharPosition(char ch)
        {
            if (_charPositions.ContainsKey(ch))
                return _charPositions[ch];
            return _charPositions.Values.First();
        }
               
        public Vector2 MeasureString(string text)
        {
            int x = 0;
            int y = 0;
            foreach (char ch in text.Trim())
            {
                var rect = GetCharPosition(ch);
                if (y < rect.Height)
                    y = rect.Height;

                x += rect.Width;
            }
            return new Vector2(x, y);
        }
    }
}