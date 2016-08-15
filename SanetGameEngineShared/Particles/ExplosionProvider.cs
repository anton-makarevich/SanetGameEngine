using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sanet.XNAEngine;
using Sanet.XNAEngine.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine.Particles
{
    /// <summary>
    /// Smoke Particle manager
    /// 
    /// </summary>
    public static class ExplosionProvider
    {
        static List<GameSprite> _explosionParticles;

        static List<ExplosingObject> _explosingObjects = new List<ExplosingObject>();

        static Random _rand = new Random();

        //we have touse the same texture for all particles as it's faster
        //than separate texture instances
        static Texture2D _explosionTexture;

        public static bool IsDrawing
        {
            get
            {
                return _explosionParticles.Any(f => f.CanDraw);
            }
        }

        public static void Initialize(int particles)
        {
            //generates x particles
            _explosionParticles = new List<GameSprite>();

            for (int i = 0; i < particles; i++)
            {
                const int w = 64;
                const int h = 128;
                var explosion = new GameSprite(_explosionTexture);
                explosion.CanDraw = false;
                explosion.PivotPoint = new Vector2(16, 16);
                explosion.Z = 105;
                explosion.DrawRect = new Rectangle(_rand.Next(2) * w, 0, w, h);
                _explosionParticles.Add(explosion);
            }
        }

        public static void LoadContent(ContentManager content, string asset)
        {
            _explosionTexture = content.Load<Texture2D>(asset);
        }

        public static void Update(RenderContext context)
        {
                                
            List<ExplosingObject> toRemove = new List<ExplosingObject>();

            foreach (var explosion in _explosingObjects)
            {
                GenerateExplosion(explosion.ExplosingPoint);
                    
                toRemove.Add(explosion);
            }

            foreach (var smoking in toRemove)
            {
                _explosingObjects.Remove(smoking);
            }

            toRemove.Clear();
            toRemove = null;

            foreach (var explosionParticle in _explosionParticles.Where(f => f.CanDraw))
            {
                explosionParticle.Update(context);
                //var location = explosionParticle.LocalPosition;
                //Vector2 dv = new Vector2(-1,_plane.Gravity*0.5f)*_plane.Speed;
                //location+=dv;
                //if (location.X < 0)
                //    explosionParticle.CanDraw = false;
                //else
                //{
                //    explosionParticle.PathAnimation.UpdateWorldPosition(dv);
                //}
            }
        }

        static void GenerateExplosion(Vector2 position)
        {
            float speed = _rand.Next(30,50);
            const int radius = 200;

            for (int i = 0; i < 360; i += 5)
            {
                float ang = MathHelper.ToRadians(i);
                var explosion = _explosionParticles.FirstOrDefault(f => !f.CanDraw);
                if (explosion != null)
                {
                    var dp = new Vector2(radius * (float)Math.Cos(ang), radius * (float)Math.Sin(ang));
                    var initPoint = new PathAnimationPoint(position + new Vector2(_rand.Next(-20, 10), _rand.Next(-20, 10)),
                        Vector2.One, i, speed);
                    var lastPoint = new PathAnimationPoint(position+dp + new Vector2(_rand.Next(-80, 70), _rand.Next(-80, 70)),
                        Vector2.One, i, speed);

                    explosion.PathAnimation = new PathAnimation(new List<PathAnimationPoint> { initPoint, lastPoint });
                    explosion.PathAnimation.Completed += () =>
                    {
                        explosion.CanDraw = false;
                    };

                    explosion.OpacityAnimation = new OpacityAnimation(1f, 0, 1500);
                    explosion.OpacityAnimation.PlayAnimation();
                    explosion.OpacityAnimation.Completed += () =>
                    {
                        explosion.CanDraw = false;
                    };
 
                    //float alpha = (float)_rand.Next(7, 9) / 10f;
                    explosion.Color = Color.FromNonPremultiplied(new Vector4(1f, 1f, 1f, 1f));
                    explosion.PathAnimation.PlayAnimation();
                    explosion.CanDraw = true;
                }
            }
        }

        public static void Draw(RenderContext context)
        {

            foreach (var smoke in _explosionParticles.Where(f => f.CanDraw))
                smoke.Draw(context);
        }
              

        public static void RegisterExplosion(ExplosingObject explosion)
        {
            _explosingObjects.Add(explosion);
        }

        public static void Clear()
        {
            foreach (var smoke in _explosionParticles.Where(f => f.CanDraw))
                smoke.CanDraw = false;
        }
    }
}
