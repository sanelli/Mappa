param(
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$HistoryFileName = "MAPPA-BENCHMARK-HISTORY.md",
    [string]$MappaBenchmarkPath = ".mappa-benchmark",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

. "$PSScriptRoot/GistHelpers.ps1"

$HistoryTableHeader = @"
| Timestamp | Version | Benchmark | Measure | Value |
| --------- | ------- | ---- | ---------- | ------- |
"@

function Get-BenchmarkHistoryFromGist
{
    param(
        [string]$GistId,
        [string]$HistoryFileName
    )

    # Prefer GH_PAT for gist API access; GITHUB_TOKEN cannot read private/org-restricted gists.
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
            # File missing from gist (first run) — treat as empty baseline.
            Write-Host "Benchmark history file '$HistoryFileName' was not found in gist $GistId (exit $exitCode); seeding empty history."
            return [pscustomobject]@{
                Exists = $false
                Content = $null
            }
        }

        $content = if ($null -eq $raw) { "" } elseif ($raw -is [array]) { ($raw | ForEach-Object { "$_" }) -join "`n" } else { [string]$raw }
        return [pscustomobject]@{
            Exists = $true
            Content = $content
        }
    }
    finally
    {
        $ErrorActionPreference = $previousEap
        $env:GH_TOKEN = $previousToken
    }
}

function Publish-GistFile
{
    param(
        [string]$GistId,
        [string]$RemoteFileName,
        [string]$LocalPath,
        [ValidateSet("Update", "Add")]
        [string]$Mode = "Update"
    )

    Invoke-GhGistEdit -GistId $GistId -RemoteFileName $RemoteFileName -LocalPath $LocalPath -Mode $Mode
}

$historyRowsPath = Join-Path $MappaBenchmarkPath "history-table.md"
if (-not (Test-Path -LiteralPath $historyRowsPath))
{
    throw "Benchmark history rows not found: $historyRowsPath. Run Scripts/RunBenchmarks.ps1 first."
}

$newRows = (Get-Content -Raw -LiteralPath $historyRowsPath).Trim()
if ([string]::IsNullOrWhiteSpace($newRows))
{
    throw "Benchmark history rows file is empty: $historyRowsPath."
}

if ($DryRun)
{
    $existingContent = $null
    $fullHistoryLocalPath = Join-Path $MappaBenchmarkPath "full-history-table.md"
    if (Test-Path -LiteralPath $fullHistoryLocalPath)
    {
        $existingContent = Get-Content -Raw -LiteralPath $fullHistoryLocalPath
    }

    $fileExists = -not [string]::IsNullOrWhiteSpace($existingContent)
}
else
{
    $gistHistory = Get-BenchmarkHistoryFromGist -GistId $GistId -HistoryFileName $HistoryFileName
    $fileExists = [bool]$gistHistory.Exists
    $existingContent = $gistHistory.Content
}

$fullHistory = Merge-MarkdownHistoryByVersion `
    -ExistingMarkdown $existingContent `
    -NewMarkdown $newRows `
    -DefaultHeader $HistoryTableHeader `
    -MinimumCellCount 5

$fullHistoryPath = Join-Path $MappaBenchmarkPath "full-history-table.md"
[System.IO.File]::WriteAllText($fullHistoryPath, $fullHistory)
Write-Host "Wrote $fullHistoryPath"

$timeSummarySvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-TIME.svg"
$memorySummarySvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-MEMORY.svg"
$timePercentSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-TIME-PERCENTAGES.svg"
$memoryPercentSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg"

if (-not (Test-Path -LiteralPath $timeSummarySvgPath))
{
    $legacyTime = Join-Path $MappaBenchmarkPath "Benchmark.Time.svg"
    if (Test-Path -LiteralPath $legacyTime)
    {
        Copy-Item -LiteralPath $legacyTime -Destination $timeSummarySvgPath -Force
        Write-Host "Copied $legacyTime -> $timeSummarySvgPath"
    }
}

if (-not (Test-Path -LiteralPath $memorySummarySvgPath))
{
    $legacyMemory = Join-Path $MappaBenchmarkPath "Benchmark.Memory.svg"
    if (Test-Path -LiteralPath $legacyMemory)
    {
        Copy-Item -LiteralPath $legacyMemory -Destination $memorySummarySvgPath -Force
        Write-Host "Copied $legacyMemory -> $memorySummarySvgPath"
    }
}

if ($DryRun)
{
    Write-Host "DryRun enabled; skipping gist update."
    exit 0
}

$previousToken = $env:GH_TOKEN
try
{
    if (-not [string]::IsNullOrWhiteSpace($env:GH_PAT))
    {
        $env:GH_TOKEN = $env:GH_PAT
    }

    if ($fileExists)
    {
        Publish-GistFile -GistId $GistId -RemoteFileName $HistoryFileName -LocalPath $fullHistoryPath -Mode Update
    }
    else
    {
        Publish-GistFile -GistId $GistId -RemoteFileName $HistoryFileName -LocalPath $fullHistoryPath -Mode Add
    }

    Publish-GistFile -GistId $GistId -RemoteFileName "MAPPA-BENCHMARK-TIME.svg" -LocalPath $timeSummarySvgPath
    Publish-GistFile -GistId $GistId -RemoteFileName "MAPPA-BENCHMARK-MEMORY.svg" -LocalPath $memorySummarySvgPath
    Publish-GistFile -GistId $GistId -RemoteFileName "MAPPA-BENCHMARK-TIME-PERCENTAGES.svg" -LocalPath $timePercentSvgPath
    Publish-GistFile -GistId $GistId -RemoteFileName "MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg" -LocalPath $memoryPercentSvgPath
}
finally
{
    $env:GH_TOKEN = $previousToken
}

exit 0
