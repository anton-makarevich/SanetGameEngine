using Microsoft.Xna.Framework.Content;
using Sanet.Polygame.Enums;

namespace Sanet.Polygame.UIObjects;

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

    private void GenerateClones(ContentManager contentManager)
    {
        Children.Clear();
        var renderContext = SceneManager.SceneManager.RenderContext;
        if (Orientation == GameObjectOrientation.Horizontal)
        {
            var x = Width;
            for (var i = 0; i < RepeatCount; i++)
            {
                var clone = new GameSprite(AssetFile);
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
            for (var i = 0; i < RepeatCount; i++)
            {
                var clone = new GameSprite(AssetFile);
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
            for (var i = 0; i < RepeatCount; i++)
            {

                for (var j = 0; j < RepeatCount2; j++)
                {
                    if (j == 0)
                        y = 0;
                    else
                        y += Height;

                    if (x == 0 && y == 0)
                        continue;

                    var clone = new GameSprite(AssetFile);
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