using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sanet.XNAEngine.Particles
{
    public class LightningLine
    {
        public Vector2 A;
        public Vector2 B;
        public float Thickness;

        Vector2 _tangent;
        const float ImageThickness = 8;

        float _thicknessScale;

        float _theta;

        Vector2 _capOrigin;
        Vector2 _middleOrigin;
        Vector2 _middleScale;

        public LightningLine() { }
        public LightningLine(Vector2 a, Vector2 b, float thickness)
        {
            A = a*SceneManager.RenderContext.DeviceScale;
            B = b * SceneManager.RenderContext.DeviceScale;
            Thickness = thickness;

            _tangent = B - A;
            _theta= (float)Math.Atan2(_tangent.Y, _tangent.X);
                        
            _thicknessScale = Thickness / ImageThickness;

            _capOrigin = new Vector2(LightningProvider.HalfCircle.Width, LightningProvider.HalfCircle.Height / 2f);
            _middleOrigin = new Vector2(0, LightningProvider.LightningSegment.Height / 2f);
            _middleScale = new Vector2(_tangent.Length(), _thicknessScale);

        }

        public void Draw(RenderContext renderContext, Color tint)
        {
            
            renderContext.SpriteBatch.Draw(LightningProvider.LightningSegment, A, null, tint, _theta, _middleOrigin, _middleScale, SpriteEffects.None, 0f);
            renderContext.SpriteBatch.Draw(LightningProvider.HalfCircle, A, null, tint, _theta, _capOrigin, _thicknessScale, SpriteEffects.None, 0f);
            renderContext.SpriteBatch.Draw(LightningProvider.HalfCircle, B, null, tint, _theta + MathHelper.Pi, _capOrigin, _thicknessScale, SpriteEffects.None, 0f);
        }
    }
}
