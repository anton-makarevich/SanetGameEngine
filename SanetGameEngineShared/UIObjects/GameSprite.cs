 
 
 
 
 
using Sanet.XNAEngine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

namespace Sanet.XNAEngine
{
    //represents simple 2d sprite
    public class GameSprite : GameObject2D
    {
        #region Fields
        private string _assetFile;
        private Texture2D _texture;
        private List<Texture2D> _textures = new List<Texture2D>();
        private int _currentTextureIndex = 0;
        Color _color;
        FrameAnimation _FrameAnimation;

        bool _applyColorToChildren = false;
        
        #endregion

        #region Properties
        public float Width { get { return _texture.Width; } }
        public float Height { get { return _texture.Height; } }

        public bool ApplyColorToChildren
        { 
            get
            {
                if (!_applyColorToChildren && Parent != null && Parent is GameSprite)
                    return ((GameSprite)Parent).ApplyColorToChildren;
                return _applyColorToChildren;
            }
            set
            {
                _applyColorToChildren=value;
            }
        }

        public string AssetFile
        {
            get
            {
                return _assetFile;
            }
        }

        public float Depth { get; set; }
        public virtual Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if (ApplyColorToChildren)
                {
                    var childrenSprites = Children.Where(f => f is GameSprite).Cast<GameSprite>().ToList();
                    foreach (var sprite in childrenSprites)
                        sprite.Color = new Color(sprite.Color.R, sprite.Color.G, sprite.Color.B, value.A);
                    var childrenTexts = Children.Where(f => f is TextBlock).Cast<TextBlock>().ToList();
                    foreach (var text in childrenTexts)
                        text.TextColor = new Color(text.TextColor.R, text.TextColor.G, text.TextColor.B, value.A);
                }
            }
        }

        public SpriteEffects Effect { get; set; }
        public Rectangle? DrawRect { get; set; }

        public FrameAnimation FrameAnimation
        {
            get
            {
                return _FrameAnimation;
            }
            set
            {
                try
                {
                    if (_FrameAnimation != null)
                        _FrameAnimation.Completed -= _FrameAnimation_Completed;
                }
                catch { }
                _FrameAnimation = value;
                _FrameAnimation.Completed += _FrameAnimation_Completed;
                CreateBoundingRect((int)_FrameAnimation.FrameSize.X, (int)_FrameAnimation.FrameSize.Y);
            }
        }

        void _FrameAnimation_Completed()
        {
            _currentTextureIndex = 0;
        }
        public PathAnimation PathAnimation { get; set; }
        public SoundAnimation SoundAnimation { get; set; }
        public OpacityAnimation OpacityAnimation { get; set; }
        public AnimationBase InAnimation { get; set; }
        public AnimationBase OutAnimation { get; set; }
        public AnimationBase InTranslation { get; set; }
        public AnimationBase OutTranslation { get; set; }

        public Texture2D Texture
        {
            get
            {
                if (_textures.Count>0)
                return _textures[0];
                return null;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(Width, Height) * 0.5f;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }
        #endregion

        #region Constructor
        public GameSprite(string assetFile)
        {
            _assetFile = assetFile;
            Color = Color.White;
            Effect = SpriteEffects.None;
        }

        public GameSprite(Texture2D texture)
        {
            _texture = texture;
            _textures.Add(_texture);
            Color = Color.White;
            Effect = SpriteEffects.None;
        }
#endregion

        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);
            //Warning! this is experimental function, never recommended to use
            if (IsCustomContent)
            {
                contentManager = SceneManager.RenderContext.GetContentManager(SceneManager.RenderContext.GlobalContentManager.RootDirectory + "\\Scenes\\" + CustomContent);
                isLocal = IsLocalContent = false;
            }
            if (isLocal == IsLocalContent )
            {
                if (!string.IsNullOrEmpty(_assetFile))
                {
                    try
                    {
                        _texture = contentManager.Load<Texture2D>(_assetFile);
                    }
                    catch 
                    {
                        Debug.WriteLine("Can't find " + _assetFile);
                    }
                }
                if (_texture == null)
                    return;
                var t = _textures.FirstOrDefault(f => f.Name == _texture.Name);
                if (t != null)
                    _textures.Remove(t);
                    //if (!_textures.Contains(_texture))
                _textures.Insert(0,_texture);
                

                if (FrameAnimation != null)
                {
                    FrameAnimation.SetSpriteSize(new Vector2(Width, Height));
                    CreateBoundingRect((int)FrameAnimation.FrameSize.X, (int)FrameAnimation.FrameSize.Y);
                    DrawRect = FrameAnimation.FrameRect;
                    //preload extra textures
                    if (FrameAnimation is FrameAnimationMulti)
                    {
                        foreach (var textureName in ((FrameAnimationMulti)FrameAnimation).ExtraTextures)
                        {
                            _textures.Add(contentManager.Load<Texture2D>(textureName));
                        }
                    }
                }
                else
                    CreateBoundingRect((int)Width, (int)Height);
            }
        }

        public override void Update(RenderContext renderContext)
        {
            if (!_textures.Any())
                return;
            //Updating animations
            if (InAnimation != null && InAnimation.IsPlaying)
            {
                InAnimation.Update(renderContext);
                if (InAnimation is OpacityAnimation)
                {
                    Color = Color.ApplyPremultipliedAlpha(((OpacityAnimation)InAnimation).Opacity);
                }
            }
            else if (InTranslation != null && InTranslation.IsPlaying)
            {
                InTranslation.Update(renderContext);
                if (InTranslation.IsPlaying)
                {
                    if (InTranslation is PathAnimation)
                    {
                        Scale(((PathAnimation)InTranslation).Scale);
                        Rotate(((PathAnimation)InTranslation).Rotation);
                        Translate(((PathAnimation)InTranslation).Position);
                        Effect = ((PathAnimation)InTranslation).Effect;
                    }
                    else if (InTranslation is OpacityAnimation)
                    {
                        Color = Color.ApplyPremultipliedAlpha(((OpacityAnimation)InTranslation).Opacity);
                        
                    }
                }
            }
            else if (OutTranslation != null && OutTranslation.IsPlaying)
            {
                OutTranslation.Update(renderContext);
                if (OutTranslation.IsPlaying)
                {
                    if (OutTranslation is PathAnimation)
                    {
                        Scale(((PathAnimation)OutTranslation).Scale);
                        Rotate(((PathAnimation)OutTranslation).Rotation);
                        Translate(((PathAnimation)OutTranslation).Position);
                        Effect = ((PathAnimation)OutTranslation).Effect;
                    }
                    else if (OutTranslation is OpacityAnimation)
                    {
                        Color = Color.ApplyPremultipliedAlpha(((OpacityAnimation)OutTranslation).Opacity);
                    }
                }
            }
            else
            {
                if (FrameAnimation != null && FrameAnimation.IsPlaying)
                {
                    FrameAnimation.Update(renderContext);
                    DrawRect = FrameAnimation.FrameRect;
                    if (FrameAnimation is FrameAnimationMulti)
                    {
                        _currentTextureIndex = ((FrameAnimationMulti)FrameAnimation).CurrentTextureNumber;
                        //Debug.WriteLine(string.Format("Frame-{0}, FrameNumber-{1}, TextureNumber-{2}, Frame-{3}",FrameAnimation.CurrentFrame,FrameAnimation.CurrentFrameNumber,_currentTextureIndex,DrawRect.ToString()));
                    }
                }
                if (PathAnimation != null && PathAnimation.IsPlaying)
                {
                    PathAnimation.Update(renderContext);
                    Scale(PathAnimation.Scale);
                    Rotate(PathAnimation.Rotation);
                    Translate(PathAnimation.Position);
                    Effect = PathAnimation.Effect;
                }
                if (OpacityAnimation != null && OpacityAnimation.IsPlaying)
                {
                    OpacityAnimation.Update(renderContext);
                    Color = Color.ApplyPremultipliedAlpha(OpacityAnimation.Opacity);
                    
                }
            }
            base.Update(renderContext);
        }

        public void ChangeOpacity(float from, float to, int time, Action callback)
        {
            OpacityAnimation = new OpacityAnimation(from, to, time);
            if (callback != null)
                OpacityAnimation.Completed += callback;
            OpacityAnimation.PlayAnimation();
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
            _textures.Clear();
            _textures.Add(_texture);
            //CreateBoundingRect((int)Width, (int)Height);
        }

        public void SetAssetFile(string asset)
        {
            _assetFile = asset;
            //CreateBoundingRect((int)Width, (int)Height);
        }

        public override void Draw(RenderContext renderContext)
        {
            if (!_textures.Any())
                return;

            if (SkipDrawing)
            {
                SkipDrawing = false;
                return;
            }
            if (CanDraw && _textures.Count>0 && _textures[_currentTextureIndex] != null)
            {

                renderContext.SpriteBatch.Draw(_textures[_currentTextureIndex], WorldPosition,
                    DrawRect, Color, MathHelper.ToRadians(WorldRotation),
                    Vector2.Zero, WorldScale, Effect, Depth);
                base.Draw(renderContext);
            }
        }

        public override void Unload()
        {
            base.Unload();
            if (IsLocalContent)//only local content is unloadeble
            {
                for (int i = 0; i < _textures.Count; i++)
                {
                    _textures[i].Dispose();
                    _textures[i] = null;
                }
                _textures.Clear();
            }
        }

        /// <summary>
        /// Renders gamesprite with children into single texture
        /// !never call inside of Draw() method
        /// </summary>
        /// <param name="renderContext"></param>
        /// <returns></returns>
        public Texture2D ToTexture(RenderContext renderContext)
        {
            return ToTexture(renderContext, (int)(Width * renderContext.DeviceScale.X), (int)(Height * renderContext.DeviceScale.Y), Color.Transparent);
        }
    }
}
