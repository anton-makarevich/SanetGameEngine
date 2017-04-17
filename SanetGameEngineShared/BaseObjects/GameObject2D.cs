using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    /// <summary>
    /// The base class for all 2D drawable things
    /// </summary>
    public class GameObject2D:IGameObject
    {
        #region Constructor
        public GameObject2D()
        {
            LocalScale = WorldScale = Vector2.One;
            Children = new List<GameObject2D>();
            CanDraw = true;
            SkipDrawing = false;
            DrawInFrontOf3D = true;
        }
        #endregion

        #region Events
        public event Action OnVisibilityChanged;
        #endregion

        #region Fields
        int _z;
        bool _canDraw;
        GameObjectOrientation _deviceScalingOrientation = GameObjectOrientation.Both;

        Vector2 _pivotPoint;

        Vector2 _worldPivot;

        string _customContent;
        #endregion

        #region Properties
        //Local positon - related to this object itself
        public Vector2 LocalPosition { get; set; }
        //position related to parent object
        public Vector2 WorldPosition { get; set; }

        //the same concept for scaling
        public Vector2 LocalScale { get; set; }
        public Vector2 WorldScale { get; set; }

        //and rotation
        public float LocalRotation { get; set; }
        public float LocalRotationInRad { get; private set; }
        public float WorldRotation { get; set; }
        public float WorldRotationInRad { get; private set; }

        public GameObjectOrientation DeviceScalingOrientation
        {
            get
            {
                return _deviceScalingOrientation;
            }
            set
            {
                _deviceScalingOrientation = value;
                foreach (var child in Children)
                    child.DeviceScalingOrientation = value;
            }
        }

        //'central' point to rotate around/translate
        public Vector2 PivotPoint 
        {
            get
            {
                return _pivotPoint;
            }
            set
            {
                _pivotPoint = value;
                foreach (var child in Children)
                    child.PivotPoint = PivotPoint -child.LocalPosition ;
            }
        }

        public Vector2 WorldPivot 
        {
            get 
            {
                if (_worldPivot != Vector2.Zero)
                    return _worldPivot;
                if (Parent != null && Parent.WorldPivot != Vector2.Zero)
                    return Parent.WorldPivot;
                return Vector2.Zero;
            }
            set
            { _worldPivot = value; }
        }

        //World
        protected Matrix WorldMatrix;

        //Parent object
        public GameObject2D Parent { get; set; }

        //Children collection
        public List<GameObject2D> Children { get; private set; }

        private Rectangle? _relativeBoundingRect;
        public virtual Rectangle? BoundingRect { get; protected set; }

        private bool _drawBoundingRect;
        public bool DrawBoundingRect
        {
            get
            {
                if (Parent != null)
                    return Parent.DrawBoundingRect || _drawBoundingRect;

                return _drawBoundingRect;
            }
            set
            {
                _drawBoundingRect = value;
            }
        }

        public virtual int ID { get; set; }

        public virtual int Z 
        {
            get
            { return _z; }
            set
            {
                _z = value;
                //??
                /*foreach (var obj in Children)
                    obj.Z = _z;*/
            }
        }

        public string Tag { get; set; }

        public string Name { get; set; }

        public GameObject2D this[int childIndex]
        {
            get
            {
                if (childIndex >= 0 && childIndex < Children.Count)
                    return Children[childIndex];

                return null;
            }
        }

        //when to draw this sprite?
        //for example background should be drawn before 3D and buttons after
        public bool DrawInFrontOf3D { get; set; }

        //wether we use local (unloadeable) or global (shared) content manager
        public virtual bool IsLocalContent { get; set; }

        public bool IsCustomContent { get; private set; }
        public string CustomContent
        {
            get
            {
                return _customContent;
            }
            set
            {
                _customContent = value;
                IsCustomContent = !string.IsNullOrEmpty(value);

            }
        }

        /// <summary>
        /// equal to 'isvisible'
        /// </summary>
        public virtual bool CanDraw
        {
            get
            {
				if (!_canDraw)
					return false;
                if (Parent != null && !Parent.CanDraw)
                    return false;
				return true;
            }
            set
            {
                if (_canDraw != value)
                {
                    _canDraw = value;
                    if (OnVisibilityChanged != null)
                        OnVisibilityChanged();
                }
            }
        }

        /// <summary>
        /// update even if invisible
        /// for performance reasons we don't update invisible items by default, but sometimes it's needed
        /// </summary>
        public bool ForceUpdate
        { get; set; }

        /// <summary>
        /// doesn't draw only in single game loop
        /// </summary>
        public virtual bool SkipDrawing { get; set; }

        //represents current object scene
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

        Vector2 _deviceScale=Vector2.One;
        public Vector2 DeviceScale 
        {
            get
            { return _deviceScale; }
            private set
            {
                _deviceScale = value;
            }
        }

        #endregion

        #region Methods
        //children methods
        public virtual void AddChild(IGameObject child)
        {
            if (!(child is GameObject2D))
                return;

            if (!Children.Contains((GameObject2D)child))
            {
                //if already a child in other object, remove from there
                if (((GameObject2D)child).Parent != null)
                    ((GameObject2D)child).Parent.RemoveChild(((GameObject2D)child).Parent);

                if (child.Z == 0)
                    child.Z = Z;
                Children.Add((GameObject2D)child);
                ReorderChildren();
                ((GameObject2D)child).Parent = this;
            }
        }

        public GameObject2D GetChildByName(string name) 
        {
            var rv = Children.Where(f =>  f.Name == name).FirstOrDefault() ;
            if (rv != null)
                return rv;
            foreach (var child in Children)
            {
                rv = child.GetChildByName(name);
                if (rv != null)
                    return rv;
            }
            return null;
        }

        public T GetChildByName<T>(string name)where T:GameObject2D
        {
            var rv = Children.Where(f => f is T && f.Name == name).FirstOrDefault() as T;
            if (rv != null)
                return rv;
            foreach (var child in Children)
            {
                rv = child.GetChildByName<T>(name);
                if (rv != null)
                    return rv;
            }
            return null;
        }

        public T GetChildByTag<T>(string tag) where T : GameObject2D
        {
            var rv = Children.Where(f => f is T && f.Tag == tag).FirstOrDefault() as T;
            if (rv != null)
                return rv;
            foreach (var child in Children)
            {
                rv = child.GetChildByTag<T>(tag);
                if (rv != null)
                    return rv;
            }
            return null;
        }

        public void ReorderChildren()
        {
            Children = Children.OrderBy(f => f.Z).ToList();
        }

        public virtual void RemoveChild(GameObject2D child)
        {
            if (Children.Remove(child))
                child.Parent = null;
        }

        public void Rotate(float rotation)
        {
            LocalRotation = rotation;
            LocalRotationInRad = MathHelper.ToRadians(rotation);
        }

        public virtual void Translate(string pos)
        {
            var coords = pos.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (coords.Length==2)
            {
                int x; int y;
                if (int.TryParse(coords[0],out x) && int.TryParse(coords[1],out y))
                {
                    Translate(x, y);
                }
            }
        }

        public virtual void Translate(float posX, float posY)
        {
            Translate(new Vector2(posX, posY));
        }

        public virtual void Translate(Vector2 position)
        {
            if (DeviceScalingOrientation== GameObjectOrientation.Horizontal)
				position.Y = position.Y * (float)Math.Pow(SceneManager.RenderContext.DeviceScale.Y / SceneManager.RenderContext.DeviceScale.X,2);
            else if(DeviceScalingOrientation== GameObjectOrientation.Vertical)
                position.X = position.X * (float)Math.Pow(SceneManager.RenderContext.DeviceScale.X / SceneManager.RenderContext.DeviceScale.Y, 2);
            LocalPosition = position;
        }

        public void Scale(float scale)
        {
            Scale(new Vector2(scale, scale));
        }

        public void Scale(float scaleX, float scaleY)
        {
            Scale(new Vector2(scaleX, scaleY));
        }

        public void Scale(Vector2 scale)
        {
            LocalScale = scale;
        }

        public void CreateBoundingRect(int width, int height, Vector2 offset)
        {
            _relativeBoundingRect = new Rectangle(0, 0, width + (int)offset.X, height + (int)offset.Y);
            BoundingRect = _relativeBoundingRect;
        }

        public void CreateBoundingRect(int width, int height)
        {
            CreateBoundingRect(width, height, Vector2.Zero);
        }

        public bool HitTest(GameObject2D gameObj)
        {
            if (!gameObj.BoundingRect.HasValue) return false;
            if (BoundingRect.HasValue && BoundingRect.Value.Intersects(gameObj.BoundingRect.Value)) return true;

            return Children.FirstOrDefault(child => child.HitTest(gameObj)) != null;
        }

        public bool HitTest(Vector2 position, bool includechildren)
        {
            if (BoundingRect.HasValue &&
                BoundingRect.Value.Contains(
                (int)position.X, (int)position.Y)
                ) return true;

            if (includechildren)
                return Children.FirstOrDefault(
                    child => child.HitTest(position, includechildren))
                    != null;
            return false;
        }

        public virtual bool HitTest(Vector2 position)
        {
            return HitTest(position, true);
        }

        public virtual void Initialize()
        {
            foreach (var child in Children)
                child.Initialize();
        }

        public virtual void LoadContent(ContentManager contentManager, bool isLocal)
        {
            foreach (var child in Children)
            {
                child.LoadContent(contentManager,isLocal);
            }
        }

        public virtual void Unload()
        {
            foreach (var child in Children)
                child.Unload();
        }
   
        public void ReorderByZ()
        {
            Children = Children.OrderBy(f => f.Z).ToList();
        }

        public virtual void Update(RenderContext renderContext)
        {
            if (CanDraw || ForceUpdate)
            {
                if (float.IsNaN(LocalPosition.X) || float.IsNaN(LocalPosition.Y))
                    return;
                WorldMatrix =
                    /*Matrix.CreateTranslation(new Vector3(-PivotPoint, 0)) */
                    Matrix.CreateScale(new Vector3(LocalScale, 1)) *
                    Matrix.CreateRotationZ(LocalRotationInRad) *
                    Matrix.CreateTranslation(new Vector3(LocalPosition, 0));

                if (Parent != null)
                {
                    //WorldMatrix = Matrix.Multiply(WorldMatrix, Matrix.CreateTranslation(new Vector3(Parent.PivotPoint, 0)));
                    WorldMatrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);
                }

                Vector3 pos, scale;
                Quaternion rot;
                var decomposite = WorldMatrix.Decompose(out scale, out rot, out pos);

                _deviceScale = renderContext.DeviceScale;
                if (DeviceScalingOrientation == GameObjectOrientation.Horizontal)
                    _deviceScale.Y = DeviceScale.X;
                else if (DeviceScalingOrientation == GameObjectOrientation.Vertical)
                    _deviceScale.X = DeviceScale.Y;

                var direction = Vector2.Transform(Vector2.UnitX, rot) * DeviceScale;
                WorldRotationInRad = (float)Math.Atan2(direction.Y, direction.X);
                WorldRotation = float.IsNaN(WorldRotation) ? 0 : MathHelper.ToDegrees(WorldRotationInRad);
                //initial worldposition, not considering rotation
                Vector2 inWP = new Vector2(pos.X, pos.Y) * _deviceScale;
                var inScale = new Vector2(scale.X, scale.Y);

                if (WorldRotation != 0)
                {
                    Vector2 rotScale;
                    if (DeviceScale.X != DeviceScale.Y)
                    {
                        //scaling of rotated asset
                        var cos = (float)Math.Cos(WorldRotationInRad);
                        var sin = (float)Math.Sin(WorldRotationInRad);
                        var dx = (float)Math.Sqrt(Math.Pow((DeviceScale.X * cos), 2) + Math.Pow((DeviceScale.Y * sin), 2));
                        var dy = (float)Math.Sqrt(Math.Pow((DeviceScale.X * sin), 2) + Math.Pow((DeviceScale.Y * cos), 2));
                        rotScale = new Vector2(dx, dy);
                    }
                    else
                        rotScale = _deviceScale;

                    WorldScale = inScale * rotScale;

                    Vector2 inCP;

                    //pivot points to rotate arround
                    inCP = PivotPoint * _deviceScale ; //pivot point not considering rotation 
                    WorldPivot = inWP + inCP;

                    Vector2 parentScale=Vector2.One;

                    //considering Parent's pivot
                    Vector2 dParentPivot = Vector2.Zero;
                    if (Parent != null && Parent.PivotPoint != Vector2.Zero)
                    {
                        if (!(Parent is TextBlock) )
                        dParentPivot = Parent.WorldPivot-WorldPivot;
                        else
                            dParentPivot = Parent.Parent.WorldPivot - WorldPivot;

                        if (Parent.PivotPoint != Vector2.Zero && LocalScale != Vector2.One)
                            parentScale=LocalScale;
                    }

                    
                    Vector2 scCP = PivotPoint * WorldScale/parentScale; //pivot point with rotation
                    //points to rotate
                    var corners = new Vector2[]
                { 
                   inWP,
                   scCP
                };
                    //array of rotated pivots
                    Vector2[] cornersRotated = new Vector2[2];
                    //rotation matrix
                    var rotMatrix = Matrix.CreateRotationZ(WorldRotationInRad);
                    //rotating points
                    Vector2.Transform(corners, ref rotMatrix, cornersRotated);
                    //distance to move our asset so its pivot to be in inCP position after rotation
                    var dCP = (inCP - cornersRotated[1]);
                    WorldPosition = inWP + dCP + dParentPivot;

                    //TODO maybe it gives some performance optimization if to combine two transforms in one
                    //calculate bounding rect for correct positioning
                    if (_relativeBoundingRect.HasValue)
                    {
                        var _boundingRect = _relativeBoundingRect.Value.Update(rotMatrix, WorldScale);
                        BoundingRect = new Rectangle((int)(_boundingRect.Left + WorldPosition.X), (int)(_boundingRect.Y + WorldPosition.Y), (int)(_boundingRect.Width /* LocalScale.X*/), (int)(_boundingRect.Height /* LocalScale.Y*/));
                    }

                }
                else
                {
                    WorldPosition = inWP;
                    WorldScale = inScale * _deviceScale;
                    WorldPivot = WorldPosition + PivotPoint * _deviceScale;
                    //calculate bounding rect for correct positioning
                    if (_relativeBoundingRect.HasValue)
                    {
                        BoundingRect = _relativeBoundingRect.Value.Update(WorldMatrix, _deviceScale);
                    }
                }

            }
            
            foreach (var child in Children)
                child.Update(renderContext);
        }
               
        public virtual void Draw(RenderContext renderContext)
        {
            if (SkipDrawing)
            {
                SkipDrawing = false;
                return;
            }
            if (CanDraw)
            {
                foreach (var child in Children)
                { if (child.CanDraw) child.Draw(renderContext); }

                if (renderContext.IsDebug && BoundingRect.HasValue)
                {
                    BoundingRect.Value.Draw(renderContext, Color.Blue);

                    if (PivotPoint != Vector2.Zero)
                        renderContext.SpriteBatch.DrawLine(WorldPosition, WorldPivot, Color.Red);
                }
            }
        }

        /// <summary>
        /// Renders object and all his children into texture
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public virtual Texture2D ToTexture(RenderContext renderContext, int width, int height, Color background)
        {
            RenderTarget2D result;

            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(renderContext.GraphicsDevice, width, height );

            renderContext.GraphicsDevice.SetRenderTarget(result);
            renderContext.GraphicsDevice.Clear(background);
            
            renderContext.SpriteBatch.Begin();
            Draw(renderContext);
            renderContext.SpriteBatch.End();

            //Release the GPU back to drawing to the screen
            renderContext.GraphicsDevice.SetRenderTarget(null);

            return result as Texture2D;
        }

        
#endregion
    }
}
