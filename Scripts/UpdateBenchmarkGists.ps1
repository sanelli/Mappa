param(
    [string]$GistId = "7f4a85bc809328b4821b03125f9190cb",
    [string]$MappaBenchmarkPath = ".mappa-benchmark",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

. "$PSScriptRoot/GistHelpers.ps1"

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

function Publish-GistFileWithAddFallback
{
    param(
        [string]$GistId,
        [string]$RemoteFileName,
        [string]$LocalPath
    )

    try
    {
        Publish-GistFile -GistId $GistId -RemoteFileName $RemoteFileName -LocalPath $LocalPath -Mode Update
    }
    catch
    {
        Write-Host "Update failed for $RemoteFileName; trying Add (first publish)..."
        Publish-GistFile -GistId $GistId -RemoteFileName $RemoteFileName -LocalPath $LocalPath -Mode Add
    }
}

$comparisonSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-COMPARISON.svg"
$timeSummarySvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-TIME.svg"
$memorySummarySvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-MEMORY.svg"
$timePercentSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-TIME-PERCENTAGES.svg"
$memoryPercentSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg"

foreach ($svgPath in @($comparisonSvgPath, $timeSummarySvgPath, $memorySummarySvgPath, $timePercentSvgPath, $memoryPercentSvgPath))
{
    if (-not (Test-Path -LiteralPath $svgPath))
    {
        throw "Benchmark SVG not found: $svgPath. Run Scripts/RunBenchmarks.ps1 first."
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

    Publish-GistFileWithAddFallback -GistId $GistId -RemoteFileName "MAPPA-BENCHMARK-COMPARISON.svg" -LocalPath $comparisonSvgPath
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
