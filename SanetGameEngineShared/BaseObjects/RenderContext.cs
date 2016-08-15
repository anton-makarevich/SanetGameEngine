 
 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace Sanet.XNAEngine
{
    public class RenderContext
    {
        #region Fields
        Vector2 _viewSize ;
        Vector2 _deviceScale;

        Dictionary<string,ContentManager> _contentManagers;
        #endregion

        #region Properties
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public GameTime GameTime { get; set; }
        public TouchCollection TouchPanelState { get; set; }
        public BaseCamera Camera { get; set; }
        public ContentManager GlobalContentManager { get; set; }
        //set this to true to draw debug sprites
        public bool IsDebug { get; set; }

        public Vector2 ViewSize
        {
            get
            {
                return _viewSize;
            }
            set
            {
                _viewSize = value;
                _deviceScale= new Vector2((float)GraphicsDevice.Viewport.Width / ViewSize.X, (float)GraphicsDevice.Viewport.Height / ViewSize.Y);
            }
        }

        public Vector2 DeviceScale
        {
            get
            {
                //for landscape

                return _deviceScale;

            }
        }
        #endregion

        #region Methods
        public void AddContentManager(ContentManager contentManager)
        {
            if (_contentManagers == null)
                _contentManagers = new Dictionary<string,ContentManager>();
            if (!_contentManagers.ContainsKey(contentManager.RootDirectory))
                _contentManagers.Add(contentManager.RootDirectory, contentManager);
        }

        public ContentManager GetContentManager(string rootDirectory)
        {
            if (_contentManagers == null)
                _contentManagers = new Dictionary<string, ContentManager>();
            if (!_contentManagers.ContainsKey(rootDirectory))
            {
                var contentManager = new ContentManager(GlobalContentManager.ServiceProvider, rootDirectory);
                AddContentManager(contentManager);
                return contentManager;
            }
            return _contentManagers[rootDirectory];
        }
        #endregion
    }
}
