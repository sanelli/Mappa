param(
    [string]$SummaryXmlPath = "./.mappa-tests-and-coverage/Summary.xml",
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$BenchmarkSummaryPath = "./.mappa-benchmark/Benchmark.Summary.md",
    [double]$WarningDropPercent = 1.0,
    [double]$FailDropPercent = 5.0,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$CommentMarker = "<!-- mappa-pr-coverage -->"

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