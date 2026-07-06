using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.Polygame.Particles
{
    public abstract class ParticleController
    {
        public string Name { get; protected set; }
        public int TimeToWait
        {
            get;
            protected set;
        }

        public int TimeOfLastGeneration { get; set; }

        public int ParticlesPerFrame
        {
            get;
            protected set;
        }
    }
}
