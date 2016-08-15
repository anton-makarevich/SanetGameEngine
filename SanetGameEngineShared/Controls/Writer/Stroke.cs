 
 
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Sanet.XNAEngine.Controls
{
    public class Stroke : GameObject2D
    {

        #region Fields
        GameSprite _finalStroke;
        #endregion

        #region Properties
        public float MinX
        {
            get;
            set;
        }
        public float MinY
        {
            get;
            set;
        }
        public float MaxX
        {
            get;
            set;
        }
        public float MaxY
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public override void AddChild(IGameObject child)
        {
            //Stroke may contain only LineSegments (or maybe more general parent class if needed)

            if (!(child is LineSegment || child is PointSegment))
                return;
            //TODO: add some logging?

            base.AddChild(child);
        }

        public void CreateBoundingRect(int x, int y, int width, int height)
        {
            BoundingRect = new Rectangle(x, y, width , height );
        }

        /// <summary>
        /// Renders all individual segments into single stroke texture
        /// </summary>
        public void Merge()
        {
            var renderContext = SceneManager.RenderContext;
            var unScale = Vector2.One/renderContext.DeviceScale;
            
            Vector2 pos = new Vector2(MinX, MinY) * unScale;
            foreach (var child in Children)
            {
                foreach (var c in child.Children)
                {
                    c.Translate(-pos + c.LocalPosition - Parent.WorldPosition *unScale);
                }
            }

            Update(renderContext);
            _finalStroke = new GameSprite(ToTexture(renderContext, (int)((MaxX - MinX) * unScale.X), (int)((MaxY - MinY) * unScale.Y), Color.Transparent));
            _finalStroke.Initialize();
            _finalStroke.IsLocalContent = true;
            _finalStroke.LoadContent(Scene.LocalContentManager,true);
            _finalStroke.Scale(unScale);
            _finalStroke.Translate(pos);
            var toRemove = Children.ToArray();
            for (int i = 0 ;i<toRemove.Length;i++)
            { 
                var child = toRemove[i];
                RemoveChild(child);
                child = null;
            }
            toRemove = null;
            base.AddChild(_finalStroke);
            Update(renderContext);
        }

        //public override void Draw(RenderContext renderContext)
        //{

        //    if (_finalStroke != null)
        //    {
        //        renderContext.SpriteBatch.Draw(_finalStroke.Texture, _finalStroke.WorldPosition,
        //            _finalStroke.DrawRect, _finalStroke.Color, MathHelper.ToRadians(_finalStroke.WorldRotation),
        //            Vector2.Zero, _finalStroke.WorldScale, _finalStroke.Effect, _finalStroke.Depth);

        //        List<Vector2> pts = new List<Vector2>()
        //    {
        //        _finalStroke.WorldPosition,
        //        _finalStroke.WorldPosition+new Vector2(_finalStroke.Width,0)*renderContext.DeviceScale,
        //        _finalStroke.WorldPosition+new Vector2(_finalStroke.Width,_finalStroke.Height)*renderContext.DeviceScale,
        //        _finalStroke.WorldPosition+new Vector2(0,_finalStroke.Height)*renderContext.DeviceScale,
        //    };
        //        renderContext.SpriteBatch.DrawPolygon(pts, Color.DarkOrange);
        //    }
        //    else
        //    {
        //        base.Draw(renderContext);
        //    }
        //}
        #endregion
    }
}