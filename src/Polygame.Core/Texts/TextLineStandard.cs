using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sanet.Polygame.BaseObjects;

namespace Sanet.Polygame.Texts;

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

    private string _text;
    private Vector2 _size;

    private readonly SpriteFont _spriteFont;

    #endregion

    #region Properties
    /// <summary>
    /// Text to print
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                _size = MeasureText();
            }
        }
    }

    public Vector2 Size => _size * LocalScale;


    public Color TextColor
    {
        get;
        set;
    }

    #endregion

    #region Methods

    private Vector2 MeasureText()
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