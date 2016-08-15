 
using Sanet.XNAEngine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    /// <summary>
    /// drawable line based on Line class 
    /// </summary>
    public class LineSegment : GameObject2D
    {
public LineSegment(string sideAssetFile, string mainAssetFile, Line line)
        {
            //MainWidth = width;
            //MainHeight = height;

            _mainPart=new GameSprite(mainAssetFile);
            
            _leftPart = new GameSprite(sideAssetFile);
            _rightPart = new GameSprite(sideAssetFile);
                        
            _rightPart.Effect = SpriteEffects.FlipHorizontally;

            
            AddChild(_leftPart);
            AddChild(_rightPart);
            AddChild(_mainPart);

                        
            _line = line;
            
        }
        

        #region Fields
        private GameSprite _mainPart;
        private GameSprite _leftPart;
        private GameSprite _rightPart;
                
        float _thickness = 1;

        private Line _line;

        Color _color;
        float _distance;
                                 
        #endregion

         #region Properties

        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }

        public Color LineColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                _mainPart.Color = value;
                //_mainPart.Color = new Color(Color.Blue, 0.5f);
                _leftPart.Color = value;
                _rightPart.Color = value;
            }

        }

         public float Width 
         { 
             get 
             {
                 return _leftPart.Height * 0.5f; ; 
             } 
         }

         public float Thickness
         {
             get
             {
                 return _thickness;
             }
             set
             {
                 _thickness = value;
             }
         }

        public float MainWidth{get;private set;}
        public float MainHeight { get; private set; }

        public override int Z
        {
            get
            {
                return base.Z;
            }
            set
            {
                base.Z = value;
                foreach (GameSprite sprite in Children)
                    sprite.Z = value;
            }
        }
#endregion

        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);
            if (isLocal == IsLocalContent)
            {
                //length of the segment
                _distance = Vector2.Distance(_line.WorldPoint1, _line.WorldPoint2);
                _mainPart.Scale(_distance / SceneManager.RenderContext.DeviceScale.Y, Thickness/*1*/);
                _leftPart.Scale(Thickness);
                _rightPart.Scale(Thickness);

                _leftPart.PivotPoint = Vector2.One * Width * Thickness;
                _mainPart.PivotPoint = _rightPart.PivotPoint = new Vector2(0, Width * Thickness);


                _mainPart.Translate(_line.Point1 - _mainPart.PivotPoint);
                _leftPart.Translate(_line.Point1 - _leftPart.PivotPoint);
                _rightPart.Translate(_line.Point2 - _rightPart.PivotPoint);

                var angleDeg = MathHelper.ToDegrees(_line.Angle);

                _leftPart.Rotate(angleDeg);
                _rightPart.Rotate(angleDeg);
                _mainPart.Rotate(angleDeg);

                //calculate borders
                var w = Width;
                if (_line.Point1.X < _line.Point2.X)
                {
                    MinX = _line.WorldPoint1.X - w;
                    MaxX = _line.WorldPoint2.X + w;
                }
                else
                {
                    MinX = _line.WorldPoint2.X - w;
                    MaxX = _line.WorldPoint1.X + w;
                }
                if (_line.Point1.Y < _line.Point2.Y)
                {
                    MinY = _line.WorldPoint1.Y - w;
                    MaxY = _line.WorldPoint2.Y + w;
                }
                else
                {
                    MinY = _line.WorldPoint2.Y - w;
                    MaxY = _line.WorldPoint1.Y + w;
                }
            }
        }

        

    }
}
