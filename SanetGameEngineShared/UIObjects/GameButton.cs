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
    public class GameButton : GameSpriteTouchable
    {
        #region Constructor
        public GameButton(string assetFile) :
            this(assetFile, false) { }

        public GameButton(string assetFile, bool isSpriteSheet) :
            base(assetFile)
        {
            _isSpriteSheet = isSpriteSheet;
            ApplyColorToChildren = true;
        }
        #endregion

        #region Fields
        private static string commonButtonClickSound;

        private string _buttonClickSound;

        private bool _isSpriteSheet;
        protected bool _isAutoDark = false;

        protected Color _normalColor;
        protected Color _pressedColor;
        protected Color _disabledColor = Color.Black;

        protected Rectangle? _normalRect, _pressedRect;
                
        #endregion

        #region Properties

        public static string CommonButtonSoundsFile
        {
            set
            {
                commonButtonClickSound = value;
            }
        }

        public string ButtonSoundsFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_buttonClickSound))
                    return _buttonClickSound;
                return commonButtonClickSound;
            }
        }

        public Color PressedColor
        {
            get
            {
                return _pressedColor;
            }
            set
            {
                _pressedColor = value;
                _normalColor = Color;
                _isAutoDark = true;
            }
        }

        public Color DisabledColor
        {
            get
            {
                return _disabledColor;
            }
            set
            {
                _disabledColor = value;
                _normalColor = Color;
                if (!IsEnabled)
                    Color = value;
            }
        }

        public string Action { get; set; }

        public override bool IsEnabled
        {
            get
            {
                return base.IsEnabled;
            }
            set
            {
                base.IsEnabled = value;
                if (DisabledColor != Color.Black)
                {
                    if (value)
                        Color = _normalColor;
                    else
                        Color = _disabledColor;
                }
            }
        }

        public bool ForcePressed
        { get; set; }

        #endregion

        #region Methods
        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);
            if (isLocal == IsLocalContent)
            {
                //Set Dimensions after the button texture is loaded, otherwise we can't extract the width and height
                if (_isSpriteSheet)
                {
                    CreateBoundingRect((int)Width / 2, (int)Height);
                    _normalRect = new Rectangle(0, 0, (int)(Width / 2f), (int)(Height));
                    _pressedRect = new Rectangle((int)(Width / 2f), 0, (int)(Width / 2f), (int)(Height));
                }
                else CreateBoundingRect((int)Width, (int)Height);
                //Actually it's bad practice - better to make one more wrapper for button and put it there
                _touchInput.OnClick += () =>
                {
                    SoundsProvider.PlaySound(ButtonSoundsFile);
                };
            }
        }

        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);

            if (!CanDraw )
                return;

            if (!_touchInput.IsPressed && !ForcePressed)
            {
                DrawRect = _normalRect;
                if (_isAutoDark && IsEnabled && (InAnimation==null || !InAnimation.IsPlaying))
                {
                    Color = _normalColor;
                }
            }
            else
            {
                DrawRect = _pressedRect;
                if (_isAutoDark)
                    Color = _pressedColor;
            }
                        

        }

        public void SetNormalColor(Color normalColor)
        {
            _normalColor = normalColor;
        }
        #endregion
    }
}
