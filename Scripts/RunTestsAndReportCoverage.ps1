param([switch]$AlwasySuccess);

[double]$Threshold = 80.00

function Get-CoverageColor
{
    param([double]$Value)

    if ($Value -lt $Threshold)
    {
        return "red"
    }

    if ($Value -lt 90)
    {
        return "orange"
    }

    return "green"
}

function Get-ShieldsIoJson
{
    param([string]$Name, [double]$Value)
    $color = Get-CoverageColor -Value $Value
    return "{ `"schemaVersion`": 1, `"label`": `"$Name`", `"message`": `"$Value%`", `"color`": `"$color`" }"
}

$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"

if (Test-Path $MappaTestsAndCoveragePath)
{
    Remove-Item -Recurse -Force $MappaTestsAndCoveragePath
}

dotnet publish -c Release --self-contained ./Mappa.Samples.Aot/
if (-not $?)
{
    Write-Host "Cannot generate native code" -ForegroundColor Red
    Exit 1
}

New-Item -ItemType Directory -Name $MappaTestsAndCoveragePath > $null
dotnet test -c Release --collect:"XPlat Code Coverage" --logger "html" --logger "xunit;LogFileName=mappa.{assembly}.test-results.xml" --results-directory "$MappaTestsAndCoveragePath"
if (-not $?)
{
    Write-Host "Test failed" -ForegroundColor Red
    Exit 1
}

dotnet tool restore
dotnet reportgenerator -reports:"$MappaTestsAndCoveragePath/**/*.xml" -targetdir:"$MappaTestsAndCoveragePath" -title:"Mappa" -reporttypes:"Html;MarkdownSummary;XmlSummary" -filefilters:"-*.g.cs" -assemblyfilters:"-Mappa.Samples" -classfilters:"-Mappa.Generator.Exceptions.MappaGeneratorException;-Mappa.Generator.Diagnostics.Debug.MappaDebug;-Mappa.Generator.Diagnostics.DiagnosticsResources;-Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors"
if (-not $?)
{
    Write-Host "Report failed" -ForegroundColor Red
    Exit 1
}

[xml]$Report = Get-Content -Raw "./$MappaTestsAndCoveragePath/Summary.xml"
[double]$LineCoverage = [double]::Parse($Report.CoverageReport.Summary.LineCoverage)
[double]$Branchcoverage = [double]::Parse($Report.CoverageReport.Summary.Branchcoverage)
[double]$Methodcoverage = [double]::Parse($Report.CoverageReport.Summary.Methodcoverage)

Get-ShieldsIoJson -Name "Line Coverage" -Value $LineCoverage | Out-File "./$MappaTestsAndCoveragePath/line-coverage-badge.json"
Get-ShieldsIoJson -Name "Branch Coverage" -Value $Branchcoverage | Out-File "./$MappaTestsAndCoveragePath/branch-coverage-badge.json"
Get-ShieldsIoJson -Name "Method Coverage" -Value $Methodcoverage | Out-File "./$MappaTestsAndCoveragePath/method-coverage-badge.json"

[xml]$currentVersionFile = Get-Content ./MappaVersion.targets
$currentMappaVersion = $currentVersionFile.Project.PropertyGroup.MappaVersion
$timestamp = Get-Date -Format "yyyy/MM/dd hh:mm:ss"
"`n| $timestamp | $currentMappaVersion | LINE | $LineCoverage |`n| $timestamp | $currentMappaVersion | BRANCH | $Branchcoverage |`n| $timestamp | $currentMappaVersion | METHOD | $Methodcoverage |" | Out-File "./$MappaTestsAndCoveragePath/history-table.md"

if (($LineCoverage -lt $Threshold) -or ($Branchcoverage -lt $Threshold) -or ($Methodcoverage -lt $Threshold))
{
    Write-Host "Poor coverage:" -ForegroundColor Red
    Write-Host " - Line Coverage: $LineCoverage" -ForegroundColor Red
    Write-Host " - Branch Coverage: $Branchcoverage" -ForegroundColor Red
    Write-Host " - Method Coverage: $Methodcoverage" -ForegroundColor Red

    if ($AlwasySuccess)
    {
        Exit 0
    }
    else
    {
        Exit 1
    }
}

Write-Host "Coverage:" -ForegroundColor Green
Write-Host " - Line Coverage: $LineCoverage %" -ForegroundColor Green
Write-Host " - Branch Coverage: $Branchcoverage %" -ForegroundColor Green
Write-Host " - Method Coverage: $Methodcoverage %" -ForegroundColor Green

Exit 0