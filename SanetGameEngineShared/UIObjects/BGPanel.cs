 
 
 
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
    /// Background container
    /// a panel wich contains of 4 parts:
    /// 1 - corner asset flipped 3 times,
    /// 2 - top part one pixel width and flipped for bottom,
    /// 3 - left side one pixel height and flipped for right
    /// 4 - one pixel main part
    /// currently supports only vertical layout, but might be expanded
    /// </summary>
    public class BGPanel : GameObject2D
    {
        #region Constructor
        public BGPanel(string cornerAssetFile, string mainAssetFile, string leftAssetFile, string topAssetFile, float width, float height)
        {
            MainWidth = width;
            MainHeight = height;

            _mainPart=new GameSprite(mainAssetFile);
            
            _leftPart = new GameSprite(leftAssetFile);
            _rightPart = new GameSprite(leftAssetFile);
            _topPart = new GameSprite(topAssetFile);
            _bottomPart = new GameSprite(topAssetFile);

            _corneTL = new GameSprite(cornerAssetFile);
            _corneTR = new GameSprite(cornerAssetFile);
            _corneBL = new GameSprite(cornerAssetFile);
            _corneBR = new GameSprite(cornerAssetFile);
                        
            _bottomPart.Effect = SpriteEffects.FlipVertically;
            _rightPart.Effect = SpriteEffects.FlipHorizontally;

            _corneTR.Effect = SpriteEffects.FlipHorizontally;
            _corneBL.Effect = SpriteEffects.FlipVertically;
            _corneBR.Effect = SpriteEffects.FlipHorizontally|SpriteEffects.FlipVertically;
            
            AddChild(_mainPart);
            AddChild(_leftPart);
            AddChild(_rightPart);
            AddChild(_topPart);
            AddChild(_bottomPart);

            AddChild(_corneTL);
            AddChild(_corneTR);
            AddChild(_corneBL);
            AddChild(_corneBR);
        }
        #endregion

        #region Fields
        private GameSprite _mainPart;

        private GameSprite _leftPart;
         private GameSprite _topPart;
         private GameSprite _bottomPart;
         private GameSprite _rightPart;

         private GameSprite _corneTL;
         private GameSprite _corneTR;
         private GameSprite _corneBL;
         private GameSprite _corneBR;
        #endregion

        #region Properties
         
         public float Width 
         { 
             get 
             {
                 return  _leftPart.Width * 2 + MainWidth; 
             } 
         }
        public float Height
        { 
            get
            { 
                return  _topPart.Height*2+MainWidth; 
            } 
        }

        public float MainWidth { get; private set; }
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
                /*foreach (IGameObject sprite in Children)
                    sprite.Z = value;*/
            }
        }

        public Color Color
        {
            get 
            {
                return _mainPart.Color;
            }
            set
            {
                _mainPart.Color = 
                    _bottomPart.Color = _topPart.Color =
                    _leftPart.Color = _rightPart.Color =
                    _corneBL.Color = _corneBR.Color = _corneTL.Color = _corneTR.Color =
                    value;
            }
        }

        public override bool IsLocalContent
        {
            get
            {
                return base.IsLocalContent;
            }
            set
            {
                base.IsLocalContent = value;
                _mainPart.IsLocalContent = value;
                _leftPart.IsLocalContent = value;
                _topPart.IsLocalContent = value;
                _bottomPart.IsLocalContent = value;
                _rightPart.IsLocalContent = value;
                _corneTL.IsLocalContent = value;
                _corneTR.IsLocalContent = value;
                _corneBL.IsLocalContent = value;
                _corneBR.IsLocalContent = value;
            }
        }

        public Vector2 Padding
        {
            get
            {
                return new Vector2(_leftPart.Width, _topPart.Height);
            }
        }
#endregion

        #region Methods
        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);

            if (isLocal==IsLocalContent)
                RecalculateSize();

        }

        void RecalculateSize()
        {
            _mainPart.Scale(MainWidth, MainHeight);

            _leftPart.Scale(1, MainHeight);
            _rightPart.Scale(1, MainHeight);
            _topPart.Scale(MainWidth, 1);
            _bottomPart.Scale(MainWidth, 1);

            _corneTL.Translate(Vector2.Zero);
            _corneTR.Translate(_leftPart.Width + MainWidth, 0);
            _corneBL.Translate(0, _topPart.Height + MainHeight);
            _corneBR.Translate(_leftPart.Width + MainWidth, _topPart.Height + MainHeight);

            _mainPart.Translate(_leftPart.Width, _topPart.Height);

            _leftPart.Translate(0, _topPart.Height);
            _topPart.Translate(_leftPart.Width, 0);
            _rightPart.Translate(_leftPart.Width + MainWidth, _topPart.Height);
            _bottomPart.Translate(_leftPart.Width, _topPart.Height + MainHeight);

            
        }

        /// <summary>
        /// Resizing panel
        /// </summary>
        /// <param name="newSize">New size (main rect part, without counting rounded corners, actual size will be ~+50px in each directing</param>
        public void Resize(Vector2 newSize)
        {
            MainWidth = newSize.X;
            MainHeight = newSize.Y;

            RecalculateSize();
        }

        #endregion

    }
}
