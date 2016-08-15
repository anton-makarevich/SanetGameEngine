using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.XNAEngine
{
    public class ListSelector : GameObject2D
    {
        #region Constructor
        public ListSelector(string cornerAssetFile, string mainAssetFile, string leftAssetFile, string topAssetFile,string bgAsset, float width, float height, GameObjectOrientation orientation = GameObjectOrientation.Vertical)
        {
            _bgAsset = bgAsset;

            _background = new BGPanel(cornerAssetFile, mainAssetFile, leftAssetFile, topAssetFile, width, height);

            _container = new ScrollingPanel(width, height, orientation);
            _container.AutoLayout = false;
            _background.AddChild(_container);
            _container.SkipInvisible = true;
            _container.OnScrolling += () =>
            {
                foreach (var bg in _items)
                    ((GameButton)bg.Children[0]).Touch.CancelTouch();
            };

            AddChild(_background);
        }
        #endregion

        #region Fields
        List<GameSprite> _items;

        BGPanel _background;

        ScrollingPanel _container;

        string _bgAsset;
        #endregion

        #region Properties
        public override bool IsLocalContent
        {
            get
            {
                return base.IsLocalContent;
            }
            set
            {
                base.IsLocalContent = value;
                _background.IsLocalContent = value;
            }
        }

        public bool IsPressed
        {
            get
            {
                return _container.IsPressed;
            }
        }

        public float Height
        {
            get
            {
                return _container.Height;
            }
        }

        public float Width
        {
            get
            {
                return _container.Width;
            }
        }

        public IGameObject SelectedItem
        {
            get
            {
                if (_items == null)
                    return null;
                var item = _items.FirstOrDefault(f => ((GameButton)f.Children[0]).ForcePressed);
                if (item == null || item.Children.Count<2)
                    return null;
                return item.Children[1];
            }
        }
        #endregion

        #region Methods
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager, isLocal);
            _container.Translate(_background.Padding);
        }

        public void SetItems(List<IGameObject> items, int itemSize)
        {
            _items = new List<GameSprite>();
            _container.Clear();
            if (_container.ScrollingDirection== GameObjectOrientation.Vertical)
                _container.ItemHeightOverride = itemSize;
            else if (_container.ScrollingDirection == GameObjectOrientation.Horizontal)
                _container.ItemWidthOverride = itemSize;

            var y = 0;
            foreach (var itemObject in items)
            {
                var item = new GameSprite("EmptyPixel");
                var itemBG = new GameButton(_bgAsset,false);
                itemBG.PressedColor = Color.DeepSkyBlue;

                _items.Add(item);

                item.AddChild(itemBG);
                item.AddChild(itemObject);
                item.Initialize();
                item.LoadContent(SceneManager.RenderContext.GlobalContentManager, false);

                if (_container.ScrollingDirection == GameObjectOrientation.Vertical)
                {
                    var hScale = itemSize / itemBG.Height;
                    itemBG.Scale(_container.Width, hScale);
                }
                else
                {
                    var wScale = itemSize / itemBG.Width;
                    itemBG.Scale(wScale,_container.Height);
                }
                
                itemBG.OnClick += () =>
                    {
                        if (_container.IsStopped)
                        {
                            foreach (var bg in _items)
                                ((GameButton)bg.Children[0]).ForcePressed = false;
                            itemBG.ForcePressed = true;
                            CanDraw = false;
                        }
                    };

                _container.AddChild(item);

                if (_container.ScrollingDirection == GameObjectOrientation.Vertical)
                {
                    item.Translate(0, y);
                    y += (int)_container.ItemHeightOverride;
                }
                else
                {
                    item.Translate(y,0);
                    y += (int)_container.ItemWidthOverride;
                }
            }
        }

        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);
        }
        
        #endregion
    }
}