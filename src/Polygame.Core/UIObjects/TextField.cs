using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Sanet.Polygame.Animations;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Input;
using Sanet.Polygame.Texts;

namespace Sanet.Polygame.UIObjects;

public class TextField : GameObject2D
{
    #region Events
    public event Action Activated;
    public event Action Deactivated;

    public event Action TextChanged;
    #endregion

    #region Constructor
    public TextField(string asset, string font, Vector2 bgScale)
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

        _text = new TextPrinter(font);
            
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
    TextPrinter _text;
    GameSpriteTouchable _bg;
    GameSprite _cursor;
    //Textfield  test
    Guid _textFieldGuid;

    Color _fontColor;
    Color _bgColor = Color.LightGray;

    GameObjectOrientation _cursorType = GameObjectOrientation.Vertical;

    float _fontSize;

    string _textString;

    string _tipString;

    InputFormat _inputFormat;
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

    public string Text => _textString;

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
        private get => _textString;
        set => _textString = value;
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
        if (!string.IsNullOrEmpty(_textString))
        {
            _text.Text = (IsPassword) ? new String('*', _textString.Length) : _textString;
            _text.TextColor = FontColor;
            _text.FontScale = FontSize;
                
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
                    _text.FontScale = FontSize * 0.8f;


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
            var pos = (_text.LinePosition == Vector2.Zero) ?
                _text.LocalPosition + new Vector2(0, dh) :
                /*(_text.LinePosition * renderContext.DeviceScale - WorldPosition)*/_text.LocalPosition + new Vector2(_text.LineSize.X, dh);
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

    void AddChar(Guid id, char key)
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

            TextString += keyString;
            if (TextChanged != null)
                TextChanged();
        }
    }

    void DeleteKey(Guid id)
    {
        if (_textFieldGuid == id && TextString.Length>0)
        {
            TextString = _text.Text.Remove(_text.Text.Length - 1);
                
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
        TextString = "";
    }

    public void OverrideText(string text)
    {
        TextString = text;
    }
        
    #endregion
}