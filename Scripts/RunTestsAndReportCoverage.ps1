param([switch]$AlwaysSuccess);

[double]$Threshold = 80.00
# Issue #238: maxima tuned above cleaned CC 15 / CRAP 30 DoD (ReportGenerator display defaults).
[double]$MaximumCyclomaticComplexity = 20
[double]$MaximumCrapScore = 35
$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"
$MappaGeneratorTestsDumpPath = ".mappa-generator-tests-dump"
$SummaryXmlPath = Join-Path $MappaTestsAndCoveragePath "Summary.xml"
$SummaryMarkdownPath = Join-Path $MappaTestsAndCoveragePath "Summary.md"

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

function Write-CoverageArtifactsAndReport
{
    param(
        [double]$LineCoverage,
        [double]$BranchCoverage,
        [double]$MethodCoverage
    )

    Get-ShieldsIoJson -Name "Line Coverage" -Value $LineCoverage | Out-File "./$MappaTestsAndCoveragePath/line-coverage-badge.json"
    Get-ShieldsIoJson -Name "Branch Coverage" -Value $BranchCoverage | Out-File "./$MappaTestsAndCoveragePath/branch-coverage-badge.json"
    Get-ShieldsIoJson -Name "Method Coverage" -Value $MethodCoverage | Out-File "./$MappaTestsAndCoveragePath/method-coverage-badge.json"

    [xml]$currentVersionFile = Get-Content ./MappaVersion.targets
    $currentMappaVersion = $currentVersionFile.Project.PropertyGroup.MappaVersion
    $timestamp = Get-Date -Format "yyyy/MM/dd HH:mm:ss"
    "| $timestamp | $currentMappaVersion | LINE | $LineCoverage |`n| $timestamp | $currentMappaVersion | BRANCH | $BranchCoverage |`n| $timestamp | $currentMappaVersion | METHOD | $MethodCoverage |" | Out-File "./$MappaTestsAndCoveragePath/history-table.md"

    $coverageBelowThreshold = ($LineCoverage -lt $Threshold) -or ($BranchCoverage -lt $Threshold) -or ($MethodCoverage -lt $Threshold)
    if ($coverageBelowThreshold)
    {
        Write-Host "Poor coverage:" -ForegroundColor Red
        Write-Host " - Line Coverage: $LineCoverage" -ForegroundColor Red
        Write-Host " - Branch Coverage: $BranchCoverage" -ForegroundColor Red
        Write-Host " - Method Coverage: $MethodCoverage" -ForegroundColor Red
    }
    else
    {
        Write-Host "Coverage:" -ForegroundColor Green
        Write-Host " - Line Coverage: $LineCoverage %" -ForegroundColor Green
        Write-Host " - Branch Coverage: $BranchCoverage %" -ForegroundColor Green
        Write-Host " - Method Coverage: $MethodCoverage %" -ForegroundColor Green
    }

    return $coverageBelowThreshold
}

function Get-RiskHotspotViolationsFromSummaryMarkdown
{
    param(
        [string]$SummaryMarkdownPath,
        [double]$MaximumCyclomaticComplexity,
        [double]$MaximumCrapScore
    )

    if (-not (Test-Path $SummaryMarkdownPath))
    {
        throw "Summary markdown not found at '$SummaryMarkdownPath'."
    }

    $markdown = Get-Content -Raw $SummaryMarkdownPath
    if ($markdown -notmatch '(?s)# Risk Hotspots\s*(.*?)(?:\r?\n# Coverage|\z)')
    {
        return @()
    }

    $riskSection = $Matches[1]
    # Rows may be one-per-line or concatenated with "||" between rows (ReportGenerator MarkdownSummary).
    $rowMatches = [regex]::Matches(
        $riskSection,
        '\|\s*(?<Assembly>[^|]+?)\s*\|\s*(?<Class>[^|]+?)\s*\|\s*(?<Method>[^|]+?)\s*\|\s*(?<Crap>\d+(?:\.\d+)?)\s*\|\s*(?<Cc>\d+(?:\.\d+)?)\s*\|')

    $violations = @()
    foreach ($rowMatch in $rowMatches)
    {
        $assembly = $rowMatch.Groups['Assembly'].Value.Trim()
        if ($assembly -eq '**Assembly**' -or $assembly -eq ':---')
        {
            continue
        }

        $crap = [double]::Parse($rowMatch.Groups['Crap'].Value, [System.Globalization.CultureInfo]::InvariantCulture)
        $cc = [double]::Parse($rowMatch.Groups['Cc'].Value, [System.Globalization.CultureInfo]::InvariantCulture)
        if ($crap -le $MaximumCrapScore -and $cc -le $MaximumCyclomaticComplexity)
        {
            continue
        }

        $violations += [pscustomobject]@{
            Assembly = $assembly
            Class = $rowMatch.Groups['Class'].Value.Trim()
            Method = $rowMatch.Groups['Method'].Value.Trim()
            CrapScore = $crap
            CyclomaticComplexity = $cc
        }
    }

    return $violations
}

if (Test-Path $MappaTestsAndCoveragePath)
{
    Remove-Item -Recurse -Force $MappaTestsAndCoveragePath
}

if (Test-Path $MappaGeneratorTestsDumpPath)
{
    Remove-Item -Recurse -Force $MappaGeneratorTestsDumpPath
}
New-Item -ItemType Directory -Name $MappaGeneratorTestsDumpPath > $null

dotnet publish -c Release --self-contained ./Mappa.Samples.Aot/
if (-not $?)
{
    Write-Host "Cannot generate native code" -ForegroundColor Red
    Exit 1
}

$publishRoot = Join-Path (Resolve-Path "./Mappa.Samples.Aot/bin/Release/net10.0").Path "*/publish"
$publishDir = Get-Item $publishRoot | Select-Object -First 1
$exe = Get-ChildItem $publishDir.FullName -File |
    Where-Object { $_.Name -like "Mappa.Samples.Aot*" -and $_.Extension -ne ".pdb" } |
    Select-Object -First 1
& $exe.FullName *>$null
if (-not $?)
{
    Write-Host "AOT executable failed" -ForegroundColor Red
    Exit 1
}

New-Item -ItemType Directory -Name $MappaTestsAndCoveragePath > $null
dotnet test -c Release --coverlet --coverlet-output-format cobertura --coverlet-exclude "[Moq]*" --report-xunit-html --report-xunit --report-xunit-filename mappa.test-results.xml --results-directory "$MappaTestsAndCoveragePath"
if (-not $?)
{
    Write-Host "Test failed" -ForegroundColor Red
    Exit 1
}

dotnet tool restore
dotnet reportgenerator -reports:"$MappaTestsAndCoveragePath/coverage.cobertura*.xml" -targetdir:"$MappaTestsAndCoveragePath" -title:"Mappa" -reporttypes:"Html;MarkdownSummary;XmlSummary" -filefilters:"-*.g.cs" -assemblyfilters:"-Mappa.Samples;-Moq" -classfilters:"-Mappa.Generator.Exceptions.MappaGeneratorException;-Mappa.Generator.Diagnostics.Debug.MappaDebug;-Mappa.Generator.Diagnostics.DiagnosticsResources;-Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors;-Mappa.Generator.Extensions.FakeType"
if (-not $?)
{
    Write-Host "Report failed" -ForegroundColor Red
    Exit 1
}

if (-not (Test-Path $SummaryXmlPath))
{
    Write-Host "Report failed: Summary.xml was not generated." -ForegroundColor Red
    Exit 1
}

[xml]$Report = Get-Content -Raw $SummaryXmlPath
[double]$LineCoverage = [double]::Parse($Report.CoverageReport.Summary.LineCoverage)
[double]$Branchcoverage = [double]::Parse($Report.CoverageReport.Summary.Branchcoverage)
[double]$Methodcoverage = [double]::Parse($Report.CoverageReport.Summary.Methodcoverage)

$coverageBelowThreshold = Write-CoverageArtifactsAndReport `
    -LineCoverage $LineCoverage `
    -BranchCoverage $Branchcoverage `
    -MethodCoverage $Methodcoverage

# Issue #238: parse Risk Hotspots from Summary.md (avoids ReportGenerator PRO maximum-threshold settings).
$riskHotspotViolations = Get-RiskHotspotViolationsFromSummaryMarkdown `
    -SummaryMarkdownPath $SummaryMarkdownPath `
    -MaximumCyclomaticComplexity $MaximumCyclomaticComplexity `
    -MaximumCrapScore $MaximumCrapScore

if ($riskHotspotViolations.Count -gt 0)
{
    Write-Host "Risk Hotspot maxima exceeded (cyclomatic complexity > $MaximumCyclomaticComplexity and/or CRAP score > $MaximumCrapScore):" -ForegroundColor Red
    foreach ($violation in ($riskHotspotViolations | Sort-Object -Property CrapScore -Descending))
    {
        Write-Host (" - CRAP={0} CC={1} | {2}.{3}" -f $violation.CrapScore, $violation.CyclomaticComplexity, $violation.Class, $violation.Method) -ForegroundColor Red
    }
    Write-Host "See Risk Hotspots in ./$MappaTestsAndCoveragePath/Summary.md and ./$MappaTestsAndCoveragePath/index.html." -ForegroundColor Red
    Exit 1
}

if ($coverageBelowThreshold)
{
    if ($AlwaysSuccess)
    {
        Exit 0
    }
    else
    {
        Exit 1
    }
}

Exit 0