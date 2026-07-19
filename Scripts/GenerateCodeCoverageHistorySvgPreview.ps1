param(
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$HistoryFileName = "MAPPA-CODE-COVERAGE-HISTORY.MD",
    [string]$OutputPath = "./.mappa-tests-and-coverage/history-preview.svg"
)

Remove-Item Env:HTTP_PROXY, Env:HTTPS_PROXY, Env:ALL_PROXY, Env:http_proxy, Env:https_proxy, Env:all_proxy -ErrorAction SilentlyContinue
$env:NO_PROXY = "*"
$env:no_proxy = "*"

. "$PSScriptRoot/CodeCoverageHistorySvg.ps1"

$outputDirectory = Split-Path -Parent $OutputPath
if ($outputDirectory -and -not (Test-Path -LiteralPath $outputDirectory))
{
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}

$historyMarkdownPath = Join-Path $outputDirectory "full-history-table.preview.md"
Write-Host "Downloading coverage history from gist $GistId ($HistoryFileName)..."
$history = gh gist view $GistId --raw -f $HistoryFileName
if (-not $?)
{
    throw "Failed to download coverage history from gist $GistId."
}

$history.Trim() | Out-File -FilePath $historyMarkdownPath -Encoding utf8
Write-Host "Generating preview SVG at $OutputPath..."
New-CodeCoverageHistorySvg -HistoryMarkdownPath $historyMarkdownPath -OutputPath $OutputPath
Write-Host "Preview SVG written to $OutputPath"
