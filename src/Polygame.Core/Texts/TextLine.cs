using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.UIObjects;

namespace Sanet.Polygame.Texts;

public class TextLine:GameObject2D
{
    #region Constructor
    public TextLine(CustomSpriteFont spriteFont,string text)
    {
        _spriteFont = spriteFont;
        //Scale(scale);
        Text=text;
    }

    public TextLine( string text):this(FontsProvider.GetDefaultFont(),text)
    { }
    #endregion

    #region Fields
    string _text;
    Vector2 _size;
        
    CustomSpriteFont _spriteFont;

    GameSprite _sprite;
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

    public GameSprite RenderedSprite => _sprite;

    public Color TextColor
    {
        get => _sprite.Color;
        set => _sprite.Color = value;
    }

    #endregion

    #region Methods
    Vector2 MeasureText()
    {
        var x = 0;
        var y = 0;
        foreach (var ch in _text.Trim())
        {
            var rect = _spriteFont.GetCharPosition(ch);
            if (y < rect.Height)
                y = rect.Height;

            x += rect.Width;
        }
        return new Vector2(x, y);
    }

    public void DrawSprite(RenderContext renderContext,  Color fontColor)
    {
        //
        var result= new RenderTarget2D(renderContext.GraphicsDevice, (int)_size.X, (int)_size.Y);

        renderContext.GraphicsDevice.SetRenderTarget(result);
        renderContext.GraphicsDevice.Clear(Color.Transparent);

        renderContext.SpriteBatch.Begin();

        var pos = Vector2.Zero;
        _spriteFont.FontSprite.Color = Color.White;
        _spriteFont.FontSprite.WorldScale = Vector2.One;
        foreach (var ch in Text)
        {
            _spriteFont.FontSprite.WorldPosition = pos;
            var rect = _spriteFont.GetCharPosition(ch);
            _spriteFont.FontSprite.DrawRect = rect;
            _spriteFont.FontSprite.Draw(renderContext);
            pos.X += rect.Width;
        }
            

        renderContext.SpriteBatch.End();
             
        /*using (SpriteBatch spriteB = new SpriteBatch(renderContext.GraphicsDevice))
        {
            spriteB.Begin();
            var pos = Vector2.Zero;
            foreach (char ch in Text)
            {
                _spriteFont.FontSprite.WorldPosition = pos;
                var rect = _spriteFont.GetCharPosition(ch);
                spriteB.Draw(_spriteFont.Texture,pos,rect,Color.White);
                pos.X += rect.Width;
            }
            spriteB.End();
        }*/


        //Release the GPU back to drawing to the screen
        renderContext.GraphicsDevice.SetRenderTarget(null);
        //if (_sprite != null)
        //    RemoveChild(_sprite);

        _sprite = new GameSprite(result as Texture2D);
        _sprite.Initialize();
        _sprite.LoadContent(renderContext.GlobalContentManager, false);
        _sprite.Color = fontColor;
        //AddChild(_sprite);
    }


    #endregion
}