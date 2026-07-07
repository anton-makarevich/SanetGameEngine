using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sanet.Polygame.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Interfaces;
using Sanet.Polygame.UIObjects;

namespace Sanet.Polygame.Particles;

/// <summary>
/// Smoke Particle manager
/// 
/// </summary>
public static class SmokeProvider
{
    private static List<GameSprite> _smokes;

    private static readonly List<SmokingObject> _smokingObjects = new List<SmokingObject>();

    private static IGameObject _smokingObject;

    private static readonly Random _rand = new Random();

    //we have touse the same texture for all particles as it's faster
    //than separate texture instances
    private static Texture2D _smokeTexture;
              

    public static void Initialize()
    {
        //generates 300 smoke particles
        _smokes = new List<GameSprite>();
            
        for (var i = 0; i <300; i++)
        {
            var smoke = new GameSprite(_smokeTexture);
            smoke.CanDraw = false;
            smoke.PivotPoint = new Vector2(16, 16);
            smoke.Z = 105;
            _smokes.Add(smoke);
        }
    }

    public static void LoadContent(ContentManager content)
    {
        _smokeTexture = content.Load<Texture2D>("Textures/Particles/smoke");
    }

    public static void Update(RenderContext context)
    {
        if (_smokingObject == null)
            return;
                    
        var toRemove = new List<SmokingObject>();

        foreach (var smoking in _smokingObjects)
        {
            //generate new smoke
            if (smoking.TimeOfLastGeneration>smoking.TimeToWait)
            {
                for (var i = 0; i < smoking.ParticlesPerFrame;i++ )
                    GenerateSmoke(smoking.SmokingPoint, smoking.Scale);

                smoking.TimeOfLastGeneration = 0;
            }
            else
                smoking.TimeOfLastGeneration += context.GameTime.ElapsedGameTime.Milliseconds;

            if (smoking.SmokingPoint.X < 0)
            {
                smoking.Dispose();
                toRemove.Add(smoking);
            }
        }

        foreach (var smoking in toRemove)
        {
            _smokingObjects.Remove(smoking);
        }

        toRemove.Clear();
        toRemove = null;

        for (var i = 0; i < _smokes.Count; i++)
        {
            var smoke = _smokes[i];
            if (!smoke.CanDraw) continue;
            smoke.Update(context);
            var location = smoke.LocalPosition;
            var dv = new Vector2(-1,0.5f);
            location+=dv;
            if (location.X < 0)
                smoke.CanDraw = false;
            else
            {
                smoke.PathAnimation.UpdateWorldPosition(dv);
            }
        }
    }

    private static void GenerateSmoke(Vector2 position, float scale)
    {
        float speed = 4;
        var smoke = _smokes.FirstOrDefault(f => !f.CanDraw);
        if (smoke != null)
        {
            float initAngle = _rand.Next(360);
            float lastAngle = _rand.Next(360);

            var initPoint = new PathAnimationPoint(position + new Vector2(_rand.Next(-20, 10), _rand.Next(-20, 10)),
                new Vector2(0.2f,0.2f)*scale,initAngle,speed);
            var lastPoint = new PathAnimationPoint(position + new Vector2(_rand.Next(-80, 70), _rand.Next(-80, 70)),
                new Vector2(1.3f,1.3f)*scale,lastAngle,speed);

            smoke.PathAnimation = new PathAnimation(new List<PathAnimationPoint> { initPoint, lastPoint });
            smoke.PathAnimation.Completed+=()=>
            {
                smoke.CanDraw=false;
            };
            var alpha = (float)_rand.Next(5, 9) / 10f;
            smoke.Color = Color.FromNonPremultiplied(new Vector4(1f, 1f, 1f, alpha));
            smoke.PathAnimation.PlayAnimation();
            smoke.CanDraw = true;
        }
    }

    public static void Draw(RenderContext context)
    {
        if (_smokingObject == null)
            return;

        for (var i = 0; i < _smokes.Count; i++)
        {
            var smoke = _smokes[i];
            if (smoke.CanDraw)
                smoke.Draw(context);
        }
    }

    public static void StartSmoking(IGameObject smokingObject)
    {
        _smokingObject=smokingObject;
    }

    public static void StopSmoking()
    {
        Clear();
        _smokingObject = null;
    }

    public static void Clear()
    {
        foreach (var smoke in _smokes)
        {
            smoke.CanDraw = false;
            if (smoke.PathAnimation != null)
            {
                smoke.PathAnimation.StopAnimation();
                smoke.PathAnimation = null;
            }
        }
            
    }

    public static void RegisterSmokingObject(SmokingObject smoking)
    {
        _smokingObjects.Add(smoking);
    }
}