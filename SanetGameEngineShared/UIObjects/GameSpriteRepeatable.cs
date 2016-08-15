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
    //represents simple 2d sprite
    public class GameSpriteRepeatable : GameSprite
    {
        public GameObjectOrientation Orientation { get; set; }

        public int RepeatCount { get; set; }
        public int RepeatCount2 { get; set; }

        public GameSpriteRepeatable(string assetFile)
            : base(assetFile)
        {
            ApplyColorToChildren = true;
        }

        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);
            if (isLocal == IsLocalContent)
            {
                GenerateClones(contentManager);
            }
        }

        void GenerateClones(ContentManager contentManager)
        {
            Children.Clear();
            RenderContext renderContext = SceneManager.RenderContext;
            if (Orientation == GameObjectOrientation.Horizontal)
            {
                var x = Width;
                for (int i = 0; i < RepeatCount; i++)
                {
                    GameSprite clone = new GameSprite(AssetFile);
                    clone.IsLocalContent = IsLocalContent;
                    clone.Translate(x, 0);
                    clone.Initialize();
                    clone.CustomContent = CustomContent;
                    clone.LoadContent(contentManager,IsLocalContent);
                    clone.Z = Z;
                    AddChild(clone);
                    x += Width;
                }
            }
            else if (Orientation == GameObjectOrientation.Vertical)
            {
                var y = Height;
                for (int i = 0; i < RepeatCount; i++)
                {
                    GameSprite clone = new GameSprite(AssetFile);
                    clone.IsLocalContent = IsLocalContent;
                    clone.Translate(0, y);
                    clone.Initialize();
                    clone.LoadContent(contentManager, IsLocalContent);
                    clone.Z = Z;
                    AddChild(clone);
                    y += Height;
                }
            }
            else
            {
                var x = 0f;
                var y = 0f;
                for (int i = 0; i < RepeatCount; i++)
                {

                    for (int j = 0; j < RepeatCount2; j++)
                    {
                        if (j == 0)
                            y = 0;
                        else
                            y += Height;

                        if (x == 0 && y == 0)
                            continue;

                        GameSprite clone = new GameSprite(AssetFile);
                        clone.IsLocalContent = IsLocalContent;
                        clone.Translate(x, y);
                        clone.Initialize();
                        clone.LoadContent(contentManager, IsLocalContent);
                        clone.Z = Z;
                        AddChild(clone);
                    }

                    x += Width;
                }
            }
        }



    }
    
}
