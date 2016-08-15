 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    //the base object for any 3d thing in game
    public abstract class GameObject3D
    {
        //very similar to GameObject2D
        //see comments there
        protected GameObject3D()
        {
            Children = new List<GameObject3D>();
            LocalScale = WorldScale = Vector3.One;
            LocalRotation = Quaternion.Identity;
            CanDraw = true;
        }

        #region Properties
        public Vector3 LocalPosition { get; set; }
        public Vector3 WorldPosition { get; private set; }

        public Quaternion LocalRotation { get; set; }
        public Quaternion WorldRotation { get; private set; }

        public Vector3 LocalScale { get; set; }
        public Vector3 WorldScale { get; private set; }

        public GameObject3D Parent { get; private set; }
        public List<GameObject3D> Children { get; private set; }

        public int ID { get; set; }

        protected Matrix WorldMatrix;

        private BoundingBox? _relativeBoundingBox;
        public virtual Rectangle? BoundingRect { get; protected set; }
        private bool _drawBoundingBox;
        public bool DrawBoundingBox
        {
            get
            {
                if (Parent != null)
                    return Parent.DrawBoundingBox || _drawBoundingBox;

                return _drawBoundingBox;
            }
            set
            {
                _drawBoundingBox = value;
            }
        }

        public GameObject3D this[int childIndex]
        {
            get
            {
                if (childIndex >= 0 && childIndex < Children.Count)
                    return Children[childIndex];

                return null;

            }
        }

        public virtual bool CanDraw { get; set; }

        private GameScene _scene;
        public GameScene Scene
        {
            get
            {
                if (_scene != null) return _scene;
                if (Parent != null) return Parent.Scene;
                return null;
            }

            set { _scene = value; }
        }
        #endregion

        #region Methods
        public void AddChild(GameObject3D child)
        {
            if (!Children.Contains(child))
            {
                child.Parent = this;
                Children.Add(child);
            }
        }

        public void RemoveChild(GameObject3D child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
            }
        }

        public void Translate(Vector3 translation)
        {
            LocalPosition = translation;
        }

        public void Translate(float x, float y, float z)
        {
            LocalPosition = new Vector3(x, y, z);
        }

        public void Scale(Vector3 scale)
        {
            LocalScale = scale;
        }

        public void Scale(float x, float y, float z)
        {
            LocalScale = new Vector3(x, y, z);
        }

        public void Rotate(Quaternion rotation)
        {
            LocalRotation *= rotation;
        }

        public void Rotate(float pitch, float yaw, float roll)
        {
            LocalRotation *= Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), MathHelper.ToRadians(roll));
        }

        public void CreateBoundingBox(float width, float height, float depth, Vector3 offset)
        {
            var max = new Vector3(width / 2.0f, height / 2.0f, depth / 2.0f);
            var min = -max;

            _relativeBoundingBox = new BoundingBox(min + offset, max + offset);
            
        }

        public void CreateBoundingBox(float width, float height, float depth)
        {
            CreateBoundingBox(width, height, depth, Vector3.Zero);
        }

        public bool HitTest(GameObject3D gameObj)
        {
            //Check Other_Object Itself
            if (gameObj.BoundingRect.HasValue && BoundingRect.HasValue)
            {
                if (BoundingRect.Value.Intersects(gameObj.BoundingRect.Value)) return true;
            }

            //Check this_Object and other_Object's Children
            if (gameObj.Children.FirstOrDefault(HitTest) != null) return true;

            //Check this_Object's children with other_Object
            return Children.FirstOrDefault(child => child.HitTest(gameObj)) != null;
        }

        public virtual bool HitTest(Rectangle objRect)
        {
            //Check Other_Object Itself
            if (BoundingRect.HasValue)
            {
                var inter = BoundingRect.Value.Intersects(objRect) || BoundingRect.Value.Contains(objRect);
                return inter;
            }
            var c=Children.FirstOrDefault(child => child.HitTest(objRect));
            return  c!= null;
        }

        public virtual void Initialize()
        {
            foreach (var child in Children)
                child.Initialize();
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            foreach (var child in Children)
                child.LoadContent(contentManager);
        }

        public virtual void Update(RenderContext renderContext)
        {
            WorldMatrix = Matrix.CreateFromQuaternion(LocalRotation) *
                          Matrix.CreateScale(LocalScale) *
                          Matrix.CreateTranslation(LocalPosition);


            if (Parent != null)
            {
                WorldMatrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);

                Vector3 scale, position;
                Quaternion rotation;

                var decomposite = WorldMatrix.Decompose(out scale, out rotation, out position);
                
                WorldPosition = position;
                WorldScale = scale;
                WorldRotation = rotation;
            }
            else
            {
                WorldPosition = LocalPosition;
                WorldScale = LocalScale;
                WorldRotation = LocalRotation;
            }

            foreach (var child in Children)
                child.Update(renderContext);

            if (_relativeBoundingBox.HasValue)
            {
                var newMax = renderContext.GraphicsDevice.Viewport.Project(_relativeBoundingBox.Value.Max,
                    renderContext.Camera.Projection, renderContext.Camera.View,WorldMatrix);
                var newMin = renderContext.GraphicsDevice.Viewport.Project(_relativeBoundingBox.Value.Min,
                    renderContext.Camera.Projection, renderContext.Camera.View, WorldMatrix);
                BoundingRect = new Rectangle((int)newMin.X, (int)newMax.Y, (int)(newMax.X - newMin.X), (int)(newMin.Y - newMax.Y));
                    //_relativeBoundingBox.Value.ToRect(WorldMatrix);
            }
        }

        public virtual void Draw(RenderContext renderContext)
        {
            if (CanDraw)
            {
                foreach (var child in Children)
                { if (child.CanDraw) child.Draw(renderContext); }

                if (renderContext.IsDebug && BoundingRect.HasValue)
                {
                    renderContext.SpriteBatch.Begin();
                    BoundingRect.Value.Draw(renderContext, Color.Blue);
                    renderContext.SpriteBatch.End();
                }
            }

        }
        #endregion
    }
}
