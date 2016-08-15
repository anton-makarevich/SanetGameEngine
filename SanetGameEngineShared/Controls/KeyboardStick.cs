 
 
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
        public class KeyboardStick:GameObject2D
        {
            #region Fields and Properties

            float _xPos = 0;
            float _yPos = 0;
            float XPos
            {
                get
                {
                    return _xPos;
                }
                set
                {
                    _xPos += value;
                    if (_xPos < -1) _xPos = -1;
                    if (_xPos >1) _xPos = 1;
                }
            }
            float YPos
            {
                get
                {
                    return _yPos;
                }
                set
                {
                    _yPos += value;
                    if (_yPos < -1) _yPos = -1;
                    if (_yPos > 1) _yPos = 1;
                }
            }
            /// <summary>
            /// The Value From Stick
            /// </summary>
            public Vector2 Stick
            {
                get
                {
                    return new Vector2 (_xPos,_yPos);
                }
            }

            
            #endregion

            public KeyboardStick()
            {
                
            }

            public override void Initialize()
            {
                base.Initialize();
                _xPos = 0;
                _yPos = 0;
            }
                    
            public override void Draw(RenderContext renderContext)
            {
                base.Draw(renderContext);
            }

            public override void Update(RenderContext renderContext)
            {
                base.Update(renderContext);

                var keyboard = Keyboard.GetState();

                

                var pressed = keyboard.GetPressedKeys();
                if (pressed.Contains(Keys.Up)
                    || pressed.Contains(Keys.Down)
                    || pressed.Contains(Keys.Right)
                    || pressed.Contains(Keys.Left))
                {
                    float value = 1;// (float)renderContext.GameTime.ElapsedGameTime.Milliseconds / 500.0f;

                    if (keyboard.IsKeyDown(Keys.Up))
                        YPos = value;
                    if (keyboard.IsKeyDown(Keys.Down))
                        YPos = -value;
                    if (keyboard.IsKeyDown(Keys.Right))
                        XPos = value;
                    if (keyboard.IsKeyDown(Keys.Left))
                        XPos = -value;
                }
                else
                {
                    _xPos = 0;
                    _yPos = 0;
                }
            }



        }
    }

}
