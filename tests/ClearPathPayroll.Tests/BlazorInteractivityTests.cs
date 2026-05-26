using Xunit;

namespace ClearPathPayroll.Tests;

public class BlazorInteractivityTests
{
    [Fact]
    public void App_RendersRoutesWithInteractiveServerMode()
    {
        var appPath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "ClearPathPayroll",
            "Components",
            "App.razor");

        var content = File.ReadAllText(Path.GetFullPath(appPath));

        Assert.Contains("<Routes @rendermode=\"InteractiveServer\" />", content);
        Assert.Contains("<HeadOutlet @rendermode=\"InteractiveServer\" />", content);
    }
}
