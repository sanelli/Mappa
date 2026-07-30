param(
    # Quoted when passed to dotnet so PowerShell does not glob-expand "*".
    [string]$Filter = "*",
    # Run only the benchmarks used by MAPPA-BENCHMARK-TIME/MEMORY.svg charts.
    [switch]$ChartBenchmarksOnly,
    [switch]$SkipRun,
    [switch]$ListAvailable
)

$ErrorActionPreference = "Stop"

. "$PSScriptRoot/BenchmarkChartsSvg.ps1"

$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$MappaBenchmarkPath = ".mappa-benchmark"
$ArtifactsPath = "BenchmarkDotNet.Artifacts"
$ResultsPath = Join-Path $ArtifactsPath "results"
$MapperOrder = @("Automapper", "Mapster", "Mapperly", "Mappa")
$SvgBenchmarkNames = $script:BenchmarkChartNames

function Get-BenchmarkDotNetFilterArgs
{
    if ($ChartBenchmarksOnly)
    {
        # BenchmarkDotNet accepts multiple patterns after a single --filter:
        #   --filter '*.ClassA.*' '*.ClassB.*'
        # Repeating --filter is rejected ("Option 'filter' is defined multiple times").
        $filterArgs = @("--filter")
        foreach ($name in $script:BenchmarkChartNames)
        {
            $filterArgs += "*$name*"
        }

        return ,$filterArgs
    }

    return @("--filter", $Filter)
}

function Get-BenchmarkFilterDescription
{
    if ($ChartBenchmarksOnly)
    {
        return ("ChartBenchmarksOnly: " + ($script:BenchmarkChartNames -join ", "))
    }

    return $Filter
}

function Get-BenchmarkNameFromCsvPath
{
    param([string]$CsvPath)

    $fileName = [System.IO.Path]::GetFileNameWithoutExtension($CsvPath)
    if ($fileName -match '^(?<type>.+)-report$')
    {
        $typeName = $Matches["type"]
        $parts = $typeName.Split(".")
        return $parts[$parts.Length - 1]
    }

    return $fileName
}

function Get-CsvPropertyValue
{
    param(
        $Row,
        [string[]]$CandidateNames
    )

    foreach ($name in $CandidateNames)
    {
        $property = $Row.PSObject.Properties[$name]
        if ($null -ne $property -and -not [string]::IsNullOrWhiteSpace([string]$property.Value))
        {
            return [string]$property.Value
        }
    }

    return $null
}

function Read-BenchmarkResults
{
    param([string]$CsvResultsDirectory)

    if (-not (Test-Path -LiteralPath $CsvResultsDirectory))
    {
        throw "Benchmark CSV results directory not found: $CsvResultsDirectory"
    }

    $csvFiles = @(Get-ChildItem -LiteralPath $CsvResultsDirectory -Filter "*-report.csv" -File)
    if ($csvFiles.Count -eq 0)
    {
        throw "No *-report.csv files found under $CsvResultsDirectory."
    }

    $benchmarksByName = [ordered]@{}

    foreach ($csvFile in $csvFiles)
    {
        $benchmarkName = Get-BenchmarkNameFromCsvPath -CsvPath $csvFile.FullName
        $rows = Import-Csv -LiteralPath $csvFile.FullName
        foreach ($row in $rows)
        {
            $method = Get-CsvPropertyValue -Row $row -CandidateNames @("Method", "method")
            if ([string]::IsNullOrWhiteSpace($method))
            {
                continue
            }

            $mapperName = $null
            foreach ($candidate in $MapperOrder)
            {
                if ($method -eq $candidate -or $method.EndsWith("." + $candidate) -or $method.EndsWith(" " + $candidate))
                {
                    $mapperName = $candidate
                    break
                }
            }

            if ($null -eq $mapperName)
            {
                continue
            }

            $meanText = Get-CsvPropertyValue -Row $row -CandidateNames @("Mean", "mean")
            $allocatedText = Get-CsvPropertyValue -Row $row -CandidateNames @("Allocated", "allocated")
            if ([string]::IsNullOrWhiteSpace($meanText) -or $meanText -eq "NA" -or $meanText -eq "?")
            {
                continue
            }

            $meanNs = ConvertTo-Nanoseconds -Text $meanText
            $allocatedBytes = ConvertTo-AllocatedBytes -Text $allocatedText
            if ($null -eq $meanNs)
            {
                continue
            }

            if (-not $benchmarksByName.Contains($benchmarkName))
            {
                $benchmarksByName[$benchmarkName] = [pscustomobject]@{
                    Name = $benchmarkName
                    Mappers = @{}
                }
            }

            $benchmarksByName[$benchmarkName].Mappers[$mapperName] = [pscustomobject]@{
                MeanNs = [Math]::Round($meanNs, 3)
                AllocatedBytes = if ($null -eq $allocatedBytes) { 0.0 } else { [Math]::Round($allocatedBytes, 3) }
            }
        }
    }

    return @($benchmarksByName.Values)
}

function Format-Metric
{
    param([double]$Value)

    return ("{0:0.###}" -f $Value)
}

function Write-BenchmarkSummaryMarkdown
{
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Benchmarks,
        [string]$OutputPath
    )

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine("# Benchmark summary")
    [void]$builder.AppendLine()
    [void]$builder.AppendLine("AutoMapper is the ratio baseline. Values are mean time in nanoseconds and allocated bytes.")
    [void]$builder.AppendLine()
    [void]$builder.Append("| Benchmark |")
    foreach ($mapper in $MapperOrder)
    {
        [void]$builder.Append(" $mapper Mean (ns) | $mapper Allocated (B) |")
    }

    [void]$builder.AppendLine()
    [void]$builder.Append("| --- |")
    foreach ($mapper in $MapperOrder)
    {
        [void]$builder.Append(" --- | --- |")
    }

    [void]$builder.AppendLine()

    foreach ($benchmark in ($Benchmarks | Sort-Object Name))
    {
        [void]$builder.Append("| $($benchmark.Name) |")
        foreach ($mapper in $MapperOrder)
        {
            $entry = $benchmark.Mappers[$mapper]
            if ($null -eq $entry)
            {
                [void]$builder.Append(" n/a | n/a |")
            }
            else
            {
                [void]$builder.Append(" $(Format-Metric -Value $entry.MeanNs) | $(Format-Metric -Value $entry.AllocatedBytes) |")
            }
        }

        [void]$builder.AppendLine()
    }

    [System.IO.File]::WriteAllText($OutputPath, $builder.ToString().TrimEnd() + "`n")
}

function Write-BenchmarkHistoryTable
{
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Benchmarks,
        [string]$MappaVersion,
        [string]$OutputPath
    )

    $timestamp = Get-Date -Format "yyyy/MM/dd HH:mm:ss"
    $builder = New-Object System.Text.StringBuilder

    # Include ALL benchmarks (Mappa time + memory), not a subset.
    foreach ($benchmark in ($Benchmarks | Sort-Object Name))
    {
        $mappa = $benchmark.Mappers["Mappa"]
        if ($null -eq $mappa)
        {
            continue
        }

        [void]$builder.AppendLine("| $timestamp | $MappaVersion | $($benchmark.Name) | TIME_NS | $(Format-Metric -Value $mappa.MeanNs) |")
        [void]$builder.AppendLine("| $timestamp | $MappaVersion | $($benchmark.Name) | ALLOC_B | $(Format-Metric -Value $mappa.AllocatedBytes) |")
    }

    [System.IO.File]::WriteAllText($OutputPath, $builder.ToString().TrimEnd() + "`n")
}

Push-Location $RepoRoot
$exitCode = 0
try
{
    if ($ListAvailable)
    {
        Write-Host "Available benchmarks:"
        $filterArgs = Get-BenchmarkDotNetFilterArgs
        # Quote each filter value: an unquoted "*" is expanded by PowerShell.
        dotnet run -c Release --project ./Mappa.Benchmark/ -- --list flat @filterArgs
        if (-not $?)
        {
            $exitCode = 1
        }

        return
    }

    if (-not $SkipRun)
    {
        if (Test-Path -LiteralPath $ArtifactsPath)
        {
            Remove-Item -Recurse -Force -LiteralPath $ArtifactsPath
        }
    }

    if (Test-Path -LiteralPath $MappaBenchmarkPath)
    {
        Remove-Item -Recurse -Force -LiteralPath $MappaBenchmarkPath
    }

    New-Item -ItemType Directory -Path $MappaBenchmarkPath | Out-Null

    if (-not $SkipRun)
    {
        $filterDescription = Get-BenchmarkFilterDescription
        $filterArgs = Get-BenchmarkDotNetFilterArgs
        Write-Host "Running benchmarks (filter: $filterDescription)..."
        # Pass filters as separate args so PowerShell does not glob-expand "*".
        dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Csv" "Html" "GitHub" @filterArgs
        if (-not $?)
        {
            Write-Host "Benchmark run failed." -ForegroundColor Red
            $exitCode = 1
            return
        }
    }
    elseif (-not (Test-Path -LiteralPath $ResultsPath))
    {
        throw "SkipRun was set but results were not found at $ResultsPath."
    }

    $benchmarks = @(Read-BenchmarkResults -CsvResultsDirectory $ResultsPath)
    if ($benchmarks.Count -eq 0)
    {
        throw "No benchmark mapper rows were parsed from CSV results."
    }

    [xml]$currentVersionFile = Get-Content -LiteralPath ./MappaVersion.targets
    $currentMappaVersion = $currentVersionFile.Project.PropertyGroup.MappaVersion

    $summaryPath = Join-Path $MappaBenchmarkPath "Benchmark.Summary.md"
    $historyPath = Join-Path $MappaBenchmarkPath "history-table.md"
    $timeSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-TIME.svg"
    $memorySvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-MEMORY.svg"

    Write-BenchmarkSummaryMarkdown -Benchmarks $benchmarks -OutputPath $summaryPath
    Write-BenchmarkHistoryTable -Benchmarks $benchmarks -MappaVersion $currentMappaVersion -OutputPath $historyPath

    $chartBenchmarks = [System.Collections.Generic.List[object]]::new()
    foreach ($name in $SvgBenchmarkNames)
    {
        $match = @($benchmarks | Where-Object { $_.Name -eq $name } | Select-Object -First 1)
        if ($match.Count -eq 0 -or $null -eq $match[0])
        {
            Write-Host "Skipping SVG series '$name' (no CSV results)." -ForegroundColor Yellow
            continue
        }

        $source = $match[0]
        $convertedMappers = @{}
        foreach ($mapper in $MapperOrder)
        {
            $entry = $source.Mappers[$mapper]
            if ($null -eq $entry)
            {
                continue
            }

            $convertedMappers[$mapper] = [pscustomobject]@{
                MeanUs = [Math]::Round($entry.MeanNs / 1000.0, 6)
                AllocatedKb = [Math]::Round($entry.AllocatedBytes / 1024.0, 6)
            }
        }

        $chartBenchmarks.Add([pscustomobject]@{
                Name = $source.Name
                Mappers = $convertedMappers
            }) | Out-Null
    }

    if ($chartBenchmarks.Count -eq 0)
    {
        throw "No SVG chart benchmarks were found in CSV results."
    }

    New-BenchmarkGroupedBarSvg `
        -Benchmarks $chartBenchmarks.ToArray() `
        -MetricProperty "MeanUs" `
        -YAxisLabel "Mean time (us)" `
        -Title "Benchmark mean time" `
        -OutputPath $timeSvgPath `
        -YAxisTickStep 50 `
        -ValueLabelFormat "0.###"

    New-BenchmarkGroupedBarSvg `
        -Benchmarks $chartBenchmarks.ToArray() `
        -MetricProperty "AllocatedKb" `
        -YAxisLabel "Allocated (KB)" `
        -Title "Benchmark allocated memory" `
        -OutputPath $memorySvgPath `
        -YAxisTickStep 50 `
        -ValueLabelFormat "0.#"

    Write-Host "Wrote:"
    Write-Host " - $summaryPath"
    Write-Host " - $historyPath"
    Write-Host " - $timeSvgPath"
    Write-Host " - $memorySvgPath"
}
finally
{
    Pop-Location
}

exit $exitCode