 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class GameModel:GameObject3D
    {
        string _assetFile;
        Model _model;

        public Vector3 ModelColor { get; set; }

        public GameModel(string assetfile)
        {
            _assetFile = assetfile;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _model = content.Load<Model>(_assetFile);
            base.LoadContent(content);
        }

        public override void Draw(RenderContext context)
        {
            var transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();

                    effect.View = context.Camera.View;
                    effect.Projection = context.Camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index]*WorldMatrix;

                    effect.LightingEnabled = true;
                    effect.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);

                    //direct light
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);
                    effect.DirectionalLight0.Direction = new Vector3(0, -1, -0.8f);//light from top (1) and front(0.8)
                    
                    //light 'reflected from bottom'
                    effect.DirectionalLight1.DiffuseColor = effect.DirectionalLight0.DiffuseColor*0.5f;
                    effect.DirectionalLight1.Direction = -effect.DirectionalLight0.Direction;//direction opposite to direct light
                    effect.EmissiveColor = ModelColor;
                }
                mesh.Draw();
            }
            base.Draw(context);
        }
    }
}
