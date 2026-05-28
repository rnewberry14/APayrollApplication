using Xunit;

namespace ClearPathPayroll.Tests;

public class TesterPackagingTests
{
    [Fact]
    public void PackagingScript_ContainsRequiredBuildAndPackageSteps()
    {
        var script = File.ReadAllText(RepoPath("packaging", "build-tester-package.ps1"));

        Assert.Contains("dotnet restore", script);
        Assert.Contains("dotnet build", script);
        Assert.Contains("dotnet test", script);
        Assert.Contains("dotnet publish", script);
        Assert.Contains("ClearPathPayroll-TesterPackage", script);
        Assert.Contains("Compress-Archive", script);
        Assert.Contains("README-TESTERS.md", script);
        Assert.Contains("START-ClearPathPayroll.bat", script);
        Assert.Contains("RESET-DEMO-DATA.bat", script);
        Assert.Contains("TROUBLESHOOTING-TESTER-STARTUP.md", script);
        Assert.Contains("MANUAL-TEST-PACKET.md", script);
        Assert.Contains("TESTER-FEEDBACK-FORM.md", script);
    }

    [Fact]
    public void PackagingScript_ContainsSafetyExclusionsAndSecretScan()
    {
        var script = File.ReadAllText(RepoPath("packaging", "build-tester-package.ps1"));

        Assert.Contains(".git", script);
        Assert.Contains("bin", script);
        Assert.Contains("obj", script);
        Assert.Contains("secrets.json", script);
        Assert.Contains("*.db", script);
        Assert.Contains("*.pdf", script);
        Assert.Contains("*.csv", script);
        Assert.Contains("api[_-]?key", script);
        Assert.Contains("Assert-NoSensitivePackageContent", script);
        Assert.Contains("Tester package only. Do not use for real payroll.", script);
    }

    [Fact]
    public void PackageManifest_ListsIncludedExcludedLimitationsAndWarnings()
    {
        var manifest = File.ReadAllText(RepoPath("packaging", "package-manifest.md"));

        Assert.Contains("App name: ClearPath Payroll", manifest);
        Assert.Contains("Build date:", manifest);
        Assert.Contains("Included Files", manifest);
        Assert.Contains("Excluded Files", manifest);
        Assert.Contains("Known Limitations", manifest);
        Assert.Contains("Safety Warnings", manifest);
        Assert.Contains("Do not enter real SSNs", manifest);
        Assert.Contains("No production ACH submission", manifest);
        Assert.Contains("No production tax filing", manifest);
    }

    [Fact]
    public void TesterLaunchFiles_ContainBeginnerSafetyAndLocalOnlyInstructions()
    {
        var readme = File.ReadAllText(RepoPath("README-TESTERS.md"));
        var start = File.ReadAllText(RepoPath("START-ClearPathPayroll.bat"));
        var reset = File.ReadAllText(RepoPath("RESET-DEMO-DATA.bat"));
        var troubleshooting = File.ReadAllText(RepoPath("TROUBLESHOOTING-TESTER-STARTUP.md"));

        Assert.Contains("What This Demo Is", readme);
        Assert.Contains("What This Demo Is Not", readme);
        Assert.Contains("Do not enter real SSNs", readme);
        Assert.Contains("START-ClearPathPayroll.bat", readme);
        Assert.Contains("RESET-DEMO-DATA.bat", readme);
        Assert.Contains("http://localhost:5080", readme);

        Assert.Contains("ClearPathPayroll.exe", start);
        Assert.Contains("ASPNETCORE_URLS=http://localhost:5080", start);
        Assert.Contains("DOTNET_ENVIRONMENT=Development", start);
        Assert.Contains("PrototypeMode__LocalDbDatabaseName=ClearPathPayroll.TesterPackage.LocalDemo", start);
        Assert.Contains("LimitedLiabilityMode__LocalDatabaseProvider=SQLite", start);
        Assert.Contains("ClearPathPayroll-startup.log", start);
        Assert.Contains("Invoke-WebRequest", start);
        Assert.Contains("/D \"%~dp0\"", start);
        Assert.Contains("Tester package only", start);
        Assert.Contains("start \"\" \"http://localhost:5080\"", start);

        Assert.Contains("choice /C YN", reset);
        Assert.Contains("App_Data\\*.db", reset);
        Assert.Contains("--reset-demo-data", reset);
        Assert.Contains("DOTNET_ENVIRONMENT=Development", reset);
        Assert.Contains("PrototypeMode__LocalDbDatabaseName=ClearPathPayroll.TesterPackage.LocalDemo", reset);
        Assert.Contains("LimitedLiabilityMode__LocalDatabaseProvider=SQLite", reset);
        Assert.Contains("It does not delete files outside this folder.", reset);
        Assert.DoesNotContain("rmdir /s", reset, StringComparison.OrdinalIgnoreCase);

        Assert.Contains("App Will Not Start", troubleshooting);
        Assert.Contains("Browser Does Not Open", troubleshooting);
        Assert.Contains("Port Already In Use", troubleshooting);
        Assert.Contains("Missing .NET Runtime", troubleshooting);
        Assert.Contains("Windows Security Warning", troubleshooting);
        Assert.Contains("What Not To Send", troubleshooting);
        Assert.Contains("API keys", troubleshooting);
    }

    [Fact]
    public void HomePage_ProvidesNoviceStartPathAndSafetyWarning()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Pages", "Home.razor"));

        Assert.Contains("ClearPath Payroll Local Demo", content);
        Assert.Contains("Local demo only", content);
        Assert.Contains("/help?article=getting-started-local-demo", content);
        Assert.Contains("/demo/seed-data", content);
        Assert.Contains("/prototype-test-checklist", content);
        Assert.DoesNotContain("Features coming soon", content);
    }

    [Fact]
    public void Program_AppliesLocalDemoMigrationsAndSupportsResetArgument()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Program.cs"));

        Assert.Contains("--reset-demo-data", content);
        Assert.Contains("EnsureDeletedAsync", content);
        Assert.Contains("MigrateAsync", content);
        Assert.Contains("EnsureCreatedAsync", content);
        Assert.Contains("UseSqlite", content);
        Assert.Contains("ClearProviders", content);
        Assert.Contains("PersistKeysToFileSystem", content);
        Assert.Contains("!localOnlyEnabled", content);
    }

    [Fact]
    public void Readme_ExplainsHowToRunPackagingScript()
    {
        var readme = File.ReadAllText(RepoPath("README.md"));

        Assert.Contains("Building a Tester Package", readme);
        Assert.Contains(".\\packaging\\build-tester-package.ps1", readme);
        Assert.Contains("dist\\ClearPathPayroll-TesterPackage.zip", readme);
    }

    private static string RepoPath(params string[] parts)
    {
        var root = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            ".."));

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
    }
}
