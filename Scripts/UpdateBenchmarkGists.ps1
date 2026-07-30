param(
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$HistoryFileName = "MAPPA-BENCHMARK-HISTORY.md",
    [string]$MappaBenchmarkPath = ".mappa-benchmark",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

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

$gistHistory = Get-BenchmarkHistoryFromGist -GistId $GistId -HistoryFileName $HistoryFileName
$fileExists = [bool]$gistHistory.Exists
$existingContent = $gistHistory.Content

if ([string]::IsNullOrWhiteSpace($existingContent))
{
    $fullHistory = $HistoryTableHeader.TrimEnd() + "`n" + $newRows + "`n"
}
else
{
    $fullHistory = $existingContent.TrimEnd() + "`n" + $newRows + "`n"
}

$fullHistoryPath = Join-Path $MappaBenchmarkPath "full-history-table.md"
[System.IO.File]::WriteAllText($fullHistoryPath, $fullHistory)

Write-Host "Wrote $fullHistoryPath"
Get-Content -Raw -LiteralPath $fullHistoryPath | Write-Host

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
        gh gist edit $GistId -f $HistoryFileName $fullHistoryPath
    }
    else
    {
        gh gist edit $GistId -a $HistoryFileName $fullHistoryPath
    }

    if (-not $?)
    {
        throw "Failed to update gist $GistId file $HistoryFileName."
    }

    Write-Host "Updated gist $GistId ($HistoryFileName)."
}
finally
{
    $env:GH_TOKEN = $previousToken
}

exit 0