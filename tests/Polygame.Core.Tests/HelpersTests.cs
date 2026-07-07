using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sanet.Polygame.Utils;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class HelpersTests
{
    [Theory]
    [InlineData(0, 0, 1, 0, 0.0)]
    [InlineData(0, 0, 0, 1, 1.57079637f)]
    [InlineData(0, 0, -1, 0, 3.14159274f)]
    [InlineData(0, 0, 0, -1, -1.57079637f)]
    [InlineData(1, 1, 4, 5, 0.927295218f)]
    public void GetAngle_WithVectors_ReturnsAngleInRadians(float x1, float y1, float x2, float y2, float expected)
    {
        var v1 = new Vector2(x1, y1);
        var v2 = new Vector2(x2, y2);

        var result = Helpers.GetAngle(v1, v2);

        result.ShouldBe(expected, 0.0001f);
    }

    [Fact]
    public void IsPointInPolygon_PointInside_ReturnsTrue()
    {
        var polygon = new List<Vector2>
        {
            new(0, 0),
            new(10, 0),
            new(10, 10),
            new(0, 10)
        };
        var point = new Vector2(5, 5);

        var result = Helpers.IsPointInPolygon(polygon, point);

        result.ShouldBeTrue();
    }

    [Fact]
    public void IsPointInPolygon_PointOutside_ReturnsFalse()
    {
        var polygon = new List<Vector2>
        {
            new(0, 0),
            new(10, 0),
            new(10, 10),
            new(0, 10)
        };
        var point = new Vector2(15, 15);

        var result = Helpers.IsPointInPolygon(polygon, point);

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPointInPolygon_PointOnEdge_ReturnsCorrectResult()
    {
        var polygon = new List<Vector2>
        {
            new(0, 0),
            new(10, 0),
            new(10, 10),
            new(0, 10)
        };
        var point = new Vector2(5, 0);

        var result = Helpers.IsPointInPolygon(polygon, point);

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPointInPolygon_Triangle_PointInside_ReturnsTrue()
    {
        var polygon = new List<Vector2>
        {
            new(0, 0),
            new(10, 0),
            new(5, 10)
        };
        var point = new Vector2(5, 4);

        var result = Helpers.IsPointInPolygon(polygon, point);

        result.ShouldBeTrue();
    }

    [Fact]
    public void QuaternionToEuler_Identity_ReturnsZero()
    {
        var q = Quaternion.Identity;

        var result = Helpers.QuaternionToEuler(q);

        result.X.ShouldBe(0f);
        result.Y.ShouldBe(0f);
        result.Z.ShouldBe(0f);
    }

    [Fact]
    public void QuaternionToEuler_RotationAroundX_ReturnsExctractedEuler()
    {
        var q = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);

        var result = Helpers.QuaternionToEuler(q);

        result.Length().ShouldBeGreaterThan(0f);
    }

    [Fact]
    public void QuaternionToEuler_RotationAroundY_ReturnsExtractedEuler()
    {
        var q = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.PiOver2);

        var result = Helpers.QuaternionToEuler(q);

        result.Length().ShouldBeGreaterThan(0f);
    }

    [Fact]
    public void QuaternionToEuler_NorthPoleSingularity_HandlesCorrectly()
    {
        var x = 0.5f;
        var y = 0.5f;
        var z = 0.5f;
        var w = 0.5f;
        var q = new Quaternion(x, y, z, w);

        var result = Helpers.QuaternionToEuler(q);

        result.Y.ShouldBe(MathHelper.PiOver2, 0.0001f);
    }

    [Theory]
    [InlineData("#FF0000", 255, 0, 0)]
    [InlineData("#00FF00", 0, 255, 0)]
    [InlineData("#0000FF", 0, 0, 255)]
    [InlineData("#FFFFFF", 255, 255, 255)]
    [InlineData("#000000", 0, 0, 0)]
    [InlineData("#AABBCC", 170, 187, 204)]
    public void GetColorFromHexValue_ValidHex_ReturnsCorrectColor(string hex, byte r, byte g, byte b)
    {
        var result = Helpers.GetColorFromHexValue(hex);

        ((int)result.R).ShouldBe(r);
        ((int)result.G).ShouldBe(g);
        ((int)result.B).ShouldBe(b);
        ((int)result.A).ShouldBe(255);
    }

    [Fact]
    public void GetColorFromText_ThreeComponents_ReturnsWhite()
    {
        var result = Helpers.GetColorFromText("255;0;0");

        ((int)result.R).ShouldBe(255);
        ((int)result.G).ShouldBe(255);
        ((int)result.B).ShouldBe(255);
        ((int)result.A).ShouldBe(255);
    }

    [Fact]
    public void GetColorFromText_HexFormat_ReturnsCorrectColor()
    {
        var result = Helpers.GetColorFromText("#FF8800");

        ((int)result.R).ShouldBe(255);
        ((int)result.G).ShouldBe(136);
        ((int)result.B).ShouldBe(0);
        ((int)result.A).ShouldBe(255);
    }

    [Fact]
    public void GetColorFromText_InvalidFormat_ReturnsWhite()
    {
        var result = Helpers.GetColorFromText("invalid");

        result.ShouldBe(Color.White);
    }

    [Theory]
    [InlineData("0;0;100;200", 0, 0, 100, 200)]
    [InlineData("10;20;30;40", 10, 20, 30, 40)]
    public void GetRectFromText_ValidText_ReturnsCorrectRectangle(string text, int x, int y, int w, int h)
    {
        var result = Helpers.GetRectFromText(text);

        result.X.ShouldBe(x);
        result.Y.ShouldBe(y);
        result.Width.ShouldBe(w);
        result.Height.ShouldBe(h);
    }

    [Fact]
    public void GetRectFromText_InvalidText_ReturnsEmptyRectangle()
    {
        var result = Helpers.GetRectFromText("invalid");

        result.ShouldBe(new Rectangle());
    }
}
