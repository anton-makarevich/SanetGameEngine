using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Sanet.XNAEngine.Particles
{
    public static class LightningProvider
    {
        public static Texture2D HalfCircle;
        public static Texture2D LightningSegment;

        static List<LightningBolt> _bolts = new List<LightningBolt>();
        static List<LightningObject> _lightningObjects = new List<LightningObject>();

        
        public static void Load(ContentManager content)
        {
            HalfCircle = content.Load<Texture2D>("Textures/Particles/halfcircle");
            LightningSegment = content.Load<Texture2D>("Textures/Particles/lightningsegment");
        }

        public static void Update(RenderContext context)
        {
            //if (!_lightningObjects.Any())
            //    return;

            foreach (var lightning in _lightningObjects)
            {
                //generate new smoke
                if (lightning.TimeOfLastGeneration > lightning.TimeToWait)
                {
                    for (int i = 0; i < lightning.ParticlesPerFrame; i++)
                    {
                        var bolt = new LightningBolt(lightning.From,lightning.To);
                        _bolts.Add(bolt);
                        lightning.NumberOfStrikes--;
                    };

                    lightning.TimeOfLastGeneration = 0;

                }
                else
                    lightning.TimeOfLastGeneration += context.GameTime.ElapsedGameTime.Milliseconds;
                                
            }


            foreach (var bolt in _bolts)
                bolt.Update();

            _lightningObjects = _lightningObjects.Where(f => f.NumberOfStrikes > 0).ToList();
            _bolts = _bolts.Where(f => !f.IsComplete).ToList();
        }

        public static void Draw(RenderContext renderContext)
        {
            foreach (var bolt in _bolts)
                bolt.Draw(renderContext);
        }


        public static void StartLightning(LightningObject lightning)
        {
            if (IsActiveLightning(lightning.Name))
                return;

            _lightningObjects.Add(lightning);
            
        }

        public static void StopLightning(string name)
        {
            var lightning = _lightningObjects.FirstOrDefault(f => f.Name == name);
            if (lightning != null)
                _lightningObjects.Remove(lightning);
        }

        static bool IsActiveLightning(string name)
        {
            return _lightningObjects.FirstOrDefault(f => f.Name == name) != null;
        }

        public static void Clear()
        {
            _lightningObjects.Clear();
        }
    }
}
