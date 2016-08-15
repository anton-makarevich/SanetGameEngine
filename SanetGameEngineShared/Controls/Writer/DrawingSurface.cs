 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.XNAEngine.Controls
{
    /// <summary>
    /// Control to be used as drawing base for thank you writer
    /// </summary>
    public class DrawingSurface : GameObject2D
    {
        #region Fields
        GameSpriteTouchable _background;
        int _width;
        int _height;

        Vector2 _lastAddedPoint=Vector2.Zero;

        Color _currentColor=Color.OrangeRed;
        float _currentThickness = 1;

        bool _isEditable;

        List<TextField> _textFields = new List<TextField>();
        List<Photo> _photos = new List<Photo>();

        List<Stroke> _undoObjects = new List<Stroke>();
        Stroke _currentObjects = new Stroke();
                
        #endregion

        #region Properties
        public Color CurrentColor
        {
            get
            {
                return _currentColor;
            }
            set
            {
                _currentColor = value;
            }
        }
        public float CurrentThickness
        {
            get
            {
                return _currentThickness;
            }
            set
            {
                _currentThickness = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }

        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public bool CanUndo
        {
            get
            {
                return _undoObjects.Any();
            }
        }

        public DrawingModes Mode { get; set; }

        public bool IsEditable 
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                if (_isEditable)
                {
                    foreach (var textField in _textFields)
                        if (textField.IsActive || textField.IsMoving)
                            textField.Deactivate();
                    foreach (var photo in _photos)
                        if (photo.IsActive )
                            photo.Deactivate();
                }
            }
        }

        public bool HasDeletables
        {
            get
            {
                if (_photos.Any(f => f.IsActive))
                    return true;
                if (_textFields.Any(f => f.IsMoving))
                    return true;
                return false;
            }
        }
        #endregion

        #region Constructor
        public DrawingSurface(int width, int height)
            : base()
        {
            _height =height;
            _width = width;
        }
        #endregion

        #region public Methods
        public override void Initialize()
        {
            
            base.Initialize();
            
        }

        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            if (isLocal==IsLocalContent)
                PrepareBackground();

            base.LoadContent(contentManager,isLocal);
            
        }

        public override void Update(RenderContext renderContext)
        {
            if (IsEditable)
            {
                if (_background.Touch.PressPointPrevious != Vector2.Zero && _background.Touch.IsPressed)
                {
                    if (_background.Touch.PressPointPrevious != _background.Touch.PressPoint)
                        AddSegment((_background.Touch.PressPointPrevious - WorldPosition) / renderContext.DeviceScale, (_background.Touch.PressPoint - WorldPosition) / renderContext.DeviceScale);
                    else
                        AddPoint((_background.Touch.PressPointPrevious - WorldPosition) / renderContext.DeviceScale);
                }
            }
            //moving text fields
            foreach (var textField in _textFields)
            {
                if (textField.IsMoving && _background.Touch.IsPressed)
                {
                    var pos = textField.LocalPosition;
                    pos += _background.Touch.Distance/renderContext.DeviceScale;
                    textField.Translate(pos);
                }
            }

            
            base.Update(renderContext);
            //moving photos
            foreach (var photo in _photos)
            {
                if (photo.IsActive && _background.Touch.IsPressed)
                {
                    if (_background.Touch.TouchPoints==1)
                        photo.Move(_background.Touch.Distance / renderContext.DeviceScale);
                    else if (_background.Touch.TouchPoints == 2)
                    {
                        var p1 = _background.Touch.PressPoint;
                        var p2 = _background.Touch.PressPoint2;
                        var pp1 = _background.Touch.PressPointPrevious;
                        var pp2 = _background.Touch.PressPointPrevious2;

                        if (p1 == Vector2.Zero || p2 == Vector2.Zero || pp1 == Vector2.Zero || pp2 == Vector2.Zero)
                            return;

                        if (p1 == pp1 || p2 == pp2)
                            return; 

                        photo.RotateManually(
                            ( p1/ renderContext.DeviceScale)-LocalPosition-photo.LocalPosition,
                            (p2 / renderContext.DeviceScale) - LocalPosition - photo.LocalPosition,
                            (pp1 / renderContext.DeviceScale) - LocalPosition - photo.LocalPosition,
                            (pp2 / renderContext.DeviceScale) - LocalPosition - photo.LocalPosition);
                    }
                }
            }
            
        }

        public void Clear()
        {
            _background.Children.Clear();
            _undoObjects.Clear();
            _currentObjects.Children.Clear();
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var lastObjects = _undoObjects.LastOrDefault();
                if (lastObjects != null && lastObjects.Children.Any())
                {
                    _background.RemoveChild(lastObjects);
                    _undoObjects.Remove(lastObjects);
                    lastObjects.Children.Clear();
                    lastObjects = null;
                    _currentObjects = new Stroke();
                }
            }
        }

        public Texture2D ToTexture()
        {
            var renderContext = SceneManager.RenderContext;
            RenderTarget2D result;
                        
            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(renderContext.GraphicsDevice, (int)(Width * renderContext.DeviceScale.X), (int)(Height *renderContext.DeviceScale.Y) );

            renderContext.GraphicsDevice.SetRenderTarget(result);
            renderContext.GraphicsDevice.Clear(Color.White);

            _background.Translate(LocalPosition * -1);
            Update(renderContext);

            renderContext.SpriteBatch.Begin();
            _background.Draw(renderContext);
            renderContext.SpriteBatch.End();

            _background.Translate(Vector2.Zero);
            Update(renderContext);
            //Release the GPU back to drawing to the screen
            renderContext.GraphicsDevice.SetRenderTarget(null);

            return result as Texture2D;
        }

        public void AddSegment(Vector2 start, Vector2 end)
        {
            Line line = new Line(start-LocalPosition, end-LocalPosition);
            var segment = new LineSegment("Brushes/Simple/0", "Brushes/Simple/2", line);
            segment.LineColor = (Mode==DrawingModes.Marker)?CurrentColor:Color.White;
            segment.Thickness = CurrentThickness;
            segment.Initialize();
            segment.LoadContent(SceneManager.RenderContext.GlobalContentManager,IsLocalContent);
            //_background.AddChild(segment);
            _currentObjects.AddChild(segment);
            if (_currentObjects.MinX > segment.MinX)
                _currentObjects.MinX = segment.MinX;
            if (_currentObjects.MinY > segment.MinY)
                _currentObjects.MinY = segment.MinY;
            if (_currentObjects.MaxX < segment.MaxX)
                _currentObjects.MaxX = segment.MaxX;
            if (_currentObjects.MaxY < segment.MaxY)
                _currentObjects.MaxY = segment.MaxY;
        }

        public void AddPoint(Vector2 point)
        {
            if (_lastAddedPoint == point)
                return;
            var segment = new PointSegment("Brushes/Simple/0",point);
            segment.Color = (Mode == DrawingModes.Marker) ? CurrentColor : Color.White;
            segment.Thickness = CurrentThickness;
            segment.Initialize();
            segment.LoadContent(SceneManager.RenderContext.GlobalContentManager, IsLocalContent);
            //_background.AddChild(segment);
            _currentObjects.AddChild(segment);
            if (_currentObjects.MinX > segment.MinX)
                _currentObjects.MinX = segment.MinX;
            if (_currentObjects.MinY > segment.MinY)
                _currentObjects.MinY = segment.MinY;
            if (_currentObjects.MaxX < segment.MaxX)
                _currentObjects.MaxX = segment.MaxX;
            if (_currentObjects.MaxY < segment.MaxY)
                _currentObjects.MaxY = segment.MaxY;
            _lastAddedPoint = point;
        }

        public void AddTextField(TextField textField)
        {
            
            textField.FontColor = CurrentColor;
            _background.AddChild(textField);
            _textFields.Add(textField);
            textField.Activated += () =>
                {
                    IsEditable = false;
                    textField.Z = _background.Children.Count;
                    _background.ReorderChildren();
                    foreach (var otherTextField in _textFields)
                    {
                        if (otherTextField.TextFieldGuid != textField.TextFieldGuid)
                            otherTextField.Deactivate();
                    }
                };
        }

        public void AddPhoto(Photo photo)
        {
                       
            _background.AddChild(photo);
            _photos.Add(photo);
            photo.Activated += () =>
            {
                IsEditable = false;
                photo.Z = _background.Children.Count;
                _background.ReorderChildren();
                foreach (var otherPhoto in _photos)
                {
                    if (otherPhoto!= photo)
                        otherPhoto.Deactivate();
                }
            };
        }

        public void SetInitial(GameSprite initial)
        {
            initial.Scale( Width/initial.Width,Height/initial.Height);
            _background.AddChild(initial);
        }

        public void DeleteActive()
        {
            var activeText = _textFields.FirstOrDefault(f => f.IsMoving);
            if (activeText != null)
            {
                _background.RemoveChild(activeText);
                _textFields.Remove(activeText);
                activeText = null;
                return;
            }
            var activePhoto = _photos.FirstOrDefault(f => f.IsActive);
            if (activePhoto != null)
            {
                _background.RemoveChild(activePhoto);
                _photos.Remove(activePhoto);
                activePhoto.Texture.Dispose();
                activePhoto = null;
                return;
            }
        }

        #endregion

        #region private Methods
        void PrepareBackground()
        {
            var graphicsDevice = SceneManager.RenderContext.GraphicsDevice;
            RenderTarget2D result;
            
            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(graphicsDevice, _width, _height);

            graphicsDevice.SetRenderTarget(result);
            graphicsDevice.Clear(Color.White); //TODO: should we support different color
                        
            //Release the GPU back to drawing to the screen
            graphicsDevice.SetRenderTarget(null);

            _background=new GameSpriteTouchable(result as Texture2D);
            _background.Initialize();
            AddChild(_background);
            _background.Touch.OnEnter += () =>
                {
                    if (_currentObjects.Children.Any())
                       _currentObjects = new Stroke();
                    _currentObjects.MinX = _currentObjects.MinY = 5000;
                    _background.AddChild(_currentObjects);
                    
                };
            _background.Touch.OnClick+=ToUndo;
            _background.Touch.OnLeave += ToUndo;
        }

        void ToUndo()
        {
            if (_currentObjects.Children.Any())
            {
                _currentObjects.Merge();
                _undoObjects.Add(_currentObjects);
            }
        }
        
        #endregion
    }
}