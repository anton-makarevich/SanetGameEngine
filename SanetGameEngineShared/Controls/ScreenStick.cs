 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sanet.XNAEngine.Controls
{
    
    namespace ScreenControlsSample
    {
        public class ScreenStick:GameObject2D
        {
            #region Fields and Properties
            //position of stick thumb
            Vector2 _thumbPosition;
            //initial thumb position at pad center
            private Vector2 _thumbOriginalPosition;
            
            //thumb boundaries
            Vector2 _maxThumb;
            Vector2 _minThumb;
                
            //for detection of thumb position relative to pad
            ContainmentType t = ContainmentType.Disjoint;

            /// <summary>
            /// The Value From Stick
            /// </summary>
            public Vector2 Stick
            {
                get
                {
                    //relative difference between current thumb position and its original position
                    Vector2 scaledVector = (_thumbPosition - _thumbOriginalPosition) / (_pad.Width / 2);
                    scaledVector.Y *= -1;

                    if (scaledVector.Length() > 1f)
                        scaledVector.Normalize();

                    return scaledVector;
                }
            }

            Vector2 _padOffset = new Vector2(1050, 600);

            private GameSprite _pad;
            private GameSprite _thumb;
            
            
            int _touchId = -1;
            bool _mousePressed = false;

            public bool IsActive
            {
                get
                {
                    return (_touchId!=-1 || _mousePressed);
                }
            }

            private BoundingSphere stickCollision;
            

            TouchLocation? stickTouch = null;
            

            float maxDistance;
            
            #endregion

            public ScreenStick(string padTexture, string thumbTexture, Color color)
            {
                
                _pad = new GameSprite(padTexture);
                _thumb =new GameSprite(thumbTexture);
                _pad.Color = color;
                _thumb.Color = color;
            }

            public override void Initialize()
            {
                
                // auto placement according to screen rotation
                AddChild(_pad);
                AddChild(_thumb);
                
                base.Initialize();
            }

            public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager, bool isLocal)
            {
                base.LoadContent(contentManager,isLocal);
                if (isLocal == IsLocalContent)
                {
                    maxDistance = (_pad.Width - _thumb.Width);
                    PlaceControls();
                }
            }

            private void PlaceControls()
            {
                //control center
                var padCenter = new Vector2(
                     _pad.Width / 2,
                     _pad.Height / 2);

                //bounding sphere
                stickCollision = new BoundingSphere(new Vector3(padCenter, 0), _pad.Width / 2);

                _thumbOriginalPosition = new Vector2(
                   (_pad.Width - _thumb.Width) / 2,
                   (_pad.Height - _thumb.Height) / 2);

                _minThumb = new Vector2(
                    padCenter.X - _pad.Width / 2 - _thumb.Width / 2,
                    padCenter.Y - _pad.Height / 2 - _thumb.Height / 2);

                _maxThumb = new Vector2(
                    padCenter.X + _pad.Width / 2 - _thumb.Width / 2,
                    padCenter.Y + _pad.Height / 2 - _thumb.Height / 2);

                _thumbPosition = _thumbOriginalPosition;
                Translate(_padOffset);
                _thumb.Translate(_thumbPosition);
                               
            }

            
            public override void Draw(RenderContext renderContext)
            {
                Vector2 currentLeftPosition = new Vector2(
                    _thumbOriginalPosition.X + Stick.X * maxDistance,
                    _thumbOriginalPosition.Y + Stick.Y * maxDistance * -1);

                
                base.Draw(renderContext);
            }

            public override void Update(RenderContext renderContext)
            {
                base.Update(renderContext);

                stickTouch = null;


                var touches = renderContext.TouchPanelState;
                //check whether we have any touch inputs
                if (touches.Count > 0)
                {
                    for (int i = 0; i < touches.Count; i++)
                    {
                        TouchLocation t = touches[i];

                        float x = t.Position.X;
                        float y = t.Position.Y;

                        if (t.Id == _touchId)
                        {
                            stickTouch = t;
                            continue;
                        }

                        if (_touchId == -1)
                        {
                            if (IsTouchingLeftStick(ref x, ref y))
                            {
                                stickTouch = t;
                                continue;
                            }
                        }


                    }

                    if (stickTouch.HasValue)
                    {
                        _thumbPosition = new Vector2(
                            stickTouch.Value.Position.X - _padOffset.X - _thumb.Width / 2,
                            stickTouch.Value.Position.Y - _padOffset.Y - _thumb.Height / 2);

                        _thumbPosition = Vector2.Clamp(_thumbPosition, _minThumb, _maxThumb);
                        _touchId = stickTouch.Value.Id;
                    }
                    else
                    {
                        _touchId = -1;
                        _thumbPosition += (_thumbOriginalPosition - _thumbPosition) * 0.9f;
                    }
                    
                }
                //get mouse input if no touch input
                else
                {
                    var mouse = Mouse.GetState();
                    //first check if we pressing stick with mouse
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        //if just pressed 
                        if (!_mousePressed)
                        {
                            float x = mouse.X;
                            float y = mouse.Y;
                            if (IsTouchingLeftStick(ref x, ref y))
                            {
                                _mousePressed = true;
                            }
                        }
                    }
                    else
                    {
                        _mousePressed = false;
                    }

                    //now calculate values based on this
                    if (_mousePressed)
                    {
                        _thumbPosition = new Vector2(
                            mouse.X - _padOffset.X - _thumb.Width / 2,
                            mouse.Y - _padOffset.Y - _thumb.Height / 2);

                        _thumbPosition = Vector2.Clamp(_thumbPosition, _minThumb, _maxThumb);
                    }
                    else
                    {
                        _thumbPosition += (_thumbOriginalPosition - _thumbPosition) * 0.9f;
                    }
                }
                _thumb.Translate(_thumbPosition);
            }

            private bool IsTouchingLeftStick(ref float x, ref float y)
            {
                Vector3 point = new Vector3(x-_padOffset.X, y-_padOffset.Y, 0);
                stickCollision.Contains(ref point, out t);
                return (t == ContainmentType.Contains);
            }

           
        }
    }

}
