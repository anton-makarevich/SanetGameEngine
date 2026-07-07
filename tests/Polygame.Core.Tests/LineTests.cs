using Microsoft.Xna.Framework;
using Sanet.Polygame.Geometry;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class LineTests
{
    [Fact]
    public void Constructor_SetsPoint1AndPoint2()
    {
        var p1 = new Vector2(1, 2);
        var p2 = new Vector2(3, 4);

        var line = new Line(p1, p2);

        line.Point1.ShouldBe(p1);
        line.Point2.ShouldBe(p2);
    }

    [Fact]
    public void Constructor_CalculatesLength()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(3, 4));

        line.Lenth.ShouldBe(5f);
    }

    [Fact]
    public void Constructor_CalculatesSlope()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(2, 4));

        line.K.ShouldBe(2f);
    }

    [Fact]
    public void Constructor_CalculatesIntercept()
    {
        var line = new Line(new Vector2(0, 1), new Vector2(2, 3));

        line.B.ShouldBe(1f);
    }

    [Fact]
    public void Constructor_Quadrant1_ReturnsCorrectAngle()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(1, -1));

        line.Angle.ShouldBeLessThan(0);
        line.Angle.ShouldBeGreaterThan(-MathHelper.PiOver2);
    }

    [Fact]
    public void Constructor_Quadrant2_ReturnsCorrectAngle()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(1, 1));

        line.Angle.ShouldBeGreaterThan(0);
        line.Angle.ShouldBeLessThan(MathHelper.PiOver2);
    }

    [Fact]
    public void Constructor_Quadrant3_ReturnsCorrectAngle()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(-1, 1));

        line.Angle.ShouldBeGreaterThan(MathHelper.PiOver2);
        line.Angle.ShouldBeLessThan(MathHelper.Pi);
    }

    [Fact]
    public void Constructor_Quadrant4_ReturnsCorrectAngle()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(-1, -1));

        line.Angle.ShouldBeLessThan(-MathHelper.PiOver2);
        line.Angle.ShouldBeGreaterThan(-MathHelper.Pi);
    }

    [Fact]
    public void Y_GivenX_ReturnsCorrectY()
    {
        var line = new Line(new Vector2(0, 1), new Vector2(2, 5));

        var y = line.Y(1);

        y.ShouldBe(3f, 0.0001f);
    }

    [Fact]
    public void Y_HorizontalLine_ReturnsConstant()
    {
        var line = new Line(new Vector2(0, 5), new Vector2(10, 5));

        var y = line.Y(7);

        y.ShouldBe(5f, 0.0001f);
    }

    [Fact]
    public void X_GivenY_ReturnsCorrectX()
    {
        var line = new Line(new Vector2(0, 1), new Vector2(2, 5));

        var x = line.X(3);

        x.ShouldBe(1f, 0.0001f);
    }

    [Fact]
    public void LineIntersect_IntersectingLines_ReturnsTrue()
    {
        var line1 = new Line(new Vector2(0, 0), new Vector2(10, 10));
        var line2 = new Line(new Vector2(0, 10), new Vector2(10, 0));

        var result = line1.LineIntersect(line2, out var point);

        result.ShouldBeTrue();
        point.X.ShouldBe(5f, 0.0001f);
        point.Y.ShouldBe(5f, 0.0001f);
    }

    [Fact]
    public void LineIntersect_ParallelLines_ReturnsFalse()
    {
        var line1 = new Line(new Vector2(0, 0), new Vector2(10, 10));
        var line2 = new Line(new Vector2(0, 1), new Vector2(10, 11));

        var result = line1.LineIntersect(line2, out var point);

        result.ShouldBeFalse();
        point.ShouldBe(Vector2.Zero);
    }

    [Fact]
    public void LineIntersect_NonOverlappingSegments_ReturnsFalse()
    {
        var line1 = new Line(new Vector2(0, 0), new Vector2(1, 1));
        var line2 = new Line(new Vector2(10, 10), new Vector2(11, 11));

        var result = line1.LineIntersect(line2, out _);

        result.ShouldBeFalse();
    }

    [Fact]
    public void LineIntersect_PerpendicularLines_ReturnsTrue()
    {
        var line1 = new Line(new Vector2(0, 5), new Vector2(10, 5));
        var line2 = new Line(new Vector2(5, 0), new Vector2(5, 10));

        var result = line1.LineIntersect(line2, out var point);

        result.ShouldBeTrue();
        point.X.ShouldBe(5f, 0.0001f);
        point.Y.ShouldBe(5f, 0.0001f);
    }

    [Fact]
    public void LineIntersect_StaticMethod_IntersectingLines_ReturnsTrue()
    {
        var line1 = new Line(new Vector2(-5, 0), new Vector2(5, 0));
        var line2 = new Line(new Vector2(0, -5), new Vector2(0, 5));

        var result = Line.LineIntersect(line1, line2, out var point);

        result.ShouldBeTrue();
        point.X.ShouldBe(0f, 0.0001f);
        point.Y.ShouldBe(0f, 0.0001f);
    }

    [Fact]
    public void LineIntersect_VerticalLine_ReturnsCorrectIntersection()
    {
        var line1 = new Line(new Vector2(3, 0), new Vector2(3, 10));
        var line2 = new Line(new Vector2(0, 5), new Vector2(10, 5));

        var result = line1.LineIntersect(line2, out var point);

        result.ShouldBeTrue();
        point.X.ShouldBe(3f, 0.0001f);
        point.Y.ShouldBe(5f, 0.0001f);
    }

    [Fact]
    public void SetPoint1_AfterConstruction_Point1PropertyIsUpdated()
    {
        var line = new Line(new Vector2(0, 0), new Vector2(2, 4));

        line.Point1 = new Vector2(5, 6);

        line.Point1.ShouldBe(new Vector2(5, 6));
    }

    [Fact]
    public void SetPoint2_AfterConstruction_Point2PropertyIsUpdated()
    {
        var line = new Line(new Vector2(0, 1), new Vector2(2, 3));

        line.Point2 = new Vector2(4, 7);

        line.Point2.ShouldBe(new Vector2(4, 7));
    }
}
