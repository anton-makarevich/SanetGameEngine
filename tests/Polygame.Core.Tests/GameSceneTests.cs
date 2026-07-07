using System.Linq;
using Microsoft.Xna.Framework.Content;
using Sanet.Polygame.BaseObjects;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Interfaces;
using Sanet.Polygame.SceneManager;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class GameSceneTests
{
    [Fact]
    public void Constructor_SetsSceneName()
    {
        var scene = new TestGameScene("MyScene");

        scene.SceneName.ShouldBe("MyScene");
    }

    [Fact]
    public void Constructor_InitializesCollections()
    {
        var scene = new TestGameScene("Test");

        scene.SceneObjects3D.ShouldBeEmpty();
        scene.OtherSceneObjects.ShouldBeEmpty();
        scene.Buttons.ShouldBeEmpty();
        scene.ToggleButtons.ShouldBeEmpty();
        scene.Texts.ShouldBeEmpty();
        scene.OutAnimatedObjects.ShouldBeEmpty();
        scene.InAnimatedObjects.ShouldBeEmpty();
        scene.TextFields.ShouldBeEmpty();
    }

    [Fact]
    public void IsLoaded_DefaultIsFalse()
    {
        var scene = new TestGameScene("Test");

        scene.IsLoaded.ShouldBeFalse();
    }

    [Fact]
    public void IsLoaded_CanBeSet()
    {
        var scene = new TestGameScene("Test");

        scene.IsLoaded = true;

        scene.IsLoaded.ShouldBeTrue();
    }

    [Fact]
    public void CanNavigateTo_DefaultIsTrue()
    {
        var scene = new TestGameScene("Test");

        scene.CanNavigateTo.ShouldBeTrue();
    }

    [Fact]
    public void Mode_DefaultIsInAnimation()
    {
        var scene = new TestGameScene("Test");

        scene.Mode.ShouldBe(GameSceneModes.InAnimation);
    }

    [Fact]
    public void TranslationMode_DefaultIsNone()
    {
        var scene = new TestGameScene("Test");

        scene.TranslationMode.ShouldBe(SceneTranslationModes.None);
    }

    [Fact]
    public void IsModalActive_DefaultIsFalse()
    {
        var scene = new TestGameScene("Test");

        scene.IsModalActive.ShouldBeFalse();
    }

    [Fact]
    public void IsBusy_DefaultIsFalse()
    {
        var scene = new TestGameScene("Test");

        scene.IsBusy.ShouldBeFalse();
    }

    [Fact]
    public void AddSceneObject_GameObject2D_AddsToRoot()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D();

        scene.AddSceneObject(obj);

        scene.GetSceneObject2DByName(obj.Name).ShouldBe(obj);
    }

    [Fact]
    public void AddSceneObject_GameObject2D_SetsScene()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D();

        scene.AddSceneObject(obj);

        obj.Scene.ShouldBe(scene);
    }

    [Fact]
    public void AddSceneObject_GameObject2D_Duplicate_DoesNotAddAgain()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D();
        scene.AddSceneObject(obj);

        scene.AddSceneObject(obj);

        scene.GetSceneObject2DByName(obj.Name).ShouldBe(obj);
        scene.RemoveSceneObject(obj);
        scene.GetSceneObject2DByName(obj.Name).ShouldBeNull();
        obj.Scene.ShouldBeNull();
    }

    [Fact]
    public void AddSceneObject_GameObject2D_MovesFromPreviousScene()
    {
        var scene1 = new TestGameScene("Scene1");
        var scene2 = new TestGameScene("Scene2");
        var obj = new GameObject2D();
        scene1.AddSceneObject(obj);

        scene2.AddSceneObject(obj);

        obj.Scene.ShouldBe(scene2);
        scene1.GetSceneObject2DByName(obj.Name).ShouldBeNull();
    }

    [Fact]
    public void RemoveSceneObject_GameObject2D_RemovesFromScene()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D();
        scene.AddSceneObject(obj);

        scene.RemoveSceneObject(obj);

        scene.GetSceneObject2DByName(obj.Name).ShouldBeNull();
        obj.Scene.ShouldBeNull();
    }

    [Fact]
    public void RemoveSceneObject_GameObject2D_NotInScene_DoesNotThrow()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D();

        Should.NotThrow(() => scene.RemoveSceneObject(obj));
    }

    [Fact]
    public void AddSceneObject_GameObject3D_AddsToList()
    {
        var scene = new TestGameScene("Test");
        var obj = new TestGameObject3D();

        scene.AddSceneObject(obj);

        scene.SceneObjects3D.ShouldContain(obj);
    }

    [Fact]
    public void AddSceneObject_GameObject3D_SetsScene()
    {
        var scene = new TestGameScene("Test");
        var obj = new TestGameObject3D();

        scene.AddSceneObject(obj);

        obj.Scene.ShouldBe(scene);
    }

    [Fact]
    public void AddSceneObject_GameObject3D_Duplicate_DoesNotAddAgain()
    {
        var scene = new TestGameScene("Test");
        var obj = new TestGameObject3D();

        scene.AddSceneObject(obj);
        scene.AddSceneObject(obj);

        scene.SceneObjects3D.Count.ShouldBe(1);
    }

    [Fact]
    public void RemoveSceneObject_GameObject3D_RemovesFromList()
    {
        var scene = new TestGameScene("Test");
        var obj = new TestGameObject3D();
        scene.AddSceneObject(obj);

        scene.RemoveSceneObject(obj);

        scene.SceneObjects3D.ShouldNotContain(obj);
        obj.Scene.ShouldBeNull();
    }

    [Fact]
    public void RemoveSceneObject_GameObject3D_NotInScene_DoesNothing()
    {
        var scene = new TestGameScene("Test");
        var obj = new TestGameObject3D();

        scene.RemoveSceneObject(obj);

        obj.Scene.ShouldBeNull();
    }

    [Fact]
    public void AddSceneObject_IGameObject_WithGameObject2D_AddsToRoot()
    {
        var scene = new TestGameScene("Test");
        IGameObject obj = new GameObject2D();

        scene.AddSceneObject(obj);

        scene.GetFirstSceneObjectOfType<GameObject2D>().ShouldBe(obj);
    }

    [Fact]
    public void AddSceneObject_IGameObject_WithNonGameObject2D_AddsToOther()
    {
        var scene = new TestGameScene("Test");
        var obj = new MockNon2DGameObject();

        scene.AddSceneObject(obj);

        scene.OtherSceneObjects.ShouldContain(obj);
    }

    [Fact]
    public void AddSceneObject_IGameObject_Non2DDuplicate_DoesNotAddAgain()
    {
        var scene = new TestGameScene("Test");
        var obj = new MockNon2DGameObject();

        scene.AddSceneObject(obj);
        scene.AddSceneObject(obj);

        scene.OtherSceneObjects.Count.ShouldBe(1);
    }

    [Fact]
    public void GetSceneObject2DByName_WithMatchingName_ReturnsObject()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Name = "Target" };
        scene.AddSceneObject(obj);

        var result = scene.GetSceneObject2DByName("Target");

        result.ShouldBe(obj);
    }

    [Fact]
    public void GetSceneObject2DByName_WithNonMatchingName_ReturnsNull()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Name = "Target" };
        scene.AddSceneObject(obj);

        var result = scene.GetSceneObject2DByName("NonExistent");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetSceneObject2DByName_T_WithMatchingName_ReturnsObject()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Name = "Target" };
        scene.AddSceneObject(obj);

        var result = scene.GetSceneObject2DByName<GameObject2D>("Target");

        result.ShouldBe(obj);
    }

    [Fact]
    public void GetSceneObject2DByName_T_WithNonMatching_ReturnsNull()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Name = "Target" };
        scene.AddSceneObject(obj);

        var result = scene.GetSceneObject2DByName<GameObject2D>("NonExistent");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetSceneObject2DByTag_T_WithMatchingTag_ReturnsObject()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Tag = "ui-element" };
        scene.AddSceneObject(obj);

        var result = scene.GetSceneObject2DByTag<GameObject2D>("ui-element");

        result.ShouldBe(obj);
    }

    [Fact]
    public void GetSceneObject2DByTag_T_WithNonMatchingTag_ReturnsNull()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Tag = "ui-element" };
        scene.AddSceneObject(obj);

        var result = scene.GetSceneObject2DByTag<GameObject2D>("non-existent");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetFirstSceneObjectOfType_T_WithMatchingType_ReturnsFirst()
    {
        var scene = new TestGameScene("Test");
        var obj1 = new GameObject2D();
        var obj2 = new GameObject2D();
        scene.AddSceneObject(obj1);
        scene.AddSceneObject(obj2);

        var result = scene.GetFirstSceneObjectOfType<GameObject2D>();

        result.ShouldBe(obj1);
    }

    [Fact]
    public void GetFirstSceneObjectOfType_T_WithNoMatch_ReturnsNull()
    {
        var scene = new TestGameScene("Test");

        var result = scene.GetFirstSceneObjectOfType<GameObject2D>();

        result.ShouldBeNull();
    }

    [Fact]
    public void ReorderChildren_DoesNotThrow()
    {
        var scene = new TestGameScene("Test");
        var obj1 = new GameObject2D { Z = 5 };
        var obj2 = new GameObject2D { Z = 1 };
        scene.AddSceneObject(obj1);
        scene.AddSceneObject(obj2);

        Should.NotThrow(() => scene.ReorderChildren());
    }

    [Fact]
    public void Activated_SetsModeToNormal_WhenNoInAnimations()
    {
        var scene = new TestGameScene("Test");

        scene.Activated();

        scene.Mode.ShouldBe(GameSceneModes.Normal);
    }

    [Fact]
    public void StopTranslation_DoesNotThrow()
    {
        var scene = new TestGameScene("Test");

        Should.NotThrow(() => scene.StopTranslation());
    }

    [Fact]
    public void SceneData_CanBeSet()
    {
        var scene = new TestGameScene("Test");
        var doc = new System.Xml.Linq.XDocument();

        scene.SceneData = doc;

        scene.SceneData.ShouldBe(doc);
    }

    [Fact]
    public void AddSceneObject_WithInsert_ResetsZToZero()
    {
        var scene = new TestGameScene("Test");
        var obj = new GameObject2D { Z = 50 };

        scene.AddSceneObject(obj, true);

        obj.Z.ShouldBe(0);
    }

    [Fact]
    public void GetOpositeTranslationMode_SlideToLeft_ReturnsSlideToRight()
    {
        GameScene.GetOpositeTranslationMode(SceneTranslationModes.SlideToLeft)
            .ShouldBe(SceneTranslationModes.SlideToRight);
    }

    [Fact]
    public void GetOpositeTranslationMode_SlideToRight_ReturnsSlideToLeft()
    {
        GameScene.GetOpositeTranslationMode(SceneTranslationModes.SlideToRight)
            .ShouldBe(SceneTranslationModes.SlideToLeft);
    }

    [Fact]
    public void GetOpositeTranslationMode_SlideToTop_ReturnsSlideToBottom()
    {
        GameScene.GetOpositeTranslationMode(SceneTranslationModes.SlideToTop)
            .ShouldBe(SceneTranslationModes.SlideToBottom);
    }

    [Fact]
    public void GetOpositeTranslationMode_SlideToBottom_ReturnsSlideToTop()
    {
        GameScene.GetOpositeTranslationMode(SceneTranslationModes.SlideToBottom)
            .ShouldBe(SceneTranslationModes.SlideToTop);
    }

    [Fact]
    public void GetOpositeTranslationMode_FadeOut_ReturnsItself()
    {
        GameScene.GetOpositeTranslationMode(SceneTranslationModes.FadeOut)
            .ShouldBe(SceneTranslationModes.FadeOut);
    }

    [Fact]
    public void GetOpositeTranslationMode_None_ReturnsItself()
    {
        GameScene.GetOpositeTranslationMode(SceneTranslationModes.None)
            .ShouldBe(SceneTranslationModes.None);
    }

    private class TestGameScene : GameScene
    {
        public TestGameScene(string name) : base(name, null) { }
    }

    private class TestGameObject3D : GameObject3D
    {
        public TestGameObject3D() : base() { }
    }

    private class MockNon2DGameObject : IGameObject
    {
        public bool CanDraw { get; set; }
        public bool ForceUpdate { get; set; }
        public bool IsLocalContent { get; set; }
        public string CustomContent { get; set; }
        public int Z { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }

        public void AddChild(IGameObject child) { }
        public void Initialize() { }
        public void LoadContent(ContentManager contentManager, bool isLocal) { }
        public void Update(RenderContext renderContext) { }
        public void Draw(RenderContext renderContext) { }
    }
}
