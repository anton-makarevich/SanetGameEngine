using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class BaseCamera:GameObject3D
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public BaseCamera()
        {
            Projection = Matrix.CreateOrthographic(1280, 768,-100,100);
        }

        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);

            var lookAt = Vector3.Transform(Vector3.Forward, WorldRotation);
            lookAt.Normalize();

            View = Matrix.CreateLookAt(WorldPosition, (WorldPosition + lookAt), Vector3.Up);
        }
    }
}
