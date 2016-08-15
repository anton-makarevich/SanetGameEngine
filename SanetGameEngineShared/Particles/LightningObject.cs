using Microsoft.Xna.Framework;


namespace Sanet.XNAEngine.Particles
{
    /// <summary>
    /// Used to create continiuos lightnings
    /// </summary>
    public class LightningObject :ParticleController
    {
        public Vector2 From { get; private set; }
        public Vector2 To { get; private set; }
        public int NumberOfStrikes { get; set; }

        public LightningObject(string name, Vector2 from, Vector2 to, int strikes)
        {
            Name = name;
            From = from;
            To = to;

            NumberOfStrikes = strikes;

            TimeToWait = 420;
            ParticlesPerFrame = 1;
        }
    }
}
