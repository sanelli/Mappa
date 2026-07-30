param(
    [string]$SummaryXmlPath = "./.mappa-tests-and-coverage/Summary.xml",
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$BenchmarkSummaryPath = "./.mappa-benchmark/Benchmark.Summary.md",
    [string]$BenchmarkHistoryTablePath = "./.mappa-benchmark/history-table.md",
    [string]$BenchmarkHistoryFileName = "MAPPA-BENCHMARK-HISTORY.md",
    [double]$WarningDropPercent = 1.0,
    [double]$FailDropPercent = 5.0,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
. "$PSScriptRoot/BenchmarkChartsSvg.ps1"
$CommentMarker = "<!-- mappa-pr-coverage -->"
# Same subset as MAPPA-BENCHMARK-TIME/MEMORY.svg and -ChartBenchmarksOnly.
$BenchmarkTrendNames = $script:BenchmarkChartNames

function Get-TrendSymbol
{
    param(
        [double]$Current,
        [double]$Baseline
    )

    if ($Current -gt $Baseline)
    {
        return "▲"
    }

    if ($Current -lt $Baseline)
    {
        return "▼"
    }

    return "="
}

function Get-BenchmarkTrendSymbol
{
    param(
        [double]$Current,
        [double]$Baseline
    )

    # Lower time / allocation is better.
    if ($Current -lt $Baseline)
    {
        return "▲"
    }

    if ($Current -gt $Baseline)
    {
        return "▼"
    }

    return "="
}

function Get-CoverageFromSummary
{
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path))
    {
        throw "Coverage summary not found: $Path. Run Scripts/RunTestsAndReportCoverage.ps1 first."
    }

    [xml]$report = Get-Content -Raw -LiteralPath $Path
    return [pscustomobject]@{
        Line = [double]::Parse($report.CoverageReport.Summary.Linecoverage, [System.Globalization.CultureInfo]::InvariantCulture)
        Branch = [double]::Parse($report.CoverageReport.Summary.Branchcoverage, [System.Globalization.CultureInfo]::InvariantCulture)
        Method = [double]::Parse($report.CoverageReport.Summary.Methodcoverage, [System.Globalization.CultureInfo]::InvariantCulture)
    }
}

function Get-BaselineCoverageFromGist
{
    param([string]$GistId)

    # Same retrieval path as Scripts/UpdateCodeCoverageGists.ps1.
    # Prefer GH_PAT for gist API access; GITHUB_TOKEN cannot read private/org-restricted gists.
    $previousToken = $env:GH_TOKEN
    try
    {
        if (-not [string]::IsNullOrWhiteSpace($env:GH_PAT))
        {
            $env:GH_TOKEN = $env:GH_PAT
        }

        $currentHistory = $( gh gist view $GistId --raw -f "MAPPA-CODE-COVERAGE-HISTORY.MD" )
        if (-not $?)
        {
            throw "Failed to download coverage history from gist $GistId."
        }
    }
    finally
    {
        $env:GH_TOKEN = $previousToken
    }

    $line = $null
    $branch = $null
    $method = $null
    foreach ($historyLine in ($currentHistory -split "`r?`n"))
    {
        if ($historyLine -notmatch '^\|')
        {
            continue
        }

        $cells = @($historyLine.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" })
        if ($cells.Count -lt 4)
        {
            continue
        }

        $type = $cells[2]
        $percentageText = $cells[3]
        if ($type -eq "Type" -or $percentageText -eq "Percentage" -or $percentageText -match '^-+$')
        {
            continue
        }

        $percentage = [double]::Parse($percentageText, [System.Globalization.CultureInfo]::InvariantCulture)
        switch ($type)
        {
            "LINE" { $line = $percentage }
            "BRANCH" { $branch = $percentage }
            "METHOD" { $method = $percentage }
        }
    }

    if (($null -eq $line) -or ($null -eq $branch) -or ($null -eq $method))
    {
        throw "Could not parse baseline LINE/BRANCH/METHOD coverage from gist history."
    }

    return [pscustomobject]@{
        Line = $line
        Branch = $branch
        Method = $method
    }
}

function Test-BenchmarkHistoryHeaderOrSeparator
{
    param([string[]]$Cells)

    if ($Cells.Count -lt 5)
    {
        return $true
    }

    if ($Cells[0] -eq "Timestamp" -or $Cells[3] -eq "Measure" -or $Cells[4] -eq "Value")
    {
        return $true
    }

    foreach ($cell in $Cells)
    {
        if ($cell -notmatch '^-+$')
        {
            return $false
        }
    }

    return $true
}

function ConvertTo-BenchmarkHistoryRows
{
    param([string]$Markdown)

    $rows = @{}
    if ([string]::IsNullOrWhiteSpace($Markdown))
    {
        return $rows
    }

    foreach ($historyLine in ($Markdown -split "`r?`n"))
    {
        if ($historyLine -notmatch '^\|')
        {
            continue
        }

        $cells = @($historyLine.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" })
        if (Test-BenchmarkHistoryHeaderOrSeparator -Cells $cells)
        {
            continue
        }

        if ($cells.Count -lt 5)
        {
            continue
        }

        $benchmark = $cells[2]
        $measure = $cells[3]
        $valueText = $cells[4]
        $value = $null
        try
        {
            $value = [double]::Parse($valueText, [System.Globalization.CultureInfo]::InvariantCulture)
        }
        catch
        {
            continue
        }

        $key = "$benchmark|$measure"
        $rows[$key] = [pscustomobject]@{
            Benchmark = $benchmark
            Measure = $measure
            Value = $value
        }
    }

    return $rows
}

function Get-BaselineBenchmarkHistoryFromGist
{
    param(
        [string]$GistId,
        [string]$HistoryFileName
    )

    $previousToken = $env:GH_TOKEN
    $previousEap = $ErrorActionPreference
    try
    {
        if (-not [string]::IsNullOrWhiteSpace($env:GH_PAT))
        {
            $env:GH_TOKEN = $env:GH_PAT
        }

        $ErrorActionPreference = "Continue"
        $raw = & gh gist view $GistId --raw -f $HistoryFileName 2>&1
        $exitCode = $LASTEXITCODE
        if ($exitCode -ne 0)
        {
            Write-Host "Benchmark history file '$HistoryFileName' was not found in gist $GistId (exit $exitCode); treating baseline as empty."
            return @{}
        }

        $content = if ($null -eq $raw) { "" } elseif ($raw -is [array]) { ($raw | ForEach-Object { "$_" }) -join "`n" } else { [string]$raw }
        if ([string]::IsNullOrWhiteSpace($content))
        {
            Write-Host "Benchmark history file '$HistoryFileName' in gist $GistId is empty; treating baseline as empty."
            return @{}
        }

        return ConvertTo-BenchmarkHistoryRows -Markdown $content
    }
    finally
    {
        $ErrorActionPreference = $previousEap
        $env:GH_TOKEN = $previousToken
    }
}

function Format-BenchmarkMetric
{
    param([double]$Value)

    return ("{0:0.###}" -f $Value)
}

function Format-BenchmarkTrendRow
{
    param(
        [string]$Benchmark,
        [string]$Measure,
        $BaselineValue,
        $CurrentValue
    )

    $baselineText = if ($null -eq $BaselineValue) { "n/a" } else { Format-BenchmarkMetric -Value ([double]$BaselineValue) }
    $currentText = if ($null -eq $CurrentValue) { "n/a" } else { Format-BenchmarkMetric -Value ([double]$CurrentValue) }

    if (($null -eq $BaselineValue) -or ($null -eq $CurrentValue))
    {
        return "| $Benchmark | $Measure | $baselineText | $currentText | n/a | = |"
    }

    $baseline = [Math]::Round([double]$BaselineValue, 3)
    $current = [Math]::Round([double]$CurrentValue, 3)
    $delta = [Math]::Round($current - $baseline, 3)
    $deltaText = if ($delta -gt 0) { "+{0:0.###}" -f $delta } elseif ($delta -lt 0) { "{0:0.###}" -f $delta } else { "0" }
    $trend = Get-BenchmarkTrendSymbol -Current $current -Baseline $baseline
    return "| $Benchmark | $Measure | $baselineText | $currentText | $deltaText | $trend |"
}

function Format-CoverageRow
{
    param(
        [string]$Name,
        [double]$Baseline,
        [double]$Current
    )

    $delta = [Math]::Round($Current - $Baseline, 1)
    $deltaText = if ($delta -gt 0) { "+{0:0.#}" -f $delta } elseif ($delta -lt 0) { "{0:0.#}" -f $delta } else { "0" }
    $trend = Get-TrendSymbol -Current $Current -Baseline $Baseline
    $baselineText = "{0:0.#}%" -f $Baseline
    $currentText = "{0:0.#}%" -f $Current
    return "| $Name | $baselineText | $currentText | $deltaText | $trend |"
}

function Get-MetricDrop
{
    param(
        [double]$Baseline,
        [double]$Current
    )

    return [Math]::Round($Baseline - $Current, 1)
}

function Publish-PullRequestCoverageComment
{
    param(
        [string]$Repository,
        [int]$PullRequestNumber,
        [string]$Body
    )

    $bodyFile = Join-Path ([System.IO.Path]::GetTempPath()) ("mappa-pr-coverage-" + [guid]::NewGuid().ToString("N") + ".md")
    try
    {
        Set-Content -LiteralPath $bodyFile -Value $Body -Encoding utf8

        # Uses github.token (GH_TOKEN). gh pr comment --edit-last updates the bot's previous
        # coverage comment, or --create-if-none posts a new one on the first run.
        # See https://cli.github.com/manual/gh_pr_comment
        gh pr comment $PullRequestNumber `
            --repo $Repository `
            --body-file $bodyFile `
            --edit-last `
            --create-if-none | Out-Null
        if (-not $?)
        {
            throw "Failed to create or update coverage comment on $Repository#$PullRequestNumber."
        }

        Write-Host "Published coverage comment on PR #$PullRequestNumber."
    }
    finally
    {
        if (Test-Path -LiteralPath $bodyFile)
        {
            Remove-Item -LiteralPath $bodyFile -Force
        }
    }
}

$current = Get-CoverageFromSummary -Path $SummaryXmlPath
$baseline = Get-BaselineCoverageFromGist -GistId $GistId

$lineDrop = Get-MetricDrop -Baseline $baseline.Line -Current $current.Line
$branchDrop = Get-MetricDrop -Baseline $baseline.Branch -Current $current.Branch
$methodDrop = Get-MetricDrop -Baseline $baseline.Method -Current $current.Method

$warningMetrics = New-Object System.Collections.Generic.List[string]
$failMetrics = New-Object System.Collections.Generic.List[string]

foreach ($metric in @(
        @{ Name = "Line"; Drop = $lineDrop },
        @{ Name = "Branch"; Drop = $branchDrop },
        @{ Name = "Method"; Drop = $methodDrop }
    ))
{
    if ($metric.Drop -gt $FailDropPercent)
    {
        $failMetrics.Add(("{0} dropped by {1:0.#} percentage points (limit {2:0.#})." -f $metric.Name, $metric.Drop, $FailDropPercent))
    }
    elseif ($metric.Drop -gt $WarningDropPercent)
    {
        $warningMetrics.Add(("{0} dropped by {1:0.#} percentage points (warning above {2:0.#})." -f $metric.Name, $metric.Drop, $WarningDropPercent))
    }
}

$bodyBuilder = New-Object System.Text.StringBuilder
[void]$bodyBuilder.AppendLine($CommentMarker)
[void]$bodyBuilder.AppendLine("## Code coverage")
[void]$bodyBuilder.AppendLine()
[void]$bodyBuilder.AppendLine("Compared against the latest published coverage history from the [code coverage gist](https://gist.github.com/sanelli/$GistId).")
[void]$bodyBuilder.AppendLine()
[void]$bodyBuilder.AppendLine("| Metric | Baseline | PR | Δ | Trend |")
[void]$bodyBuilder.AppendLine("| ------ | -------- | -- | - | ----- |")
[void]$bodyBuilder.AppendLine((Format-CoverageRow -Name "Line" -Baseline $baseline.Line -Current $current.Line))
[void]$bodyBuilder.AppendLine((Format-CoverageRow -Name "Branch" -Baseline $baseline.Branch -Current $current.Branch))
[void]$bodyBuilder.AppendLine((Format-CoverageRow -Name "Method" -Baseline $baseline.Method -Current $current.Method))
[void]$bodyBuilder.AppendLine()

if ($warningMetrics.Count -gt 0)
{
    [void]$bodyBuilder.AppendLine("> [!WARNING]")
    [void]$bodyBuilder.AppendLine("> Coverage dropped by more than $WarningDropPercent percentage point(s):")
    foreach ($warning in $warningMetrics)
    {
        [void]$bodyBuilder.AppendLine("> - $warning")
    }

    [void]$bodyBuilder.AppendLine()
}

if ($failMetrics.Count -gt 0)
{
    [void]$bodyBuilder.AppendLine("> [!CAUTION]")
    [void]$bodyBuilder.AppendLine("> Coverage dropped by more than $FailDropPercent percentage point(s). This pull request check will fail.")
    foreach ($failure in $failMetrics)
    {
        [void]$bodyBuilder.AppendLine("> - $failure")
    }

    [void]$bodyBuilder.AppendLine()
}

if (($warningMetrics.Count -eq 0) -and ($failMetrics.Count -eq 0))
{
    [void]$bodyBuilder.AppendLine("No coverage metric dropped by more than $WarningDropPercent percentage point(s) versus baseline.")
}

[void]$bodyBuilder.AppendLine()
[void]$bodyBuilder.AppendLine("## Benchmarks")
[void]$bodyBuilder.AppendLine()

if (Test-Path -LiteralPath $BenchmarkSummaryPath)
{
    $summaryMarkdown = (Get-Content -Raw -LiteralPath $BenchmarkSummaryPath).Trim()
    # Drop the leading H1 so the PR section heading remains ## Benchmarks.
    $summaryMarkdown = $summaryMarkdown -replace '(?s)^#\s+Benchmark summary\s*', ""
    [void]$bodyBuilder.AppendLine($summaryMarkdown.Trim())
}
else
{
    [void]$bodyBuilder.AppendLine("Benchmark summary not found. Run Scripts/RunBenchmarks.ps1 before commenting.")
}

[void]$bodyBuilder.AppendLine()
[void]$bodyBuilder.AppendLine("## Benchmark trends (Mappa)")
[void]$bodyBuilder.AppendLine()
[void]$bodyBuilder.AppendLine("Lower is better. Compared against the latest published Mappa-only rows from [`MAPPA-BENCHMARK-HISTORY.md`](https://gist.github.com/sanelli/$GistId).")
[void]$bodyBuilder.AppendLine()
[void]$bodyBuilder.AppendLine("| Benchmark | Measure | Baseline | PR | Δ | Trend |")
[void]$bodyBuilder.AppendLine("| --------- | ------- | -------- | -- | - | ----- |")

$benchmarkBaselineRows = Get-BaselineBenchmarkHistoryFromGist -GistId $GistId -HistoryFileName $BenchmarkHistoryFileName
$benchmarkCurrentRows = @{}
if (Test-Path -LiteralPath $BenchmarkHistoryTablePath)
{
    $benchmarkCurrentRows = ConvertTo-BenchmarkHistoryRows -Markdown (Get-Content -Raw -LiteralPath $BenchmarkHistoryTablePath)
}
else
{
    Write-Host "Benchmark history table not found at $BenchmarkHistoryTablePath; PR benchmark values will be n/a."
}

foreach ($benchmarkName in $BenchmarkTrendNames)
{
    foreach ($measure in @("TIME_NS", "ALLOC_B"))
    {
        $key = "$benchmarkName|$measure"
        $baselineEntry = $benchmarkBaselineRows[$key]
        $currentEntry = $benchmarkCurrentRows[$key]
        $baselineValue = if ($null -eq $baselineEntry) { $null } else { $baselineEntry.Value }
        $currentValue = if ($null -eq $currentEntry) { $null } else { $currentEntry.Value }
        [void]$bodyBuilder.AppendLine((Format-BenchmarkTrendRow -Benchmark $benchmarkName -Measure $measure -BaselineValue $baselineValue -CurrentValue $currentValue))
    }
}

$commentBody = $bodyBuilder.ToString().TrimEnd() + "`n"

Write-Host "Baseline coverage: line=$($baseline.Line)% branch=$($baseline.Branch)% method=$($baseline.Method)%"
Write-Host "PR coverage: line=$($current.Line)% branch=$($current.Branch)% method=$($current.Method)%"
Write-Host $commentBody

if ($DryRun)
{
    Write-Host "DryRun enabled; skipping pull request comment publish."
}
else
{
    $repository = $env:GITHUB_REPOSITORY
    if ([string]::IsNullOrWhiteSpace($repository))
    {
        throw "GITHUB_REPOSITORY is not set."
    }

    $pullRequestNumberText = $env:PR_NUMBER
    if ([string]::IsNullOrWhiteSpace($pullRequestNumberText))
    {
        throw "PR_NUMBER is not set."
    }

    $pullRequestNumber = [int]$pullRequestNumberText
    Publish-PullRequestCoverageComment `
        -Repository $repository `
        -PullRequestNumber $pullRequestNumber `
        -Body $commentBody
}

if ($failMetrics.Count -gt 0)
{
    Write-Host "Coverage regression exceeds fail threshold ($FailDropPercent%)." -ForegroundColor Red
    exit 1
}

if ($warningMetrics.Count -gt 0)
{
    Write-Host "Coverage regression exceeds warning threshold ($WarningDropPercent%)." -ForegroundColor Yellow
}

exit 0