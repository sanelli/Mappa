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

function Format-BenchmarkRatio
{
    param(
        $NumeratorEntry,
        $DenominatorEntry,
        [string]$MetricProperty
    )

    if (($null -eq $NumeratorEntry) -or ($null -eq $DenominatorEntry))
    {
        return "n/a"
    }

    $numerator = [double]$NumeratorEntry.$MetricProperty
    $denominator = [double]$DenominatorEntry.$MetricProperty
    if ($denominator -eq 0)
    {
        return "n/a"
    }

    return ("{0:0.#}%" -f (($numerator / $denominator) * 100.0))
}

function Write-BenchmarkSummaryMarkdown
{
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Benchmarks,
        [string]$OutputPath
    )

    $ratioCompetitors = @("Automapper", "Mapperly", "Mapster")

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine("# Benchmark summary")
    [void]$builder.AppendLine()
    [void]$builder.AppendLine("AutoMapper is the BenchmarkDotNet ratio baseline. Absolute values are mean time in nanoseconds and allocated bytes. Ratio columns are competitor / Mappa as a percentage (n/a when either side is missing or Mappa is zero).")
    [void]$builder.AppendLine()
    [void]$builder.Append("| Benchmark |")
    foreach ($mapper in $MapperOrder)
    {
        [void]$builder.Append(" $mapper Mean (ns) | $mapper Allocated (B) |")
    }

    foreach ($competitor in $ratioCompetitors)
    {
        [void]$builder.Append(" $competitor/Mappa Time | $competitor/Mappa Alloc |")
    }

    [void]$builder.AppendLine()
    [void]$builder.Append("| --- |")
    foreach ($mapper in $MapperOrder)
    {
        [void]$builder.Append(" --- | --- |")
    }

    foreach ($competitor in $ratioCompetitors)
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

        $mappaEntry = $benchmark.Mappers["Mappa"]
        foreach ($competitor in $ratioCompetitors)
        {
            $competitorEntry = $benchmark.Mappers[$competitor]
            $timeRatio = Format-BenchmarkRatio -NumeratorEntry $competitorEntry -DenominatorEntry $mappaEntry -MetricProperty "MeanNs"
            $allocRatio = Format-BenchmarkRatio -NumeratorEntry $competitorEntry -DenominatorEntry $mappaEntry -MetricProperty "AllocatedBytes"
            [void]$builder.Append(" $timeRatio | $allocRatio |")
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
        # Default job uses more iterations/warmups than Short (longer wall-clock, lower noise).
        dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Default -e "Csv" "Html" "GitHub" @filterArgs
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
    $timePercentSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-TIME-PERCENTAGES.svg"
    $memoryPercentSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-MEMORY-PERCENTAGES.svg"
    $comparisonSvgPath = Join-Path $MappaBenchmarkPath "MAPPA-BENCHMARK-COMPARISON.svg"
    $comparisonMarkdownPath = Join-Path $MappaBenchmarkPath "Benchmark.Comparison.md"

    Write-BenchmarkSummaryMarkdown -Benchmarks $benchmarks -OutputPath $summaryPath
    Write-BenchmarkHistoryTable -Benchmarks $benchmarks -MappaVersion $currentMappaVersion -OutputPath $historyPath

    $comparisonRows = @(Get-BenchmarkComparisonRows -Benchmarks $benchmarks)
    if ($comparisonRows.Count -eq 0)
    {
        throw "No comparison chart benchmarks were found in CSV results."
    }

    New-BenchmarkComparisonSvg -Rows $comparisonRows -OutputPath $comparisonSvgPath
    Write-BenchmarkComparisonMarkdown -Rows $comparisonRows -OutputPath $comparisonMarkdownPath

    $chartBenchmarks = [System.Collections.Generic.List[object]]::new()
    $percentageChartBenchmarks = [System.Collections.Generic.List[object]]::new()
    $percentageMappers = @("Automapper", "Mapster", "Mapperly")
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

        $mappaEntry = $source.Mappers["Mappa"]
        $percentageMappersMap = @{}
        if ($null -ne $mappaEntry)
        {
            foreach ($mapper in $percentageMappers)
            {
                $entry = $source.Mappers[$mapper]
                if ($null -eq $entry)
                {
                    continue
                }

                $timePercent = if ($mappaEntry.MeanNs -eq 0) {
                    $null
                } else {
                    [Math]::Round(($entry.MeanNs / $mappaEntry.MeanNs) * 100.0, 3)
                }
                $allocPercent = if ($mappaEntry.AllocatedBytes -eq 0) {
                    $null
                } else {
                    [Math]::Round(($entry.AllocatedBytes / $mappaEntry.AllocatedBytes) * 100.0, 3)
                }

                if (($null -eq $timePercent) -and ($null -eq $allocPercent))
                {
                    continue
                }

                $percentageMappersMap[$mapper] = [pscustomobject]@{
                    TimePercent = $timePercent
                    AllocPercent = $allocPercent
                }
            }
        }

        if ($percentageMappersMap.Count -gt 0)
        {
            $percentageChartBenchmarks.Add([pscustomobject]@{
                    Name = $source.Name
                    Mappers = $percentageMappersMap
                }) | Out-Null
        }
    }

    if ($chartBenchmarks.Count -eq 0)
    {
        throw "No SVG chart benchmarks were found in CSV results."
    }

    $memoryChartBenchmarks = @(
        $chartBenchmarks |
            Where-Object { $script:BenchmarkMemoryChartExcludedNames -notcontains $_.Name }
    )

    New-BenchmarkGroupedBarSvg `
        -Benchmarks $chartBenchmarks.ToArray() `
        -MetricProperty "MeanUs" `
        -YAxisLabel "Mean time (us)" `
        -Title "Benchmark mean time" `
        -OutputPath $timeSvgPath `
        -YAxisTickStep 50 `
        -ValueLabelFormat "0.###"

    if ($memoryChartBenchmarks.Count -gt 0)
    {
        New-BenchmarkGroupedBarSvg `
            -Benchmarks $memoryChartBenchmarks `
            -MetricProperty "AllocatedKb" `
            -YAxisLabel "Allocated (KB)" `
            -Title "Benchmark allocated memory" `
            -OutputPath $memorySvgPath `
            -YAxisTickStep 100 `
            -ValueLabelFormat "0.#"
    }

    if ($percentageChartBenchmarks.Count -gt 0)
    {
        # Filter out mappers with null for the selected metric so bars are skipped.
        $timePercentBenchmarks = [System.Collections.Generic.List[object]]::new()
        $allocPercentBenchmarks = [System.Collections.Generic.List[object]]::new()
        foreach ($benchmark in $percentageChartBenchmarks)
        {
            $timeMappers = @{}
            $allocMappers = @{}
            foreach ($mapper in $percentageMappers)
            {
                $entry = $benchmark.Mappers[$mapper]
                if ($null -eq $entry)
                {
                    continue
                }

                if ($null -ne $entry.TimePercent)
                {
                    $timeMappers[$mapper] = [pscustomobject]@{ TimePercent = [double]$entry.TimePercent }
                }

                if ($null -ne $entry.AllocPercent)
                {
                    $allocMappers[$mapper] = [pscustomobject]@{ AllocPercent = [double]$entry.AllocPercent }
                }
            }

            if ($timeMappers.Count -gt 0)
            {
                $timePercentBenchmarks.Add([pscustomobject]@{ Name = $benchmark.Name; Mappers = $timeMappers }) | Out-Null
            }

            if (($allocMappers.Count -gt 0) -and ($script:BenchmarkMemoryChartExcludedNames -notcontains $benchmark.Name))
            {
                $allocPercentBenchmarks.Add([pscustomobject]@{ Name = $benchmark.Name; Mappers = $allocMappers }) | Out-Null
            }
        }

        if ($timePercentBenchmarks.Count -gt 0)
        {
            New-BenchmarkGroupedBarSvg `
                -Benchmarks $timePercentBenchmarks.ToArray() `
                -MetricProperty "TimePercent" `
                -YAxisLabel "Competitor / Mappa (%)" `
                -Title "Benchmark mean time vs Mappa" `
                -OutputPath $timePercentSvgPath `
                -YAxisTickStep 250 `
                -YAxisMax 1500 `
                -ValueLabelFormat "0.#" `
                -ValueLabelSuffix "%" `
                -MapperNames $percentageMappers `
                -EmphasizeGuideAt 100 `
                -BoldValueLabelsAboveGuide `
                -ProportionalOverflowStubs `
                -BreakLineStyle RoundedWave `
                -DrawMinorYAxisGuides
        }

        if ($allocPercentBenchmarks.Count -gt 0)
        {
            New-BenchmarkGroupedBarSvg `
                -Benchmarks $allocPercentBenchmarks.ToArray() `
                -MetricProperty "AllocPercent" `
                -YAxisLabel "Competitor / Mappa (%)" `
                -Title "Benchmark allocated memory vs Mappa" `
                -OutputPath $memoryPercentSvgPath `
                -YAxisTickStep 100 `
                -ValueLabelFormat "0.#" `
                -ValueLabelSuffix "%" `
                -MapperNames $percentageMappers `
                -EmphasizeGuideAt 100 `
                -BoldValueLabelsAboveGuide `
                -DrawMinorYAxisGuides
        }
    }

    Write-Host "Wrote:"
    Write-Host " - $summaryPath"
    Write-Host " - $historyPath"
    Write-Host " - $comparisonSvgPath"
    Write-Host " - $comparisonMarkdownPath"
    Write-Host " - $timeSvgPath"
    Write-Host " - $memorySvgPath"
    Write-Host " - $timePercentSvgPath"
    Write-Host " - $memoryPercentSvgPath"
}
finally
{
    Pop-Location
}

exit $exitCode