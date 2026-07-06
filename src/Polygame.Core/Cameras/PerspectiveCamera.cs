using System;
using Microsoft.Xna.Framework;

namespace Sanet.Polygame.Cameras;

public class PerspectiveCamera:BaseCamera
{
        

    public PerspectiveCamera()
    {
        Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 3f, 800f / 480f, 0.1f, 500f);
    }

        
}