 
 
 
 
 
 
 
 
 
 
 
 
 
 
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sanet.XNAEngine
{
    /// <summary>
    /// basic math class to describe line
    /// </summary>
    public class Line
    {
        private Vector2 _point1;
        private Vector2 _point2;

        float _length;
        float _k;
        float _b;
        float _angle;
        
        public Line(Vector2 p1, Vector2 p2)
        {
            _point1 = p1;
            _point2 = p2;

            _length=(_point1 - _point2).Length();
            _k=(_point1.Y - _point2.Y) / (_point1.X - _point2.X);
            _b=_point1.Y - K * _point1.X;

            _angle=(float)Math.Atan(K);

            var q = Quadrant;
            if (q == 1)
                _angle = _angle.ToRange(-MathHelper.PiOver2,0, MathHelper.Pi);
            else if (q == 2)
                _angle = _angle.ToRange(0, MathHelper.PiOver2, MathHelper.Pi);
            else if (q == 3)
                _angle = _angle.ToRange(MathHelper.PiOver2, MathHelper.Pi, MathHelper.Pi);
            else if (q == 4)
                _angle = _angle.ToRange(-MathHelper.Pi, -MathHelper.PiOver2, MathHelper.Pi);
        }

        public Vector2 Point1
        {
            get { return _point1; }
            set { _point1 = value; }
        }
        public Vector2 Point2
        {
            get { return _point2; }
            set { _point2 = value; }
        }

        public Vector2 WorldPoint1
        {
            get
            {
                return Point1 * SceneManager.RenderContext.DeviceScale;
            }
        }
        public Vector2 WorldPoint2
        {
            get
            {
                return Point2* SceneManager.RenderContext.DeviceScale;
            }
        }
        public float Lenth
        {
            get
            {
                return _length;
            }
        }

        public float K
        {

            get 
            {
                return _k;
            }
        }

        int Quadrant
        {
            get
            {
                Vector2 direction = _point2-_point1;
                if (direction.X>=0 && direction.Y<=0)
                    return 1;
                else if (direction.X>=0 && direction.Y>=0)
                    return 2;
                else if (direction.X <= 0 && direction.Y >= 0)
                    return 3;
                else 
                    return 4;
            }
        }
        /// <summary>
        /// Angle in radians
        /// </summary>
        public float Angle
        {
            get
            {
                return _angle;
            }
        }

        public float B
        {
            get
            {
                return _b;
            }
        }

        public bool LineIntersect(Line line, out Vector2 point)
        {
            return LineIntersect(this, line, out point);
        }

        public static bool LineIntersect(Line line1, Line line2, out Vector2 intersectPoint)
        {
            float K1 = 0;
            float K2 = 0;
            float b1 = 0;
            float b2 = 0;
            float iX = 0;
            float iY = 0;
            float X11 = 0;
            float X12 = 0;
            float X21 = 0;
            float X22 = 0;
            float Y11 = 0;
            float Y12 = 0;
            float Y21 = 0;
            float Y22 = 0;

            intersectPoint = Vector2.Zero;

            if (line1.Point1.X < line1.Point2.X)
            {
                X11 = line1.Point1.X;
                X12 = line1.Point2.X;
            }
            else
            {
                X12 = line1.Point1.X;
                X11 = line1.Point2.X;
            }
            if (line2.Point1.X < line2.Point2.X)
            {
                X21 = line2.Point1.X;
                X22 = line2.Point2.X;
            }
            else
            {
                X22 = line2.Point1.X;
                X21 = line2.Point2.X;
            }

            if (line1.Point1.Y < line1.Point2.Y)
            {
                Y11 = line1.Point1.Y;
                Y12 = line1.Point2.Y;
            }
            else
            {
                Y12 = line1.Point1.Y;
                Y11 = line1.Point2.Y;
            }

            if (line2.Point1.Y < line2.Point2.Y)
            {
                Y21 = line2.Point1.Y;
                Y22 = line2.Point2.Y;
            }
            else
            {
                Y22 = line2.Point1.Y;
                Y21 = line2.Point2.Y;
            }

            if (X21 > X12)
                return false;
            if (Y21 > Y12)
                return false;
            if (X11 > X22)
                return false;
            if (Y11 > Y22)
                return false;

            if (X22 == X21)
            {
                K1 = (line1.Point1.Y - line1.Point2.Y) / (line1.Point1.X - line1.Point2.X);
                b1 = ((line1.Point2.Y * line1.Point1.X) - (line1.Point1.Y * line1.Point2.X)) / (line1.Point1.X - line1.Point2.X);
                iY = K1 * X22 + b1;
                if (iY >= Y11 & iY <= Y12)
                {
                    if (iY >= Y21 & iY <= Y22)
                    {
                        intersectPoint = new Vector2(X22,iY);
                        
                        return true;
                    }
                }
                return false;
            }

            if (X12 == X11)
            {
                K2 = (line2.Point1.Y - line2.Point2.Y) / (line2.Point1.X - line2.Point2.X);
                b2 = ((line2.Point2.Y * line2.Point1.X) - (line2.Point1.Y * line2.Point2.X)) / (line2.Point1.X - line2.Point2.X);
                iY = K2 * X12 + b2;
                if (iY >= Y11 & iY <= Y12)
                {
                    if (iY >= Y21 & iY <= Y22)
                    {
                        intersectPoint = new Vector2(X12,iY);
                        return true;
                    }
                }
                return false;
            }

            K1 = (line1.Point1.Y - line1.Point2.Y) / (line1.Point1.X - line1.Point2.X);
            K2 = (line2.Point1.Y - line2.Point2.Y) / (line2.Point1.X - line2.Point2.X);

            if (K1 == K2)
                return false;

            b1 = ((line1.Point2.Y * line1.Point1.X) - (line1.Point1.Y * line1.Point2.X)) / (line1.Point1.X - line1.Point2.X);
            b2 = ((line2.Point2.Y * line2.Point1.X) - (line2.Point1.Y * line2.Point2.X)) / (line2.Point1.X - line2.Point2.X);
            iX = (b2 - b1) / (K1 - K2);
            iY = (K1 * b2 - K2 * b1) / (K1 - K2);
            if (iX >= X11 & iX <= X12 & iY >= Y11 & iY <= Y12)
            {
                if (iX >= X21 & iX <= X22 & iY >= Y21 & iY <= Y22)
                {
                    intersectPoint = new Vector2(iX,iY);
                    return true;
                }
            }
            return false;
        }

        public float Y(float x)
        {
            return x * K + B;
        }

        public float X(float y)
        {
            return (y- B)/K;
        }
    }
}