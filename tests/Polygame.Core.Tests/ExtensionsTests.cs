using Microsoft.Xna.Framework;
using Sanet.Polygame.Utils;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class ExtensionsTests
{
    [Fact]
    public void Rotate_ZeroAngle_ReturnsSameVector()
    {
        var vector = new Vector2(10, 5);
        var pivot = Vector2.Zero;

        var result = vector.Rotate(pivot, 0);

        result.X.ShouldBe(10);
        result.Y.ShouldBe(5);
    }

    [Fact]
    public void Rotate_NinetyDegrees_ReturnsRotatedVector()
    {
        var vector = new Vector2(10, 0);
        var pivot = Vector2.Zero;

        var result = vector.Rotate(pivot, MathHelper.PiOver2);

        result.X.ShouldBe(0, 0.0001f);
        result.Y.ShouldBe(10, 0.0001f);
    }

    [Fact]
    public void Rotate_OneEightyDegrees_ReturnsOppositeVector()
    {
        var vector = new Vector2(10, 0);
        var pivot = Vector2.Zero;

        var result = vector.Rotate(pivot, MathHelper.Pi);

        result.X.ShouldBe(-10, 0.0001f);
        result.Y.ShouldBe(0, 0.0001f);
    }

    [Fact]
    public void Rotate_WithPivot_ReturnsCorrectResult()
    {
        var vector = new Vector2(15, 5);
        var pivot = new Vector2(10, 5);

        var result = vector.Rotate(pivot, MathHelper.PiOver2);

        result.X.ShouldBe(10, 0.0001f);
        result.Y.ShouldBe(10, 0.0001f);
    }

    [Fact]
    public void Rotate90_Clockwise_ReturnsRotatedVector()
    {
        var vector = new Vector2(10, 5);

        var result = vector.Rotate90(true);

        result.X.ShouldBe(-5);
        result.Y.ShouldBe(10);
    }

    [Fact]
    public void Rotate90_CounterClockwise_ReturnsRotatedVector()
    {
        var vector = new Vector2(10, 5);

        var result = vector.Rotate90(false);

        result.X.ShouldBe(5);
        result.Y.ShouldBe(-10);
    }

    [Fact]
    public void Rotate90_Clockwise_Twice_ReturnsOpposite()
    {
        var vector = new Vector2(10, 5);

        var result = vector.Rotate90(true).Rotate90(true);

        result.X.ShouldBe(-10);
        result.Y.ShouldBe(-5);
    }

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(180f, 3.14159274f)]
    [InlineData(90f, 1.57079637f)]
    [InlineData(45f, 0.7853982f)]
    public void ToRadians_ConvertsCorrectly(float degrees, float expectedRadians)
    {
        var result = degrees.ToRadians();

        result.ShouldBe(expectedRadians, 0.0001f);
    }

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(3.14159274f, 180f)]
    [InlineData(1.57079637f, 90f)]
    [InlineData(0.7853982f, 45f)]
    public void ToDegrees_ConvertsCorrectly(float radians, float expectedDegrees)
    {
        var result = radians.ToDegrees();

        result.ShouldBe(expectedDegrees, 0.0001f);
    }

    [Fact]
    public void ToRadians_AndBack_IsIdentity()
    {
        var original = 123.456f;

        var result = original.ToRadians().ToDegrees();

        result.ShouldBe(original, 0.0001f);
    }

    [Theory]
    [InlineData(5f, 10f, 5f)]
    [InlineData(12f, 10f, 2f)]
    [InlineData(-1f, 10f, 9f)]
    [InlineData(0f, 10f, 0f)]
    [InlineData(10f, 10f, 10f)]
    [InlineData(15f, 10f, 5f)]
    public void ToRange_WithUpperBound_WrapsCorrectly(float value, float range, float expected)
    {
        var result = value.ToRange(range);

        result.ShouldBe(expected, 0.0001f);
    }

    [Theory]
    [InlineData(5f, 0f, 10f, 5f)]
    [InlineData(12f, 0f, 10f, 2f)]
    [InlineData(-5f, 0f, 10f, 5f)]
    [InlineData(15f, 0f, 10f, 5f)]
    [InlineData(10f, 0f, 10f, 10f)]
    [InlineData(0f, 0f, 10f, 0f)]
    public void ToRange_WithLowerAndUpper_WrapsCorrectly(float value, float lower, float upper, float expected)
    {
        var result = value.ToRange(lower, upper);

        result.ShouldBe(expected, 0.0001f);
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(4, true)]
    [InlineData(0, true)]
    [InlineData(-2, true)]
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(-1, false)]
    public void IsEven_ReturnsCorrectResult(int number, bool expected)
    {
        var result = number.IsEven();

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(255, 0, 0, "FF0000")]
    [InlineData(0, 255, 0, "00FF00")]
    [InlineData(0, 0, 255, "0000FF")]
    [InlineData(255, 255, 255, "FFFFFF")]
    [InlineData(0, 0, 0, "000000")]
    [InlineData(170, 187, 204, "AABBCC")]
    public void ToHexString_ReturnsCorrectHex(byte r, byte g, byte b, string expected)
    {
        var color = new Color(r, g, b);

        var result = color.ToHexString();

        result.ShouldBe(expected);
    }
}
