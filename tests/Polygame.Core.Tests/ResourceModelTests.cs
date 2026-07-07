using System.Xml.Linq;
using NSubstitute;
using Sanet.Polygame.Interfaces;
using Sanet.Polygame.Localization;
using Sanet.Polygame.Utils;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class ResourceModelTests
{
    private readonly IFileLoader _fileLoader;

    public ResourceModelTests()
    {
        _fileLoader = Substitute.For<IFileLoader>();
        XMLLoader.FileLoader = _fileLoader;
    }

    [Fact]
    public void Constructor_NoLocaleLoaded_IsLoadedIsFalse()
    {
        var model = new ResourceModel(["en", "fr"]);

        model.IsLoaded.ShouldBeFalse();
    }

    [Fact]
    public void GetString_NoLocalesLoaded_ReturnsResourceKey()
    {
        var model = new ResourceModel(["en"]);

        var result = model.GetString("SomeResource");

        result.ShouldBe("SomeResource");
    }

    [Fact]
    public void Load_AfterAllLocales_SetsLoaded()
    {
        _fileLoader.LoadDocument(Arg.Any<string>())
            .Returns(XDocument.Parse("<root><data name='TestKey'><value>TestValue</value></data></root>"));
        var model = new ResourceModel(["en"]);

        model.LoadStep();
        model.IsLoaded.ShouldBeFalse();
        model.LoadStep();
        model.IsLoaded.ShouldBeTrue();
    }

    [Fact]
    public void GetString_AfterLoadingLocale_ReturnsCorrectValue()
    {
        _fileLoader.LoadDocument(Arg.Any<string>())
            .Returns(XDocument.Parse("<root><data name='Greeting'><value>Hello</value></data></root>"));
        var model = new ResourceModel(["en"]);
        model.LoadStep();

        var result = model.GetString("Greeting");

        result.ShouldBe("Hello");
    }

    [Fact]
    public void GetString_MissingResource_ReturnsResourceKey()
    {
        _fileLoader.LoadDocument(Arg.Any<string>())
            .Returns(XDocument.Parse("<root><data name='Greeting'><value>Hello</value></data></root>"));
        var model = new ResourceModel(["en"]);
        model.LoadStep();

        var result = model.GetString("MissingResource");

        result.ShouldBe("MissingResource");
    }

    [Fact]
    public void LoadStep_MultipleLocales_LoadsStepByStep()
    {
        _fileLoader.LoadDocument(Arg.Any<string>())
            .Returns(
                XDocument.Parse("<root><data name='Key'><value>English</value></data></root>"),
                XDocument.Parse("<root><data name='Key'><value>French</value></data></root>"));
        var model = new ResourceModel(["en", "fr"]);

        model.LoadStep();
        model.IsLoaded.ShouldBeFalse();
        model.LoadStep();
        model.IsLoaded.ShouldBeFalse();
        model.LoadStep();
        model.IsLoaded.ShouldBeTrue();
    }

    [Fact]
    public void GetString_MultipleLocales_ReturnsValueForCurrentLanguage()
    {
        _fileLoader.LoadDocument(Arg.Any<string>())
            .Returns(
                XDocument.Parse("<root><data name='Key'><value>English</value></data></root>"),
                XDocument.Parse("<root><data name='Key'><value>French</value></data></root>"));
        var model = new ResourceModel(["en", "fr"]);
        model.LoadStep();
        model.LoadStep();

        var result = model.GetString("Key");

        result.ShouldBe("English");
    }

    [Fact]
    public void CurrentLanguage_SetLanguage_ReturnsNewLanguage()
    {
        var model = new ResourceModel(["en", "fr"]);

        model.CurrentLanguage = "fr";

        model.CurrentLanguage.ShouldBe("fr");
    }

    [Fact]
    public void LoadStep_CallsFileLoaderWithCorrectPath()
    {
        _fileLoader.LoadDocument(Arg.Any<string>()).Returns(XDocument.Parse("<root/>"));
        var model = new ResourceModel(["en"]);

        model.LoadStep();

        _fileLoader.Received(1).LoadDocument(Arg.Is<string>(s =>
            s.StartsWith("Data/Strings/") && s.EndsWith("/Resources.resw")));
    }
}
