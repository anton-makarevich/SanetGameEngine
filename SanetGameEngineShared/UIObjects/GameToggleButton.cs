using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class GameToggleButton :GameButton
    {
        public event EventHandler Switched;

        public bool IsChecked { get; set; }
        
        public GameToggleButton(string assetFile) 
            :base(assetFile,true)
        {

        }

        public GameToggleButton(string assetFile, bool isSpriteShhet)
            : base(assetFile, isSpriteShhet)
        {

        }

        public bool TwoSideSwitch { get; set; }


        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);

            if (isLocal == IsLocalContent)
            {
                _touchInput.OnClick += () =>
                {
                    if (!TwoSideSwitch)
                    {
                        if (IsChecked)
                            return;
                        IsChecked = true;
                    }
                    else
                    {
                        IsChecked = !IsChecked;
                    }
                    if (Switched != null)
                        Switched(this, null);
                };
            }
        }

        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);

            if (!CanDraw)
                return;

            if (IsChecked)
            {
                DrawRect = _pressedRect;
                if (_isAutoDark)
                    Color = _pressedColor;
            }
            else
            {
                DrawRect = _normalRect;
                if (_isAutoDark && IsEnabled && (InAnimation == null || !InAnimation.IsPlaying))
                {
                    Color = _normalColor;
                }
            }
            
            
        }
    }
}
