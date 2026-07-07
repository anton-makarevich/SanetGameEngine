using System.Collections.Generic;
using System.Reflection;
using Sanet.Polygame.Enums;
using Sanet.Polygame.SceneManager;
using Shouldly;
using Xunit;
using SM = Sanet.Polygame.SceneManager.SceneManager;

namespace Sanet.Polygame.Tests;

public class SceneManagerTests
{
    public SceneManagerTests()
    {
        SM.GameScenes.Clear();
        var stack = typeof(SM).GetField("_navigatedScenes",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (stack?.GetValue(null) is Stack<string> s)
            s.Clear();
    }

    [Fact]
    public void AddGameScene_AddsToGameScenes()
    {
        var scene = new TestGameScene("TestScene");

        SM.AddGameScene(scene);

        SM.GameScenes.ShouldContain(scene);
    }

    [Fact]
    public void AddGameScene_Duplicate_DoesNotAdd()
    {
        var scene = new TestGameScene("TestScene");
        SM.AddGameScene(scene);

        SM.AddGameScene(scene);

        SM.GameScenes.Count.ShouldBe(1);
    }

    [Fact]
    public void RemoveGameScene_RemovesFromCollection()
    {
        var scene = new TestGameScene("TestScene");
        SM.AddGameScene(scene);

        SM.RemoveGameScene(scene);

        SM.GameScenes.ShouldNotContain(scene);
    }

    [Fact]
    public void IsLoaded_AllScenesLoaded_ReturnsTrue()
    {
        var scene1 = new TestGameScene("Scene1") { IsLoaded = true };
        var scene2 = new TestGameScene("Scene2") { IsLoaded = true };
        SM.AddGameScene(scene1);
        SM.AddGameScene(scene2);

        SM.IsLoaded.ShouldBeTrue();
    }

    [Fact]
    public void IsLoaded_OneSceneNotLoaded_ReturnsFalse()
    {
        var scene1 = new TestGameScene("Scene1") { IsLoaded = true };
        var scene2 = new TestGameScene("Scene2") { IsLoaded = false };
        SM.AddGameScene(scene1);
        SM.AddGameScene(scene2);

        SM.IsLoaded.ShouldBeFalse();
    }

    [Fact]
    public void SetActiveScene_WithExistingScene_ReturnsTrue()
    {
        var scene = new TestGameScene("TargetScene");
        SM.AddGameScene(scene);

        var result = SM.SetActiveScene("TargetScene");

        result.ShouldBeTrue();
    }

    [Fact]
    public void SetActiveScene_WithNonExistingScene_ReturnsFalse()
    {
        var result = SM.SetActiveScene("NonExistent");

        result.ShouldBeFalse();
    }

    [Fact]
    public void SetActiveScene_WithoutRemembering_DoesNotPushToStack()
    {
        var scene1 = new TestGameScene("Scene1");
        var scene2 = new TestGameScene("Scene2");
        SM.AddGameScene(scene1);
        SM.AddGameScene(scene2);

        SM.SetActiveScene("Scene1", false, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();
        SM.SetActiveScene("Scene2", false, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();

        SM.PreviousScene.ShouldBeNull();
    }

    [Fact]
    public void NavigateBack_WithPreviousScene_ReturnsTrue()
    {
        var scene1 = new TestGameScene("Scene1");
        var scene2 = new TestGameScene("Scene2");
        SM.AddGameScene(scene1);
        SM.AddGameScene(scene2);

        SM.SetActiveScene("Scene1", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();
        SM.SetActiveScene("Scene2", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();

        var result = SM.NavigateBack();

        result.ShouldBeTrue();
    }

    [Fact]
    public void PreviousScene_WithHistory_ReturnsLastScene()
    {
        var scene1 = new TestGameScene("Scene1");
        var scene2 = new TestGameScene("Scene2");
        SM.AddGameScene(scene1);
        SM.AddGameScene(scene2);

        SM.SetActiveScene("Scene1", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();
        SM.SetActiveScene("Scene2", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();

        SM.PreviousScene.ShouldBe(scene1);
    }

    [Fact]
    public void NavigateBack_NoHistory_ReturnsFalse()
    {
        var scene1 = new TestGameScene("Scene1");
        SM.AddGameScene(scene1);
        SM.SetActiveScene("Scene1", false, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();

        var result = SM.NavigateBack();

        result.ShouldBeFalse();
    }

    [Fact]
    public void NavigateBackToOneOf_MatchingScene_ReturnsTrue()
    {
        var scene1 = new TestGameScene("Scene1");
        var scene2 = new TestGameScene("Scene2");
        var scene3 = new TestGameScene("Scene3");
        SM.AddGameScene(scene1);
        SM.AddGameScene(scene2);
        SM.AddGameScene(scene3);

        SM.SetActiveScene("Scene1", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();
        SM.SetActiveScene("Scene2", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();
        SM.SetActiveScene("Scene3", true, SceneTranslationModes.None, 0);
        SM.ActivateNewScene();

        var result = SM.NavigateBackToOneOf(["Scene1"]);

        result.ShouldBeTrue();
    }

    private class TestGameScene : GameScene
    {
        public TestGameScene(string name) : base(name, null) { }

        public override void Deactivated() { }
    }
}
