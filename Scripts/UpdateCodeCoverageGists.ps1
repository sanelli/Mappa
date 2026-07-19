param([switch]$DryRun)

$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"
. "$PSScriptRoot/CodeCoverageHistorySvg.ps1"

Get-Content -Raw "./$MappaTestsAndCoveragePath/line-coverage-badge.json"
if (-not $DryRun)
{
    gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-BADGE-LINE-COVERAGE.json" "./$MappaTestsAndCoveragePath/line-coverage-badge.json"
}
Get-Content -Raw "./$MappaTestsAndCoveragePath/branch-coverage-badge.json"
if (-not $DryRun)
{
    gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-BADGE-BRANCH-COVERAGE.json" "./$MappaTestsAndCoveragePath/branch-coverage-badge.json"
}

Get-Content -Raw "./$MappaTestsAndCoveragePath/method-coverage-badge.json"
if (-not $DryRun)
{
    gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-BADGE-METHOD-COVERAGE.json" "./$MappaTestsAndCoveragePath/method-coverage-badge.json"
}

if (-not $DryRun)
{
    if (Test-Path -Type Leaf "./$MappaTestsAndCoveragePath/full-history-table.md")
    {
        Remove-Item "./$MappaTestsAndCoveragePath/full-history-table.md"
    }
}

if (-not $DryRun)
{
    $currentHistory = $( gh gist view "7f4a85bc809328b4821b03125f9190cb" --raw -f "MAPPA-CODE-COVERAGE-HISTORY.MD" )
}
else
{
    $currentHistory = $( Get-Content -Raw "./$MappaTestsAndCoveragePath/full-history-table.md" )
}

$currentHistory = $currentHistory.Trim()
$currentHistory | Out-File "./$MappaTestsAndCoveragePath/full-history-table.md"
Get-Content -Raw "./$MappaTestsAndCoveragePath/history-table.md" | Out-File -Append -NoNewline "./$MappaTestsAndCoveragePath/full-history-table.md"
Get-Content -Raw "./$MappaTestsAndCoveragePath/full-history-table.md"

if (-not $DryRun)
{
    gh gist edit "7f4a85bc809328b4821b03125f9190cb" -a "MAPPA-CODE-COVERAGE-HISTORY.MD" "./$MappaTestsAndCoveragePath/full-history-table.md"
}

New-CodeCoverageHistorySvg `
    -HistoryMarkdownPath "./$MappaTestsAndCoveragePath/full-history-table.md" `
    -OutputPath "./$MappaTestsAndCoveragePath/history.svg"
Get-Content -Raw "./$MappaTestsAndCoveragePath/history.svg"

if (-not $DryRun)
{
    gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-CODE-COVERAGE-HISTORY.svg" "./$MappaTestsAndCoveragePath/history.svg"
}
