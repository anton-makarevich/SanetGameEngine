using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class PerspectiveCamera:BaseCamera
    {
        

        public PerspectiveCamera()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 3f, 800f / 480f, 0.1f, 500f);
        }

        
    }
}
