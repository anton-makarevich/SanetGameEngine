using System;
namespace Sanet.XNAEngine
{
    public interface IGameObject
    {
        
        void Draw(global::Sanet.XNAEngine.RenderContext renderContext);
        void Initialize();
        void LoadContent(global::Microsoft.Xna.Framework.Content.ContentManager contentManager, bool isLocal);
        void Update(global::Sanet.XNAEngine.RenderContext renderContext);

        void AddChild(IGameObject child);

        bool CanDraw { get; set; }
        bool ForceUpdate { get; set; }

        bool IsLocalContent { get; set; }

        string CustomContent { get; set; }

        int Z { get; set; }
        string Tag {get;set;}

        string Name { get; set; }
    }
}
