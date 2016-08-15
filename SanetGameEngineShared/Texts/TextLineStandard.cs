using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class TextLineStandard:GameObject2D
    {
        #region Constructor
        public TextLineStandard(SpriteFont spriteFont,string text)
        {
            _spriteFont = spriteFont;
            //Scale(scale);
            Text=text;
        }

        public TextLineStandard(string text)
            : this(FontProviderStandard.GetDefaultFont(), text)
        { }
        #endregion

        #region Fields
        string _text;
        Vector2 _size;
        
        SpriteFont _spriteFont;

        #endregion

        #region Properties
        /// <summary>
        /// Text to print
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    _size = MeasureText();
                }
            }
        }

        public Vector2 Size
        {
            get
            {
                return _size * LocalScale;
            }
        }


        public Color TextColor
        {
            get;
            set;
        }

        #endregion

        #region Methods
        Vector2 MeasureText()
        {
            return _spriteFont.MeasureString(_text);
        }

        public override void Draw(RenderContext renderContext)
        {
            base.Draw(renderContext);
            renderContext.SpriteBatch.DrawString(_spriteFont, Text, WorldPosition, TextColor, WorldRotation, Vector2.Zero, WorldScale, SpriteEffects.None, 1.0f);
        }
        
        #endregion
    }
}
