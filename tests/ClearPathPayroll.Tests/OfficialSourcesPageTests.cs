using Xunit;

namespace ClearPathPayroll.Tests;

public class OfficialSourcesPageTests
{
    private static string ReadOfficialSourcesPage()
    {
        var pagePath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "ClearPathPayroll",
            "Components",
            "Pages",
            "OfficialSources.razor");

        return File.ReadAllText(Path.GetFullPath(pagePath));
    }

    [Fact]
    public void Page_ContainsRequiredWarningsAndReviewButton()
    {
        var content = ReadOfficialSourcesPage();

        Assert.Contains("Official source excerpt", content);
        Assert.Contains("Mark reviewed", content);
        Assert.Contains("Last reviewed", content);
        Assert.Contains("OfficialSourceService.NotCurrentWarning", content);
        Assert.Contains("OfficialSourceService.AgeReviewWarning", content);
        Assert.Contains("Review the full official instructions", content);
        Assert.Contains("User is responsible for verifying applicability", content);
    }

    [Theory]
    [InlineData("You should")]
    [InlineData("You must")]
    [InlineData("This means you are required to")]
    [InlineData("This guarantees compliance")]
    [InlineData("ClearPath recommends")]
    [InlineData("ClearPath advises")]
    public void Page_DoesNotContainProhibitedAdviceWording(string prohibitedPhrase)
    {
        var content = ReadOfficialSourcesPage();

        Assert.DoesNotContain(prohibitedPhrase, content);
    }
}
