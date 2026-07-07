using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Enums;

namespace Sanet.Polygame.UIObjects;

public class ScrollBar:GameObject2D
{
    private readonly BGContainer _bg;
    private readonly BGContainer _slider;

    public ScrollBar(BGContainer bg, BGContainer slider)
    {
        _bg = bg;
        _slider = slider;

        AddChild(_bg);
        AddChild(_slider);
    }

    public float Height => _bg.Height;

    public float Width => _bg.Width;

    public float Length
    {
        get => _bg.Length;
        set => _bg.Length = value-_bg.HeaderLength;
    }

    public float ScrollLength
    {
        get => _slider.Length;
        set => _slider.Length = value;
    }

    public GameObjectOrientation Orientation => _bg.Orientation;

    public void UpdatePosition(float relativePosition)
    {
        var min = 0;
        var max = Length - ScrollLength;

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