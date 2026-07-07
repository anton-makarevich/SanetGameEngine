using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Interfaces;
using Sanet.Polygame.SceneManager;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class GameObject2DTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var obj = new GameObject2D();

        obj.LocalScale.ShouldBe(Vector2.One);
        obj.WorldScale.ShouldBe(Vector2.One);
        obj.CanDraw.ShouldBeTrue();
        obj.SkipDrawing.ShouldBeFalse();
        obj.DrawInFrontOf3D.ShouldBeTrue();
        obj.Children.ShouldBeEmpty();
        obj.Parent.ShouldBeNull();
        obj.DeviceScale.ShouldBe(Vector2.One);
    }

    [Fact]
    public void AddChild_SetsParent()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();

        parent.AddChild(child);

        child.Parent.ShouldBe(parent);
    }

    [Fact]
    public void AddChild_NonGameObject2D_DoesNothing()
    {
        var parent = new GameObject2D();
        var invalidChild = new MockGameObject();

        parent.AddChild(invalidChild);

        parent.Children.ShouldBeEmpty();
    }

    [Fact]
    public void AddChild_AddsToChildren()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();

        parent.AddChild(child);

        parent.Children.ShouldContain(child);
    }

    [Fact]
    public void AddChild_AlreadyInOtherParent_RemovesFromOldParent()
    {
        var oldParent = new GameObject2D();
        var newParent = new GameObject2D();
        var child = new GameObject2D();
        oldParent.AddChild(child);

        newParent.AddChild(child);

        oldParent.Children.ShouldNotContain(child);
        child.Parent.ShouldBe(newParent);
    }

    [Fact]
    public void AddChild_InheritsZFromParentWhenZero()
    {
        var parent = new GameObject2D { Z = 5 };
        var child = new GameObject2D { Z = 0 };

        parent.AddChild(child);

        child.Z.ShouldBe(5);
    }

    [Fact]
    public void AddChild_PreservesChildZWhenNonZero()
    {
        var parent = new GameObject2D { Z = 5 };
        var child = new GameObject2D { Z = 10 };

        parent.AddChild(child);

        child.Z.ShouldBe(10);
    }

    [Fact]
    public void RemoveChild_ClearsParent()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();
        parent.AddChild(child);

        parent.RemoveChild(child);

        child.Parent.ShouldBeNull();
        parent.Children.ShouldNotContain(child);
    }

    [Fact]
    public void RemoveChild_NonExistent_DoesNothing()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();

        parent.RemoveChild(child);

        parent.Children.ShouldBeEmpty();
    }

    [Fact]
    public void GetChildByName_ReturnsDirectChild()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D { Name = "Target" };
        parent.AddChild(child);

        var result = parent.GetChildByName("Target");

        result.ShouldBe(child);
    }

    [Fact]
    public void GetChildByName_ReturnsNestedChild()
    {
        var root = new GameObject2D();
        var middle = new GameObject2D();
        var target = new GameObject2D { Name = "Target" };
        root.AddChild(middle);
        middle.AddChild(target);

        var result = root.GetChildByName("Target");

        result.ShouldBe(target);
    }

    [Fact]
    public void GetChildByName_NotFound_ReturnsNull()
    {
        var parent = new GameObject2D();
        parent.AddChild(new GameObject2D { Name = "Child1" });

        var result = parent.GetChildByName("NonExistent");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetChildByNameGeneric_ReturnsTypedChild()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D { Name = "Target" };
        parent.AddChild(child);

        var result = parent.GetChildByName<GameObject2D>("Target");

        result.ShouldBe(child);
    }

    [Fact]
    public void GetChildByNameGeneric_WrongType_ReturnsNull()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D { Name = "Target" };
        parent.AddChild(child);

        var result = parent.GetChildByName<GameObject2D>("NonExistent");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetChildByTagGeneric_ReturnsTaggedChild()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D { Tag = "special" };
        parent.AddChild(child);

        var result = parent.GetChildByTag<GameObject2D>("special");

        result.ShouldBe(child);
    }

    [Fact]
    public void ReorderChildren_SortsByZ()
    {
        var parent = new GameObject2D();
        var child1 = new GameObject2D { Z = 3 };
        var child2 = new GameObject2D { Z = 1 };
        var child3 = new GameObject2D { Z = 2 };
        parent.AddChild(child1);
        parent.AddChild(child2);
        parent.AddChild(child3);

        parent.ReorderChildren();

        parent.Children[0].Z.ShouldBe(1);
        parent.Children[1].Z.ShouldBe(2);
        parent.Children[2].Z.ShouldBe(3);
    }

    [Fact]
    public void Rotate_SetsLocalRotationAndRadians()
    {
        var obj = new GameObject2D();

        obj.Rotate(90f);

        obj.LocalRotation.ShouldBe(90f);
        obj.LocalRotationInRad.ShouldBe(MathHelper.PiOver2, 0.0001f);
    }

    [Fact]
    public void Rotate_Zero_ResetsRotation()
    {
        var obj = new GameObject2D();
        obj.Rotate(45f);

        obj.Rotate(0f);

        obj.LocalRotation.ShouldBe(0f);
        obj.LocalRotationInRad.ShouldBe(0f);
    }

    [Fact]
    public void Scale_SingleValue_SetsUniformScale()
    {
        var obj = new GameObject2D();

        obj.Scale(3f);

        obj.LocalScale.ShouldBe(new Vector2(3, 3));
    }

    [Fact]
    public void Scale_TwoValues_SetsXYScale()
    {
        var obj = new GameObject2D();

        obj.Scale(2f, 4f);

        obj.LocalScale.ShouldBe(new Vector2(2, 4));
    }

    [Fact]
    public void Scale_Vector_SetsScale()
    {
        var obj = new GameObject2D();

        obj.Scale(new Vector2(5, 6));

        obj.LocalScale.ShouldBe(new Vector2(5, 6));
    }

    [Fact]
    public void CreateBoundingRect_SetsBoundingRect()
    {
        var obj = new GameObject2D();

        obj.CreateBoundingRect(100, 50);

        obj.BoundingRect.ShouldNotBeNull();
        obj.BoundingRect.Value.Width.ShouldBe(100);
        obj.BoundingRect.Value.Height.ShouldBe(50);
    }

    [Fact]
    public void CreateBoundingRect_WithOffset_IncludesOffset()
    {
        var obj = new GameObject2D();

        obj.CreateBoundingRect(100, 50, new Vector2(10, 5));

        obj.BoundingRect.Value.Width.ShouldBe(110);
        obj.BoundingRect.Value.Height.ShouldBe(55);
    }

    [Fact]
    public void HitTest_WithBoundingRect_ReturnsTrueForContainedPoint()
    {
        var obj = new GameObject2D();
        obj.CreateBoundingRect(100, 100);

        var result = obj.HitTest(new Vector2(50, 50));

        result.ShouldBeTrue();
    }

    [Fact]
    public void HitTest_WithBoundingRect_ReturnsFalseForOutsidePoint()
    {
        var obj = new GameObject2D();
        obj.CreateBoundingRect(100, 100);

        var result = obj.HitTest(new Vector2(200, 200));

        result.ShouldBeFalse();
    }

    [Fact]
    public void HitTest_NoBoundingRect_ReturnsFalse()
    {
        var obj = new GameObject2D();

        var result = obj.HitTest(new Vector2(50, 50));

        result.ShouldBeFalse();
    }

    [Fact]
    public void HitTest_WithChildHit_ReturnsTrue()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();
        parent.AddChild(child);
        child.CreateBoundingRect(100, 100);

        var result = parent.HitTest(new Vector2(50, 50));

        result.ShouldBeTrue();
    }

    [Fact]
    public void HitTest_WithGameObject_HitsIntersecting()
    {
        var obj1 = new GameObject2D();
        obj1.CreateBoundingRect(100, 100);
        var obj2 = new GameObject2D();
        obj2.CreateBoundingRect(100, 100);

        var result = obj1.HitTest(obj2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HitTest_WithGameObject_NoIntersection_ReturnsFalse()
    {
        var obj1 = new GameObject2D();
        obj1.CreateBoundingRect(10, 10);
        var obj2 = new GameObject2D();

        var result = obj1.HitTest(obj2);

        result.ShouldBeFalse();
    }

    [Fact]
    public void HitTest_WithGameObject_NoBoundingRect_ReturnsFalse()
    {
        var obj1 = new GameObject2D();
        var obj2 = new GameObject2D();

        var result = obj1.HitTest(obj2);

        result.ShouldBeFalse();
    }

    [Fact]
    public void HitTest_ExcludeChildren_PointOutsideParentInsideChild_ReturnsFalse()
    {
        var parent = new GameObject2D();
        parent.CreateBoundingRect(10, 10);
        var child = new GameObject2D();
        child.CreateBoundingRect(100, 100);
        parent.AddChild(child);

        var result = parent.HitTest(new Vector2(50, 50), false);

        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("100;200", 100, 200)]
    [InlineData("0;0", 0, 0)]
    [InlineData("-5;-10", -5, -10)]
    public void Translate_String_SetsLocalPosition(string input, float expectedX, float expectedY)
    {
        var obj = new GameObject2D();

        obj.Translate(input);

        obj.LocalPosition.X.ShouldBe(expectedX);
        obj.LocalPosition.Y.ShouldBe(expectedY);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("100")]
    [InlineData("100;200;300")]
    public void Translate_String_InvalidInput_DoesNotChangePosition(string input)
    {
        var obj = new GameObject2D();
        obj.LocalPosition = new Vector2(10, 20);

        obj.Translate(input);

        obj.LocalPosition.X.ShouldBe(10);
        obj.LocalPosition.Y.ShouldBe(20);
    }

    [Fact]
    public void CanDraw_WhenParentInvisible_ReturnsFalse()
    {
        var parent = new GameObject2D { CanDraw = false };
        var child = new GameObject2D { CanDraw = true };
        parent.AddChild(child);

        child.CanDraw.ShouldBeFalse();
    }

    [Fact]
    public void CanDraw_WhenParentVisible_ReturnsTrue()
    {
        var parent = new GameObject2D { CanDraw = true };
        var child = new GameObject2D { CanDraw = true };
        parent.AddChild(child);

        child.CanDraw.ShouldBeTrue();
    }

    [Fact]
    public void CanDraw_SetFalse_RaisesEvent()
    {
        var obj = new GameObject2D();
        var eventRaised = false;
        obj.OnVisibilityChanged += () => eventRaised = true;

        obj.CanDraw = false;

        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void CanDraw_SetSameValue_DoesNotRaiseEvent()
    {
        var obj = new GameObject2D();
        var eventRaised = false;
        obj.OnVisibilityChanged += () => eventRaised = true;

        obj.CanDraw = true;

        eventRaised.ShouldBeFalse();
    }

    [Fact]
    public void PivotPoint_Set_CascadesToChildren()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D { LocalPosition = new Vector2(10, 20) };
        parent.AddChild(child);

        parent.PivotPoint = new Vector2(50, 50);

        child.PivotPoint.ShouldBe(new Vector2(40, 30));
    }

    [Fact]
    public void DeviceScalingOrientation_Set_CascadesToChildren()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();
        parent.AddChild(child);

        parent.DeviceScalingOrientation = GameObjectOrientation.Horizontal;

        child.DeviceScalingOrientation.ShouldBe(GameObjectOrientation.Horizontal);
    }

    [Fact]
    public void WorldPivot_WhenNotSet_UsesParentWorldPivot()
    {
        var parent = new GameObject2D();
        parent.WorldPivot = new Vector2(100, 200);
        var child = new GameObject2D();
        parent.AddChild(child);

        child.WorldPivot.ShouldBe(new Vector2(100, 200));
    }

    [Fact]
    public void WorldPivot_WhenSet_ReturnsValue()
    {
        var obj = new GameObject2D();

        obj.WorldPivot = new Vector2(50, 60);

        obj.WorldPivot.ShouldBe(new Vector2(50, 60));
    }

    [Fact]
    public void WorldPivot_Default_ReturnsZero()
    {
        var obj = new GameObject2D();

        obj.WorldPivot.ShouldBe(Vector2.Zero);
    }

    [Fact]
    public void Indexer_ValidIndex_ReturnsChild()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();
        parent.AddChild(child);

        var result = parent[0];

        result.ShouldBe(child);
    }

    [Fact]
    public void Indexer_InvalidIndex_ReturnsNull()
    {
        var parent = new GameObject2D();

        var result = parent[0];

        result.ShouldBeNull();
    }

    [Fact]
    public void DrawBoundingRect_InheritsFromParent()
    {
        var parent = new GameObject2D { DrawBoundingRect = true };
        var child = new GameObject2D();
        parent.AddChild(child);

        child.DrawBoundingRect.ShouldBeTrue();
    }

    [Fact]
    public void CustomContent_Set_UpdatesIsCustomContent()
    {
        var obj = new GameObject2D();

        obj.CustomContent = "test";

        obj.IsCustomContent.ShouldBeTrue();
        obj.CustomContent.ShouldBe("test");
    }

    [Fact]
    public void CustomContent_SetNullOrEmpty_IsCustomContentIsFalse()
    {
        var obj = new GameObject2D();
        obj.CustomContent = "test";

        obj.CustomContent = "";

        obj.IsCustomContent.ShouldBeFalse();
    }

    [Fact]
    public void Scene_WhenSet_ReturnsValue()
    {
        var obj = new GameObject2D();
        var scene = new TestGameScene("test");

        obj.Scene = scene;

        obj.Scene.ShouldBe(scene);
    }

    [Fact]
    public void Scene_WhenNotSetAndHasParent_UsesParentScene()
    {
        var grandparent = new GameObject2D();
        var parent = new GameObject2D();
        var child = new GameObject2D();
        grandparent.AddChild(parent);
        parent.AddChild(child);
        var scene = new TestGameScene("test");
        grandparent.Scene = scene;

        child.Scene.ShouldBe(scene);
    }

    [Fact]
    public void Initialize_CallsInitializeOnChildren()
    {
        var parent = new GameObject2D();
        var child = new InitializableSpy();
        parent.AddChild(child);

        parent.Initialize();

        child.Initialized.ShouldBeTrue();
    }

    [Fact]
    public void Unload_CallsUnloadOnChildren()
    {
        var parent = new GameObject2D();
        var child = new GameObject2D();
        parent.AddChild(child);

        parent.Unload();

        child.ForceUpdate.ShouldBe(child.ForceUpdate);
    }

    [Fact]
    public void ReorderByZ_SortsChildren()
    {
        var parent = new GameObject2D();
        var child1 = new GameObject2D { Z = 5 };
        var child2 = new GameObject2D { Z = 1 };
        parent.AddChild(child1);
        parent.AddChild(child2);

        parent.ReorderByZ();

        parent.Children[0].ShouldBe(child2);
        parent.Children[1].ShouldBe(child1);
    }

    private class InitializableSpy : GameObject2D
    {
        public bool Initialized { get; private set; }
        public override void Initialize()
        {
            Initialized = true;
            base.Initialize();
        }
    }

    private class MockGameObject : IGameObject
    {
        public int Z { get => 0; set { } }
        public bool CanDraw { get => true; set { } }
        public bool ForceUpdate { get => false; set { } }
        public bool IsLocalContent { get => false; set { } }
        public string CustomContent { get => ""; set { } }
        public string Tag { get => ""; set { } }
        public string Name { get => ""; set { } }
        public void AddChild(IGameObject child) { }
        public void Initialize() { }
        public void LoadContent(ContentManager contentManager, bool isLocal) { }
        public void Unload() { }
        public void Update(RenderContext renderContext) { }
        public void Draw(RenderContext renderContext) { }
    }

    private class TestGameScene : GameScene
    {
        public TestGameScene(string name) : base(name, null) { }
    }
}
