using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Input;
using Sanet.Polygame.Texts;

namespace Sanet.Polygame.UIObjects;

public class GameTextButton :GameObject2D
{
    #region Events
    public event Action OnClick;
    #endregion

    #region Constructor
    public GameTextButton(GameButton button, TextBlockStandard text, Vector2 size = new Vector2()) 
    {
        _size = size;

        _button = button;
        _text = text;
        _textNormalColor = _text.TextColor;
        AddChild(button);
        AddChild(text);
    }
    #endregion

    #region Fields

    private Vector2 _size;

    private readonly GameButton _button;
    private readonly TextBlockStandard _text;

    private Color _textActiveColor;
    private readonly Color _textNormalColor;
    #endregion

    #region Properties
    public TouchInput Touch => _button.Touch;

    public Color PressedColor
    {
        get => _button.PressedColor;
        set => _button.PressedColor=value;
    }

    public Color DisabledColor
    {
        get => _button.DisabledColor;
        set => _button.DisabledColor = value;
    }

    public Color TextActiveColor
    {
        get => _textActiveColor;
        set => _textActiveColor = value;
    }

    public string Action 
    {
        get => _button.Action;
        set => _button.Action = value;
    }

    public bool IsEnabled
    {
        get => _button.IsEnabled;
        set => _button.IsEnabled = value;
    }

    public string Text
    {
        get => _text.Text;
        set => _text.Text = value;
    }
    #endregion

    #region Methods
    public override void LoadContent(ContentManager contentManager, bool isLocal)
    {
        base.LoadContent(contentManager,isLocal);
        if (isLocal == IsLocalContent)
        {
            if (_size==Vector2.Zero)
            {
                _size = _button.Size;
            }
            else
            {
                _button.Scale(_size / _button.Size);
            }
            _text.Rect = new Rectangle(0, 0, (int)_size.X, (int)_size.Y);
            _text.Alignment = TextAlignment.Middle;
            _button.OnClick += () =>
            {
                if (OnClick != null)
                    OnClick();
            };
        }
    }

    public override void Update(RenderContext renderContext)
    {
        base.Update(renderContext);

        if (!CanDraw )
            return;

        if (_button.Touch.IsPressed)
        {
            _text.TextColor = _textActiveColor;
        }
        else
        {
            _text.TextColor = _textNormalColor;
        }
                        

    }

    #endregion
}