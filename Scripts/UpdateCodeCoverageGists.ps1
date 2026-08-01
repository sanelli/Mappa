param(
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$HistoryFileName = "MAPPA-CODE-COVERAGE-HISTORY.MD",
    [string]$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

. "$PSScriptRoot/GistHelpers.ps1"
. "$PSScriptRoot/CodeCoverageHistorySvg.ps1"

$CoverageHistoryHeader = @"
| Timestamp | Version | Type | Percentage |
| --------- | ------- | ---- | ---------- |
"@

function Get-CoverageHistoryFromGist
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
            Write-Host "Coverage history file '$HistoryFileName' was not found in gist $GistId (exit $exitCode); seeding empty history."
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

$lineBadgePath = Join-Path $MappaTestsAndCoveragePath "line-coverage-badge.json"
$branchBadgePath = Join-Path $MappaTestsAndCoveragePath "branch-coverage-badge.json"
$methodBadgePath = Join-Path $MappaTestsAndCoveragePath "method-coverage-badge.json"
$historyRowsPath = Join-Path $MappaTestsAndCoveragePath "history-table.md"
$fullHistoryPath = Join-Path $MappaTestsAndCoveragePath "full-history-table.md"
$historySvgPath = Join-Path $MappaTestsAndCoveragePath "history.svg"

Get-Content -Raw -LiteralPath $lineBadgePath
Get-Content -Raw -LiteralPath $branchBadgePath
Get-Content -Raw -LiteralPath $methodBadgePath

if (-not (Test-Path -LiteralPath $historyRowsPath))
{
    throw "Coverage history rows not found: $historyRowsPath. Run Scripts/RunTestsAndReportCoverage.ps1 first."
}

$newRows = (Get-Content -Raw -LiteralPath $historyRowsPath).Trim()
if ([string]::IsNullOrWhiteSpace($newRows))
{
    throw "Coverage history rows file is empty: $historyRowsPath."
}

if ($DryRun)
{
    $existingContent = $null
    if (Test-Path -LiteralPath $fullHistoryPath)
    {
        $existingContent = Get-Content -Raw -LiteralPath $fullHistoryPath
    }

    $fileExists = -not [string]::IsNullOrWhiteSpace($existingContent)
}
else
{
    $gistHistory = Get-CoverageHistoryFromGist -GistId $GistId -HistoryFileName $HistoryFileName
    $fileExists = [bool]$gistHistory.Exists
    $existingContent = $gistHistory.Content
}

$fullHistory = Merge-MarkdownHistoryByVersion `
    -ExistingMarkdown $existingContent `
    -NewMarkdown $newRows `
    -DefaultHeader $CoverageHistoryHeader `
    -MinimumCellCount 4

[System.IO.File]::WriteAllText($fullHistoryPath, $fullHistory)
Write-Host "Wrote $fullHistoryPath"
Get-Content -Raw -LiteralPath $fullHistoryPath

New-CodeCoverageHistorySvg `
    -HistoryMarkdownPath $fullHistoryPath `
    -OutputPath $historySvgPath
Get-Content -Raw -LiteralPath $historySvgPath

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

    Invoke-GhGistEdit -GistId $GistId -RemoteFileName "MAPPA-BADGE-LINE-COVERAGE.json" -LocalPath $lineBadgePath -Mode Update
    Invoke-GhGistEdit -GistId $GistId -RemoteFileName "MAPPA-BADGE-BRANCH-COVERAGE.json" -LocalPath $branchBadgePath -Mode Update
    Invoke-GhGistEdit -GistId $GistId -RemoteFileName "MAPPA-BADGE-METHOD-COVERAGE.json" -LocalPath $methodBadgePath -Mode Update

    if ($fileExists)
    {
        Invoke-GhGistEdit -GistId $GistId -RemoteFileName $HistoryFileName -LocalPath $fullHistoryPath -Mode Update
    }
    else
    {
        Invoke-GhGistEdit -GistId $GistId -RemoteFileName $HistoryFileName -LocalPath $fullHistoryPath -Mode Add
    }

    Invoke-GhGistEdit -GistId $GistId -RemoteFileName "MAPPA-CODE-COVERAGE-HISTORY.svg" -LocalPath $historySvgPath -Mode Update
}
finally
{
    $env:GH_TOKEN = $previousToken
}

exit 0
