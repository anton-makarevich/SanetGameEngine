using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Sanet.XNAEngine
{
    public static class Helpers
    {
        public static float GetAngle(Vector2 vector1, Vector2 vector2)
        {
            return (float)Math.Atan2((vector2.Y - vector1.Y), (vector2.X - vector1.X));
        }

        public static List<Vector2> ProjectPointsTo2D(Vector3[] corners, Matrix transform)
        {
            var transformedCorners = new Vector3[corners.Length];
            Vector3.Transform(corners, ref transform, transformedCorners);
            List<Vector2> returnValue = new List<Vector2>();
            foreach (var v3 in transformedCorners)
                returnValue.Add(new Vector2(v3.X, v3.Y));
            return returnValue;
        }

        /// <summary> 
        /// Determines if the given point is inside the polygon 
        /// </summary> 
        /// <param name="polygon">the vertices of polygon</param> 
        /// <param name="testPoint">the given point</param> 
        /// <returns>true if the point is inside the polygon; otherwise, false</returns> 
        public static bool IsPointInPolygon(List<Vector2> polygon, Vector2 testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Found this somewhere on the internet.
        /// Need to check if it works correctly
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vector3 QuaternionToEuler(Quaternion q1)
        {
            Vector3 v = Vector3.Zero;

            double sqw = q1.W*q1.W;
            double  sqx = q1.X*q1.X;
            double  sqy = q1.Y*q1.Y;
            double  sqz = q1.Z*q1.Z;
	        double  unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
	        var test = q1.X*q1.Y + q1.Z*q1.W;
	        if (test > 0.499*unit) { // singularity at north pole
		        v.X = (float)(2 * Math.Atan2((double)q1.X,(double)q1.W));
		        v.Y = (float)Math.PI/2;
		        v.Z = 0;
		        return v;
	        }
	        if (test < -0.499*unit) { // singularity at south pole
		        v.X = (float)( -2 * Math.Atan2((double)q1.X,(double)q1.W));
		        v.Y = (float)-Math.PI/2;
		        v.Z = 0;
		        return v;
	        }
            v.X = (float)Math.Atan2(2*(double)q1.Y*(double)q1.W-2*(double)q1.X*(double)q1.Z , sqx - sqy - sqz + sqw);
	        v.Y = (float)Math.Asin(2*test/unit);
            v.Z = (float)Math.Atan2(2 * (double)q1.X * (double)q1.W - 2 * (double)q1.Y * (double)q1.Z, -sqx + sqy - sqz + sqw);

            return v;
        } 
        //public static Vector3 QuaternionToEuler(Quaternion rotation)
        //{
        //    Vector3 rotationaxes = new Vector3();

        //    Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
        //    Vector3 up = Vector3.Transform(Vector3.Up, rotation);
        //    rotationaxes = AngleTo(new Vector3(), forward);
        //    if (rotationaxes.X == MathHelper.PiOver2)
        //    {
        //        rotationaxes.Y = (float)Math.Atan2(up.Z, up.X);
        //        rotationaxes.Z = 0;
        //    }
        //    else if (rotationaxes.X == -MathHelper.PiOver2)
        //    {
        //        rotationaxes.Y = (float)Math.Atan2(-up.Z, -up.X);
        //        rotationaxes.Z = 0;
        //    }
        //    else
        //    {
        //        up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
        //        up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));
        //        rotationaxes.Z = (float)Math.Atan2(up.Y, -up.X);
        //    }
        //    return rotationaxes;  
        //}

        ////returns Euler angles that point from one point to another
        //public static Vector3 AngleTo(Vector3 from, Vector3 location)
        //{
        //    Vector3 angle = new Vector3();
        //    Vector3 v3 = Vector3.Normalize(location - from);
        //    angle.X = (float)Math.Asin(v3.Y);
        //    angle.Y = (float)Math.Atan2(-v3.Z, -v3.X);
        //    return angle;
        //}

        public static Color GetColorFromText(string text)
        {
            if (text.StartsWith("#"))
                return GetColorFromHexValue(text);
            var ints = GetIntsFromText(text);
            if (ints.Count == 4) 
            {
                return Color.FromNonPremultiplied(ints[0], ints[1], ints[2], ints[3]);
            }
            return Color.White;
        }

        public static Color GetColorFromHexValue(string hexValue)
        {
            hexValue = hexValue.Replace("#", "");
            System.Globalization.NumberStyles style = System.Globalization.NumberStyles.HexNumber;
            int hexColorAsInteger = int.Parse(hexValue, style);
            byte[] colorData = BitConverter.GetBytes(hexColorAsInteger);

            //Mind the order.
            byte alpha = 255;
            byte red = colorData[2];
            byte green = colorData[1];
            byte blue = colorData[0];

            return new Color(red, green, blue,alpha);
        }

        public static Rectangle GetRectFromText(string text)
        {
            var ints = GetIntsFromText(text);
            if (ints.Count == 4)
            {
                return new Rectangle(ints[0], ints[1], ints[2], ints[3]);
            }
            return new Rectangle();
        }

        static List<int> GetIntsFromText(string text)
        {
            List<int> rv = new List<int>();
            var textArray = text.Split(new char[] { ';' });
            foreach (string item in textArray)
            {
                int value=0;
                if (int.TryParse(item.Trim(), out value))
                {
                    rv.Add(value);
                }

            }
            return rv;
        }
    }
}
