using Microsoft.Xna.Framework;
namespace Sanet.XNAEngine
{
    public class ScrollBar:GameObject2D
    {
        BGContainer _bg;
        BGContainer _slider;

        public ScrollBar(BGContainer bg, BGContainer slider)
        {
            _bg = bg;
            _slider = slider;

            AddChild(_bg);
            AddChild(_slider);
        }

        public float Height
        {
            get
            { return _bg.Height; }
        }

        public float Width
        {
            get
            {
                return _bg.Width;
            }
        }

        public float Length
        {
            get
            {
                return _bg.Length;
            }
            set
            {
                _bg.Length = value-_bg.HeaderLength;
            }
        }

        public float ScrollLength
        {
            get
            {
                return _slider.Length;
            }
            set
            {
                _slider.Length = value;
            }
        }

        public GameObjectOrientation Orientation
        {
            get
            {
                return _bg.Orientation;
            }
        }

        public void UpdatePosition(float relativePosition)
        {
            int min = 0;
            float max = Length - ScrollLength;

            var y = relativePosition * max;
            if (y < min)
                y = min;
            if (y > max)
                y = max;

           if (Orientation == GameObjectOrientation.Horizontal)
                    _slider.Translate(y, 0);
                else
                    _slider.Translate(0, y);
            
        }
    }
}