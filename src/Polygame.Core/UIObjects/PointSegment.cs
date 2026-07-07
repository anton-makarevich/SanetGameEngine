using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sanet.Polygame.BaseObjects;

namespace Sanet.Polygame.UIObjects;

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
    private readonly GameSprite _leftPart;
    private readonly GameSprite _rightPart;

    private float _thickness = 1;

    private readonly Vector2 _point;

    private Color _color;
                                         
    #endregion

    #region Properties

    public float MinX { get; private set; }
    public float MaxX { get; private set; }
    public float MinY { get; private set; }
    public float MaxY { get; private set; }

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            _leftPart.Color = value;
            _rightPart.Color = value;
        }

    }

    public float Width => _leftPart.Height * 0.5f;

    public float Thickness
    {
        get => _thickness;
        set => _thickness = value;
    }
                 

    public float MainWidth{get;private set;}
    public float MainHeight { get; private set; }

    public override int Z
    {
        get => base.Z;
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
            var wPoint = _point * SceneManager.SceneManager.RenderContext.DeviceScale;
            MinX = wPoint.X - w;
            MaxX = wPoint.X + w;

            MinY = wPoint.Y - w;
            MaxY = wPoint.Y + w;
        }
    }

        

}