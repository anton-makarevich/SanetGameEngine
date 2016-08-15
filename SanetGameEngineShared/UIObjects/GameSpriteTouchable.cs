using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class GameSpriteTouchable : GameSprite
    {
        #region Constructor
        public GameSpriteTouchable(string assetFile) :base(assetFile)
        {
            IsEnabled = true;
        }
        public GameSpriteTouchable(Texture2D texture)
            : base(texture)
        {
            IsEnabled = true;
        }
        #endregion

        #region Events
        public event Action OnClick;
        public event Action OnEnter;
        public event Action OnLeave;
        #endregion

        #region Fields
        protected TouchInput _touchInput;
        #endregion

        #region Properties
        public TouchInput Touch
        {
            get
            {
                return _touchInput;
            }
        }

        public virtual bool IsEnabled { get; set; }
        #endregion

        #region Methods
        public override void Initialize()
        {
            base.Initialize();
            
        }
        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager, isLocal);
            if (isLocal == IsLocalContent)
            {
                _touchInput = new TouchInput(this);
                _touchInput.OnClick += () =>
                {
                    if (OnClick != null)
                        OnClick();
                };
                _touchInput.OnEnter += () =>
                {
                    if (OnEnter != null)
                        OnEnter();
                };
                _touchInput.OnLeave += () =>
                {
                    if (OnLeave != null)
                        OnLeave();
                };
            }
        } 
        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);
            if (_touchInput != null && CanDraw && (Scene==null || !Scene.IsBusy) && IsEnabled)
                _touchInput.Update(renderContext);
        }
        #endregion
    }
}
