using Sanet.XNAEngine.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
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
        Vector2 _size;

        GameButton _button;
        TextBlockStandard _text;

        Color _textActiveColor;
        Color _textNormalColor;
        #endregion

        #region Properties
        public TouchInput Touch
        {
            get
            {
                return _button.Touch;
            }
            
        }
        
        public Color PressedColor
        {
            get
            {
                return _button.PressedColor;
            }
            set
            {
                _button.PressedColor=value;
            }
        }

        public Color DisabledColor
        {
            get
            {
                return _button.DisabledColor;
            }
            set
            {
                _button.DisabledColor = value;
                
            }
        }

        public Color TextActiveColor
        {
            get
            {
                return _textActiveColor;
            }
            set
            {
                _textActiveColor = value;
            }
        }

        public string Action 
        {
            get
            {
                return _button.Action;
            }
            set
            {
                _button.Action = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _button.IsEnabled;
            }
            set
            {
                _button.IsEnabled = value;
            }
        }

        public string Text
        {
            get
            {
                return _text.Text;
            }
            set
            {
                _text.Text = value;
            }
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
}
