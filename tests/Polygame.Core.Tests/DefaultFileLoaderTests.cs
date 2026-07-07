using System.IO;
using System.Xml.Linq;
using Sanet.Polygame.Utils;
using Shouldly;
using Xunit;

namespace Sanet.Polygame.Tests;

public class DefaultFileLoaderTests
{
    [Fact]
    public void LoadDocument_WithValidFixtureFile_ReturnsXDocument()
    {
        var fixturePath = Path.GetTempFileName();
        try
        {
            var expectedXml = "<root><item name='test'>value</item></root>";
            File.WriteAllText(fixturePath, expectedXml);

            var loader = new DefaultFileLoader();
            var result = loader.LoadDocument(fixturePath);

            result.ShouldNotBeNull();
            result.Root.ShouldNotBeNull();
            result.Root.Name.LocalName.ShouldBe("root");
            result.Root.Element("item")?.Attribute("name")?.Value.ShouldBe("test");
            result.Root.Element("item")?.Value.ShouldBe("value");
        }
        finally
        {
            if (File.Exists(fixturePath))
                File.Delete(fixturePath);
        }
    }

    [Fact]
    public void LoadDocument_FileNotFound_ThrowsFileNotFoundException()
    {
        var loader = new DefaultFileLoader();

        Should.Throw<System.IO.FileNotFoundException>(() =>
            loader.LoadDocument("nonexistent_file.xml"));
    }
}
