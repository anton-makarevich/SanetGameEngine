using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Interfaces;

namespace Sanet.Polygame.UIObjects;

/// <summary>
/// Background container
/// a panel wich contains of 3 parts - one side with rounded corners, scaled central part (1pixel width sprite) and flipped first part
/// currently supports only vertical layout, but might be expanded
/// </summary>
public class BGContainer : GameObject2D
{
    #region Constructor
    public BGContainer(string mainAssetFile, string sideAssetFile, float length, GameObjectOrientation orientation)
    {
        Orientation = orientation;

        _length = length;

        _mainPart=new GameSprite(mainAssetFile);
        _firstPart = new GameSprite(sideAssetFile);
        _lastPart = new GameSprite(sideAssetFile);


        if (Orientation == GameObjectOrientation.Vertical)
        {
                
            _lastPart.Effect = SpriteEffects.FlipVertically;
        }
        else 
        {
            _lastPart.Effect = SpriteEffects.FlipHorizontally;
        }
        AddChild(_firstPart);
        AddChild(_mainPart);
        AddChild(_lastPart);
    }
    #endregion

    #region Fields
    private readonly GameSprite _mainPart;
    private readonly GameSprite _firstPart;
    private readonly GameSprite _lastPart;

    private float _length;
    #endregion

    #region Properties
    public GameObjectOrientation Orientation { get;private set; }

    public float Width => (Orientation == GameObjectOrientation.Vertical) ? _mainPart.Width : _firstPart.Width * 2 + Length;

    public float Height => (Orientation == GameObjectOrientation.Vertical) ? _firstPart.Height*2+Length:_mainPart.Height;

    public float Length
    {
        get => _length;
        set
        {
            if (_length!=value)
            {
                _length = value;
                UpdateAssets();
            }
        }
    }

    /// <summary>
    /// Length of round parts
    /// </summary>
    public float HeaderLength => ((Orientation == GameObjectOrientation.Vertical) ? _firstPart.Height:_firstPart.Width) * 2;

    public override int Z
    {
        get => base.Z;
        set
        {
            base.Z = value;
            foreach (IGameObject obj in Children)
                obj.Z = value;
        }
    }

    public override bool IsLocalContent
    {
        get => base.IsLocalContent;
        set
        {
            base.IsLocalContent = value;
            _mainPart.IsLocalContent = value;
            _firstPart.IsLocalContent = value;
            _lastPart.IsLocalContent = value;
        }
    }
    #endregion

    #region Methods
    public override void LoadContent(ContentManager contentManager, bool isLocal)
    {
        base.LoadContent(contentManager,isLocal);
        if (isLocal == IsLocalContent)
        {
            UpdateAssets();
        }
    }

    private void UpdateAssets()
    {
        _firstPart.Translate(Vector2.Zero);
        if (Orientation == GameObjectOrientation.Vertical)
        {
            _mainPart.Scale(1, Length);
            _mainPart.Translate(0, _firstPart.Height);
            _lastPart.Translate(0, _firstPart.Height + Length);
        }
        else
        {
            _mainPart.Scale(Length,1);
            _mainPart.Translate(_firstPart.Width, 0);
            _lastPart.Translate(_firstPart.Width + Length, 0);
        }
    }
    #endregion
}