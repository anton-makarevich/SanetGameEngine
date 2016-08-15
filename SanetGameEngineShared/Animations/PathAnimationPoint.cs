 
 
 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sanet.XNAEngine.Animations
{
    public class PathAnimationPoint
    {

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }

        public float Speed { get; set; }

        public SpriteEffects Effect { get; set; }

        public PathAnimationPoint(XElement xmldata)
        {
            Position = new Vector2(int.Parse(xmldata.Attribute("X").Value), int.Parse(xmldata.Attribute("Y").Value));

            Scale = new Vector2(
                float.Parse(xmldata.Attribute("ScaleX").Value, CultureInfo.InvariantCulture),
                float.Parse(xmldata.Attribute("ScaleY").Value, CultureInfo.InvariantCulture));

            Rotation = float.Parse(xmldata.Attribute("Rotation").Value);
            Speed = float.Parse(xmldata.Attribute("Speed").Value);
            var effectString = xmldata.Attribute("Effect").Value;
            switch (effectString)
            {
                case "FlipHorizontally":
                    Effect = SpriteEffects.FlipHorizontally;
                    break;
                case "FlipVertically":
                    Effect = SpriteEffects.FlipVertically;
                    break;
                default:
                    Effect = SpriteEffects.None;
                    break;
            }
        }

        public PathAnimationPoint(Vector2 position, Vector2 scale, float rotation, float speed)
        {
            Position = position;

            Scale = scale;

            Rotation = rotation;
            Speed = speed;

        }
    }
}
