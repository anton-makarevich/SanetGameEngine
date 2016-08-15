using Microsoft.Xna.Framework;
using System;

namespace Sanet.XNAEngine.Particles
{
    /// <summary>
    /// Wrapper for object that can smoke - it might be any GameObject2D
    /// </summary>
    public class SmokingObject:ParticleController, IDisposable
    {

#region Fields
        GameObject2D _baseObject;

        Vector2 _offset;
#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseObject">base 2d object, i.e. ufo or building etc </param>
        /// <param name="offset">offset of 'smoking point' from baseobject's location</param>
        /// <param name="timeToWait">how often to generate smoke</param>
        /// <param name="smokesPerFrame">how much smokes to generate per frame</param>
        public SmokingObject(GameObject2D baseObject,Vector2 offset, int timeToWait, int smokesPerFrame, float scale)
        {
            _baseObject = baseObject;
            _offset = offset;
            TimeToWait=timeToWait;
            ParticlesPerFrame= smokesPerFrame;
            Scale = scale;
        }

#region Properties
        public Vector2 SmokingPoint
        {
            get
            {
                return _baseObject.LocalPosition+_offset;
            }
        }
                

        public float Scale
        {
            get;
            private set;
        }
#endregion

        public void Dispose()
        {
            _baseObject = null;
        }


    }
}
