using Sanet.XNAEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class Photo:GameSpriteTouchable
    {
        #region Constructor
        public Photo(Texture2D texture, string name):base(texture)
        {
            _ltAnchor = new GameSprite("anchor");
            _lbAnchor = new GameSprite("anchor");
            _rtAnchor = new GameSprite("anchor");
            _rbAnchor = new GameSprite("anchor");


            _ltAnchor.IsLocalContent = true;
            _rtAnchor.IsLocalContent = true;
            _lbAnchor.IsLocalContent = true;
            _rbAnchor.IsLocalContent = true;

            AddChild(_ltAnchor);
            AddChild(_lbAnchor);
            AddChild(_rtAnchor);
            AddChild(_rbAnchor);

            _ltAnchor.CanDraw = _lbAnchor.CanDraw = _rtAnchor.CanDraw = _rbAnchor.CanDraw = false;

            Initialize();
            
            OnClick/*Enter*/ += () =>
                Activate();

            
        }
        #endregion

        #region Events
        public event Action Activated;
        public event Action Deactivated;
        #endregion

        #region Fields
        GameSprite _ltAnchor;
        GameSprite _rtAnchor;
        GameSprite _lbAnchor;
        GameSprite _rbAnchor;
        #endregion

        #region Properties
        public bool IsActive
        {
            get
            {
                return _ltAnchor.CanDraw;
            }
        }

        
        #endregion

        #region Methods
        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);

            if (isLocal == IsLocalContent)
            {
                var halfAnchor = new Vector2(_rbAnchor.Width, _rbAnchor.Height) * 0.5f;

                _ltAnchor.Translate(-halfAnchor);
                _rbAnchor.Translate(new Vector2(Width, Height) - halfAnchor);
                _lbAnchor.Translate(-halfAnchor.X, Height - halfAnchor.Y);
                _rtAnchor.Translate(Width - halfAnchor.X, -halfAnchor.Y);
            }
        }

        public override void Update(RenderContext renderContext)
        {
            //_ltAnchor.CanDraw = _lbAnchor.CanDraw = _rtAnchor.CanDraw = _rbAnchor.CanDraw = IsActive;
            base.Update(renderContext);
        }

        public void Activate()
        {
            
            if (IsActive)
                return; //already active
            _ltAnchor.CanDraw = _lbAnchor.CanDraw = _rtAnchor.CanDraw = _rbAnchor.CanDraw = true;
            if (Activated != null)
                Activated();

        }

        public void Deactivate()
        {

            _ltAnchor.CanDraw = _lbAnchor.CanDraw = _rtAnchor.CanDraw = _rbAnchor.CanDraw = false;
            //IsMoving = false;
            if (Deactivated != null)
                Deactivated();
        }

        public void Move(Vector2 distance)
        {
            if (distance == Vector2.Zero)
                return;
            
            var pos = LocalPosition;
            pos += distance;
            Translate(pos);
        }

        public void RotateManually(Vector2 p1, Vector2 p2, Vector2 pp1, Vector2 pp2)
        {

            
            PivotPoint = (p1 + pp1 + p2 + pp2) / 4;
            var dir1 = p2 - PivotPoint;
            var a1 = (float)Math.Atan2(dir1.Y, dir1.X);
            var dir2 = pp2 - PivotPoint;
            var a2 = (float)Math.Atan2(dir2.Y, dir2.X);

            var angle = MathHelper.ToDegrees(a1 - a2);
            /*
            var dist1 = pp1 - p1;
            var dist2 = pp2 - p2;
            Scale(GetScale(dist1));*/
            Rotate(LocalRotation + angle);
        }


        Vector2 GetScale(Vector2 distance)
        {
            var size = new Vector2(Width, Height);
            var actSize = size * LocalScale;
            return LocalScale * actSize / (actSize - distance);
        }

        public override void Draw(RenderContext renderContext)
        {
            base.Draw(renderContext);

        }
        #endregion
    }
}
