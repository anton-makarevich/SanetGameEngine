 
 
 
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
	public class PointSegment : GameObject2D
    {
		#region Constructor
		public PointSegment(string assetFile, Vector2 point)
        {
            
            _leftPart = new GameSprite(assetFile);
            _rightPart = new GameSprite(assetFile);
                        
            _rightPart.Effect = SpriteEffects.FlipHorizontally;

            
            AddChild(_leftPart);
            AddChild(_rightPart);
               
			_point = point;
  		}
		#endregion
        
        #region Fields
        private GameSprite _leftPart;
        private GameSprite _rightPart;
                
        float _thickness = 1;

		private Vector2 _point;

        Color _color;
                                         
        #endregion

         #region Properties

        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                _leftPart.Color = value;
                _rightPart.Color = value;
            }

        }

         public float Width 
         { 
             get 
             {
                 return _leftPart.Height * 0.5f; 
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
                _leftPart.Scale(Thickness);
                _rightPart.Scale(Thickness);

                                                                
				_leftPart.Translate(_point -Vector2.One * Width * Thickness);
				_rightPart.Translate(_point -new Vector2(0, Width * Thickness));
				                
                //calculate borders
                var w = Width;
                var wPoint = _point * SceneManager.RenderContext.DeviceScale;
                MinX = wPoint.X - w;
                MaxX = wPoint.X + w;

                MinY = wPoint.Y - w;
                MaxY = wPoint.Y + w;
            }
        }

        

    }
}
