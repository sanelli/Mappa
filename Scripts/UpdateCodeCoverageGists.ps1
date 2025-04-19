$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"

Get-Content -Raw "./$MappaTestsAndCoveragePath/line-coverage-badge.json"
gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-BADGE-LINE-COVERAGE.json" "./$MappaTestsAndCoveragePath/line-coverage-badge.json"

Get-Content -Raw "./$MappaTestsAndCoveragePath/branch-coverage-badge.json"
gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-BADGE-BRANCH-COVERAGE.json" "./$MappaTestsAndCoveragePath/branch-coverage-badge.json"

Get-Content -Raw "./$MappaTestsAndCoveragePath/method-coverage-badge.json"
gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-BADGE-METHOD-COVERAGE.json" "./$MappaTestsAndCoveragePath/method-coverage-badge.json"

if(Test-Path -Type Leaf "./$MappaTestsAndCoveragePath/full-history-table.md")
{
    Remove-Item "./$MappaTestsAndCoveragePath/full-history-table.md"
}

$currentHistory = $(gh gist view "7f4a85bc809328b4821b03125f9190cb" --raw -f "MAPPA-CODE-COVERAGE-HISTORY.MD")
$currentHistory = $currentHistory.Trim()
$currentHistory | Out-File "./$MappaTestsAndCoveragePath/full-history-table.md"
Get-Content -Raw "./$MappaTestsAndCoveragePath/history-table.md" | Out-File -Append -NoNewline "./$MappaTestsAndCoveragePath/full-history-table.md"

Get-Content -Raw "./$MappaTestsAndCoveragePath/full-history-table.md"
gh gist edit "7f4a85bc809328b4821b03125f9190cb" -a "MAPPA-CODE-COVERAGE-HISTORY.MD" "./$MappaTestsAndCoveragePath/full-history-table.md"