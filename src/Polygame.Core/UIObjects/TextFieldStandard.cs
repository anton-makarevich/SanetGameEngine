using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Sanet.Polygame.Animations;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Input;
using Sanet.Polygame.Texts;

namespace Sanet.Polygame.UIObjects;

public class TextFieldStandard : GameObject2D
{
    #region Events
    public event Action Activated;
    public event Action Deactivated;

    public event Action TextChanged;
    #endregion

    #region Constructor
    public TextFieldStandard(string asset, string font, Vector2 bgScale)
    {
        _bg = new GameSpriteTouchable(asset);
        _bg.Color = Microsoft.Xna.Framework.Color.Transparent;
        _bg.Scale(bgScale);
        //_bg.CanDraw = false;
        _bg.OnClick += () =>
        {
            if (_bg.Touch.PressTime < 300 ||!IsMoveable)
            {
                Activate();
            }
            else
                StartMoving();
        };
        AddChild(_bg);

        _text = new TextBlockStandard(font);
            
        TextString = "";
        AddChild(_text);
        _cursor = new GameSprite("WhitePixel");
        _cursor.Color = Microsoft.Xna.Framework.Color.Black;
        _cursor.OpacityAnimation = new OpacityAnimation(1, 0, 350);
        _cursor.CanDraw = false;
        AddChild(_cursor);

        _textFieldGuid = Guid.NewGuid();

            
    }

        
    #endregion

    #region Fields

    private readonly TextBlockStandard _text;
    private readonly GameSpriteTouchable _bg;

    private readonly GameSprite _cursor;
    //Textfield  test
    private readonly Guid _textFieldGuid;

    private Color _fontColor;
    private Color _bgColor = Color.LightGray;

    private GameObjectOrientation _cursorType = GameObjectOrientation.Vertical;

    private float _fontSize;

    private readonly StringBuilder _textBuilder = new(64);

    private string _tipString;

    private InputFormat _inputFormat;
    #endregion

    #region Properties
    public bool IsActive => _cursor.CanDraw;

    public Color FontColor
    {
        get => _fontColor;
        set => _fontColor = value;
    }

    public Color BackgroundColor
    {
        get => _bgColor;
        set
        {
            _bgColor = value;
            if (IsActive)
                _bg.Color = value;
        }
    }

    public bool IsMoving { get; private set; }

    public bool IsMoveable { get; set; }

    public float FontSize
    {
        get => _fontSize;
        set => _fontSize = value;
    }

    public string Text => _textBuilder.ToString();

    public int MaxChars { get; set; }

    public GameObjectOrientation CursorType
    { 
        get => _cursorType;
        set => _cursorType = value;
    }

    public string TipText { get; set; }
    public string TipTextLabel
    {
        get => _tipString;
        set => _tipString= value;
    }

    public bool IsPassword {get;set;}

    public InputFormat InputFormat
    {
        get => _inputFormat;
        set
        {
            if (_inputFormat != value)
                _inputFormat = value;
        }
    }

    public string TextString
    {
        private get => _textBuilder.ToString();
        set
        {
            _textBuilder.Clear();
            _textBuilder.Append(value);
        }
    }

    public Guid TextFieldGuid => _textFieldGuid;

    public TouchInput Touch => _bg.Touch;

    #endregion

    #region Methods
    public override void LoadContent(ContentManager contentManager, bool isLocal)
    {
        base.LoadContent(contentManager,isLocal);
        if (isLocal == IsLocalContent)
        {
            _text.Rect = new Microsoft.Xna.Framework.Rectangle(0, 5, (int)( _bg.LocalScale.X), (int)( _bg.LocalScale.Y ));
            _text.LineSpacing = (int)_bg.LocalScale.Y;
        }
    }
    public override void Update(RenderContext renderContext)
    {
        //If password - replace chars with password char
        //TODO maybe need to show last char for few seconds
        //or alternatively have button to see typed password when pressed
        if (_textBuilder.Length > 0)
        {
            _text.Text = (IsPassword) ? new String('*', _textBuilder.Length) : _textBuilder.ToString();
            _text.TextColor = FontColor;
            _text.FontScale = Vector2.One*FontSize;
                
            _text.Alignment = TextAlignment.TopLeft;
        }
        else
        {
            //if no text entered we can show "tip" if available
            if (!string.IsNullOrEmpty(TipText) && !IsActive)
            {
                if (_text.Text != TipText)
                {
                    _text.Text = TipText;
                    _text.TextColor = new Color(FontColor, 0.6f);
                    //var sc = (_bg.LocalScale.X * FontSize) / (_text.LineSize.X);
                    //sc = Math.Min(0.75f, sc);
                    _text.FontScale = Vector2.One * FontSize * 0.8f;


                    _text.Alignment = TextAlignment.Middle;
                }
            }
            else 
            {
                _text.Text = "";
            }
        }

        base.Update(renderContext);

        //cursor
        if (IsActive)
        {
            var h = _bg.LocalScale.Y;//_text.Rect.Height;//.FontHeight;
            var dh = -h * 0.5f;
            if (_cursorType == GameObjectOrientation.Vertical)
            {
                _cursor.Scale(5, h);
            }
            else
            {
                dh = h;
                _cursor.Scale(35, 7);
            }
            var pos = _text.LocalPosition + new Vector2(_text.LineSize.X, dh);
            _cursor.Translate(pos);
        }
        else            
        {
            if (IsMoveable && _bg.Touch.IsPressed && _bg.Touch.PressTime>300)
            {
                StartMoving();
            }
        }
            
    }

    private void AddChar(Guid id, char key)
    {
        if (_text.LineSize.X > _bg.LocalScale.X - 10)
            return;

        if (MaxChars != 0 && _text.Text.Length == MaxChars)
            return;

        var keyString = key.ToString();

        if (_textFieldGuid == id)
        {
            if (InputFormat == InputFormat.Numeric)
            {
                int outi;
                if (!int.TryParse(keyString, out outi))
                    return;
            }
            else if (InputFormat == InputFormat.Capital)
                keyString = keyString.ToUpper();

            _textBuilder.Append(keyString);
            if (TextChanged != null)
                TextChanged();
        }
    }

    private void DeleteKey(Guid id)
    {
        if (_textFieldGuid == id && _textBuilder.Length > 0)
        {
            _textBuilder.Length -= 1;
                
            if (TextChanged != null)
                TextChanged();
        }
    }

    public void Activate()
    {
        if (_cursor.CanDraw)
            return; //already active
        KeyboardManager.RequestTextFieldActivation(_textFieldGuid, InputFormat);
        _cursor.OpacityAnimation.PlayAnimation(11000);
        _cursor.CanDraw =  true;
        _bg.Color = BackgroundColor;

        KeyboardManager.OnKeyInput += AddChar;
        KeyboardManager.OnKeyDelete += DeleteKey;

        if (Activated != null)
            Activated();
            
    }

    public void StartMoving()
    {
        if (IsMoving)
            return; //already active
        _bg.Color = Microsoft.Xna.Framework.Color.LightGreen;
        IsMoving = true;
        if (Activated != null)
            Activated();
    }

    public void Deactivate()
    {
           
        KeyboardManager.RequestTextFieldDeActivation(_textFieldGuid);
        _cursor.OpacityAnimation.StopAnimation();
        _cursor.CanDraw = /*_bg.CanDraw =*/ false;
        _bg.Color = Microsoft.Xna.Framework.Color.Transparent;
        IsMoving = false;
        KeyboardManager.OnKeyInput -= AddChar;
        KeyboardManager.OnKeyDelete -= DeleteKey;
        if (Deactivated != null)
            Deactivated();
    }

    public void Clear()
    {
        _textBuilder.Clear();
    }

    public void OverrideText(string text)
    {
        _textBuilder.Clear();
        _textBuilder.Append(text);
    }
        
    #endregion
}