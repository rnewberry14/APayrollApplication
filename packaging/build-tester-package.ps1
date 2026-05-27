[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$PackageName = "ClearPathPayroll-TesterPackage"
)

$ErrorActionPreference = "Stop"

$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptRoot
$SolutionPath = Join-Path $RepoRoot "ClearPathPayroll.sln"
$ProjectPath = Join-Path $RepoRoot "src\ClearPathPayroll\ClearPathPayroll.csproj"
$PublishOutput = Join-Path $RepoRoot "dist\publish-output"
$DistRoot = Join-Path $RepoRoot "dist"
$PackageRoot = Join-Path $DistRoot $PackageName
$ZipPath = Join-Path $DistRoot "$PackageName.zip"
$BuildDateUtc = (Get-Date).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'")

function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host ""
    Write-Host "== $Name =="
    $global:LASTEXITCODE = 0
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "Step '$Name' failed with exit code $LASTEXITCODE."
    }
}

function Copy-IfExists {
    param(
        [string]$Source,
        [string]$Destination
    )

    if (Test-Path -LiteralPath $Source) {
        Copy-Item -LiteralPath $Source -Destination $Destination -Force
    }
}

function Remove-PackageArtifactsByPattern {
    param([string]$Root)

    $patterns = @(
        ".git",
        "bin",
        "obj",
        "*.user",
        "secrets.json",
        "*.db",
        "*.db-shm",
        "*.db-wal",
        "*.mdf",
        "*.ldf",
        "*.pdf",
        "*.csv",
        "*.tsv",
        "*.tab",
        "*.xlsx",
        "*.xls",
        "*upload*",
        "*import*source*"
    )

    foreach ($pattern in $patterns) {
        Get-ChildItem -LiteralPath $Root -Recurse -Force -Filter $pattern -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -ne $Root } |
            Remove-Item -Recurse -Force
    }
}

function Assert-NoSensitivePackageContent {
    param([string]$Root)

    $blockedPatterns = @(
        "BEGIN PRIVATE KEY",
        "AccountKey=",
        "SharedAccessKey=",
        "xox[baprs]-",
        "AKIA[0-9A-Z]{16}",
        "sk_live_",
        "pk_live_",
        "api[_-]?key\s*[:=]\s*[""'][^""']{12,}",
        "password\s*[:=]\s*[""'][^""']{8,}",
        "\b\d{3}-\d{2}-\d{4}\b",
        "\b\d{2}-\d{7}\b"
    )

    $allowedFiles = @(
        "README-TESTERS.md",
        "MANUAL-TEST-PACKET.md",
        "TESTER-FEEDBACK-FORM.md",
        "package-manifest.md",
        "appsettings.json",
        "appsettings.Development.json",
        "appsettings.Staging.json"
    )

    $textFiles = Get-ChildItem -LiteralPath $Root -Recurse -File -Force |
        Where-Object {
            $_.Length -lt 2MB -and
            $_.Extension -notin @(".dll", ".exe", ".pdb", ".png", ".ico", ".dat", ".gz", ".br")
        }

    foreach ($file in $textFiles) {
        $content = Get-Content -LiteralPath $file.FullName -Raw -ErrorAction SilentlyContinue
        if ([string]::IsNullOrWhiteSpace($content)) {
            continue
        }

        $contentToScan = $content.
            Replace("00-0000000", "DEMO-FEIN").
            Replace("000-00-0000", "DEMO-SSN").
            Replace("123-45-6789", "DEMO-SSN").
            Replace("123-45-1234", "DEMO-SSN").
            Replace("12-3456789", "DEMO-EIN")

        foreach ($pattern in $blockedPatterns) {
            if ($contentToScan -match $pattern) {
                $fileName = Split-Path -Leaf $file.FullName
                $isAllowedPlaceholderFile = $allowedFiles -contains $fileName -and
                    ($content -match "ENTER_|PLACEHOLDER|Tester package only|Do not enter real")

                if (-not $isAllowedPlaceholderFile) {
                    throw "Potential sensitive value matched pattern '$pattern' in packaged file '$($file.FullName)'. Package was not created."
                }
            }
        }
    }
}

Invoke-Step "Clean previous publish and package output" {
    New-Item -ItemType Directory -Path $DistRoot -Force | Out-Null
    if (Test-Path -LiteralPath $PublishOutput) {
        Remove-Item -LiteralPath $PublishOutput -Recurse -Force
    }
    if (Test-Path -LiteralPath $PackageRoot) {
        Remove-Item -LiteralPath $PackageRoot -Recurse -Force
    }
    if (Test-Path -LiteralPath $ZipPath) {
        Remove-Item -LiteralPath $ZipPath -Force
    }
}

Invoke-Step "Restore" {
    dotnet restore $SolutionPath
}

Invoke-Step "Build" {
    dotnet build $SolutionPath --configuration $Configuration --no-restore
}

Invoke-Step "Test" {
    dotnet test (Join-Path $RepoRoot "tests\ClearPathPayroll.Tests\ClearPathPayroll.Tests.csproj") --configuration $Configuration --no-build
}

Invoke-Step "Publish" {
    dotnet publish $ProjectPath --configuration $Configuration --output $PublishOutput --no-build
}

Invoke-Step "Create tester package folder" {
    New-Item -ItemType Directory -Path $PackageRoot -Force | Out-Null
    Copy-Item -Path (Join-Path $PublishOutput "*") -Destination $PackageRoot -Recurse -Force

    $DocsDestination = Join-Path $PackageRoot "docs"
    if (Test-Path -LiteralPath (Join-Path $RepoRoot "docs")) {
        Copy-Item -LiteralPath (Join-Path $RepoRoot "docs") -Destination $DocsDestination -Recurse -Force
    }

    Copy-IfExists (Join-Path $RepoRoot "README-TESTERS.md") (Join-Path $PackageRoot "README-TESTERS.md")
    Copy-IfExists (Join-Path $RepoRoot "START-ClearPathPayroll.bat") (Join-Path $PackageRoot "START-ClearPathPayroll.bat")
    Copy-IfExists (Join-Path $RepoRoot "RESET-DEMO-DATA.bat") (Join-Path $PackageRoot "RESET-DEMO-DATA.bat")
    Copy-IfExists (Join-Path $RepoRoot "TROUBLESHOOTING-TESTER-STARTUP.md") (Join-Path $PackageRoot "TROUBLESHOOTING-TESTER-STARTUP.md")
    Copy-IfExists (Join-Path $RepoRoot "MANUAL-TEST-PACKET.md") (Join-Path $PackageRoot "MANUAL-TEST-PACKET.md")
    Copy-IfExists (Join-Path $RepoRoot "TESTER-FEEDBACK-FORM.md") (Join-Path $PackageRoot "TESTER-FEEDBACK-FORM.md")

    $ManifestSource = Join-Path $ScriptRoot "package-manifest.md"
    if (Test-Path -LiteralPath $ManifestSource) {
        $manifest = Get-Content -LiteralPath $ManifestSource -Raw
        $manifest = $manifest.Replace("{{BuildDateUtc}}", $BuildDateUtc)
        Set-Content -LiteralPath (Join-Path $PackageRoot "package-manifest.md") -Value $manifest -Encoding UTF8
    }

    Remove-PackageArtifactsByPattern -Root $PackageRoot
    Assert-NoSensitivePackageContent -Root $PackageRoot
}

Invoke-Step "Create zip" {
    Compress-Archive -Path (Join-Path $PackageRoot "*") -DestinationPath $ZipPath -Force
}

Write-Host ""
Write-Host "Tester package created:"
Write-Host "Folder: $PackageRoot"
Write-Host "Zip:    $ZipPath"
Write-Host ""
Write-Host "Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data."
