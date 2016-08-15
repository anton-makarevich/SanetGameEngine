using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine.Particles
{
    /// <summary>
    /// Wrapper for object that can smoke - it might be any GameObject2D
    /// </summary>
    public class ExplosingObject:ParticleController, IDisposable
    {

#region Fields
        GameObject2D _baseObject;

        Vector2 _offset;
#endregion

        public ExplosingObject(GameObject2D baseObject,Vector2 offset)
        {
            _baseObject = baseObject;
            _offset = offset;
            
        }

        public ExplosingObject(Vector2 offset)
        {
            _baseObject = new GameObject2D();
            _offset = offset;

        }

#region Properties
        public Vector2 ExplosingPoint
        {
            get
            {
                return _baseObject.LocalPosition+_offset;
            }
        }
              
               
#endregion

        public void Dispose()
        {
            _baseObject = null;
        }


    }
}
