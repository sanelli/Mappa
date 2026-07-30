# Shared helpers for Mappa benchmark grouped-bar SVG charts.

$script:BenchmarkMapperOrder = @("Automapper", "Mapster", "Mapperly", "Mappa")
$script:BenchmarkMapperColors = @{
    Automapper = "#4E79A7"
    Mapster    = "#E15759"
    Mapperly   = "#F28E2B"
    Mappa      = "#76B7B2"
}

# Same subset used by MAPPA-BENCHMARK-TIME.svg / MAPPA-BENCHMARK-MEMORY.svg.
$script:BenchmarkChartNames = @(
    "ArrayToListBenchmark",
    "DictionaryBenchmark",
    "ListToArrayBenchmark",
    "FastListToArrayBenchmark",
    "IQueryableProjectionBenchmark",
    "NestedDtoBenchmark",
    "ListToHashSetBenchmark"
)

function ConvertTo-InvariantDouble
{
    param([string]$Text)

    if ([string]::IsNullOrWhiteSpace($Text))
    {
        return $null
    }

    $normalized = $Text.Trim() -replace ',', ''
    if ($normalized -match '[-+]?\d+(\.\d+)?([eE][-+]?\d+)?')
    {
        return [double]::Parse($Matches[0], [System.Globalization.CultureInfo]::InvariantCulture)
    }

    return $null
}

function ConvertTo-Nanoseconds
{
    param([string]$Text)

    $value = ConvertTo-InvariantDouble -Text $Text
    if ($null -eq $value)
    {
        return $null
    }

    $lower = $Text.ToLowerInvariant()
    if ($lower -match '\bms\b')
    {
        return $value * 1000000.0
    }

    if ($lower -match '\bus\b' -or $lower -match '\bμs\b')
    {
        return $value * 1000.0
    }

    if ($lower -match '\bs\b' -and $lower -notmatch '\bns\b' -and $lower -notmatch '\bms\b' -and $lower -notmatch '\bus\b')
    {
        return $value * 1000000000.0
    }

    return $value
}

function ConvertTo-AllocatedBytes
{
    param([string]$Text)

    $value = ConvertTo-InvariantDouble -Text $Text
    if ($null -eq $value)
    {
        return $null
    }

    $lower = $Text.ToLowerInvariant()
    if ($lower -match '\bkb\b')
    {
        return $value * 1024.0
    }

    if ($lower -match '\bmb\b')
    {
        return $value * 1024.0 * 1024.0
    }

    if ($lower -match '\bgb\b')
    {
        return $value * 1024.0 * 1024.0 * 1024.0
    }

    return $value
}

function New-BenchmarkGroupedBarSvg
{
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Benchmarks,

        [Parameter(Mandatory = $true)]
        [string]$MetricProperty,

        [Parameter(Mandatory = $true)]
        [string]$YAxisLabel,

        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [string]$OutputPath,

        [Parameter(Mandatory = $true)]
        [double]$YAxisTickStep,

        [string]$ValueLabelFormat = "0.##"
    )

    if ($Benchmarks.Count -eq 0)
    {
        throw "No benchmark rows available to chart."
    }

    if ($YAxisTickStep -le 0)
    {
        throw "YAxisTickStep must be greater than zero."
    }

    $mappers = $script:BenchmarkMapperOrder
    $groupCount = $Benchmarks.Count
    $seriesCount = $mappers.Count

    $leftMargin = 80.0
    $rightMargin = 40.0
    $topMargin = 60.0
    $bottomMargin = 120.0
    $groupGap = 24.0
    $barWidth = 18.0
    $groupWidth = ($seriesCount * $barWidth) + 8.0
    $plotWidth = [Math]::Max(640.0, ($groupCount * ($groupWidth + $groupGap)) + $groupGap)
    $plotHeight = 360.0
    $svgWidth = [Math]::Round($leftMargin + $plotWidth + $rightMargin, 0)
    $svgHeight = [Math]::Round($topMargin + $plotHeight + $bottomMargin, 0)

    $maxValue = 0.0
    foreach ($benchmark in $Benchmarks)
    {
        foreach ($mapper in $mappers)
        {
            $entry = $benchmark.Mappers[$mapper]
            if ($null -eq $entry)
            {
                continue
            }

            $metric = [double]$entry.$MetricProperty
            if ($metric -gt $maxValue)
            {
                $maxValue = $metric
            }
        }
    }

    $tickCount = [int][Math]::Ceiling($maxValue / $YAxisTickStep)
    if ($tickCount -lt 1)
    {
        $tickCount = 1
    }

    $axisMax = $tickCount * $YAxisTickStep

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine('<?xml version="1.0" encoding="UTF-8" standalone="no"?>')
    [void]$builder.AppendLine("<svg xmlns=`"http://www.w3.org/2000/svg`" width=`"$svgWidth`" height=`"$svgHeight`" viewBox=`"0 0 $svgWidth $svgHeight`">")
    [void]$builder.AppendLine("  <rect width=`"100%`" height=`"100%`" fill=`"#ffffff`"/>")
    [void]$builder.AppendLine("  <text x=`"$([Math]::Round($svgWidth / 2, 1))`" y=`"28`" text-anchor=`"middle`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"18`" fill=`"#222222`">$Title</text>")
    [void]$builder.AppendLine("  <text x=`"20`" y=`"$([Math]::Round($topMargin + ($plotHeight / 2), 1))`" text-anchor=`"middle`" transform=`"rotate(-90 20 $([Math]::Round($topMargin + ($plotHeight / 2), 1)))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" fill=`"#444444`">$YAxisLabel</text>")

    for ($tickIndex = 0; $tickIndex -le $tickCount; $tickIndex++)
    {
        $value = $tickIndex * $YAxisTickStep
        $ratio = 1.0 - ($value / $axisMax)
        $y = $topMargin + ($plotHeight * $ratio)
        if ($YAxisTickStep -ge 1.0)
        {
            $label = "{0:0}" -f $value
        }
        else
        {
            $label = "{0:0.##}" -f $value
        }

        [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$([Math]::Round($y, 2))`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$([Math]::Round($y, 2))`" stroke=`"#dddddd`" stroke-width=`"1`"/>")
        [void]$builder.AppendLine("  <text x=`"$([Math]::Round($leftMargin - 8, 2))`" y=`"$([Math]::Round($y + 4, 2))`" text-anchor=`"end`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"11`" fill=`"#555555`">$label</text>")
    }

    [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$topMargin`" x2=`"$leftMargin`" y2=`"$([Math]::Round($topMargin + $plotHeight, 2))`" stroke=`"#333333`" stroke-width=`"1.5`"/>")
    [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$([Math]::Round($topMargin + $plotHeight, 2))`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$([Math]::Round($topMargin + $plotHeight, 2))`" stroke=`"#333333`" stroke-width=`"1.5`"/>")

    for ($groupIndex = 0; $groupIndex -lt $groupCount; $groupIndex++)
    {
        $benchmark = $Benchmarks[$groupIndex]
        $groupStartX = $leftMargin + $groupGap + ($groupIndex * ($groupWidth + $groupGap))

        for ($seriesIndex = 0; $seriesIndex -lt $seriesCount; $seriesIndex++)
        {
            $mapper = $mappers[$seriesIndex]
            $entry = $benchmark.Mappers[$mapper]
            if ($null -eq $entry)
            {
                continue
            }

            $metric = [double]$entry.$MetricProperty
            $barHeight = ($metric / $axisMax) * $plotHeight
            $x = $groupStartX + ($seriesIndex * $barWidth)
            $y = $topMargin + $plotHeight - $barHeight
            $color = $script:BenchmarkMapperColors[$mapper]
            [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($x, 2))`" y=`"$([Math]::Round($y, 2))`" width=`"$barWidth`" height=`"$([Math]::Round($barHeight, 2))`" fill=`"$color`"/>")

            $valueText = $metric.ToString($ValueLabelFormat, [System.Globalization.CultureInfo]::InvariantCulture)
            $textX = $x + ($barWidth / 2.0)
            $textY = $y - 4.0
            # Always place the value vertically above the bar, using the bar color.
            [void]$builder.AppendLine("  <text x=`"$([Math]::Round($textX, 2))`" y=`"$([Math]::Round($textY, 2))`" text-anchor=`"start`" dominant-baseline=`"middle`" transform=`"rotate(-90 $([Math]::Round($textX, 2)) $([Math]::Round($textY, 2)))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"9`" fill=`"$color`">$valueText</text>")
        }

        $labelX = $groupStartX + ($groupWidth / 2.0)
        $labelY = $topMargin + $plotHeight + 16
        $escapedName = [System.Security.SecurityElement]::Escape($benchmark.Name)
        [void]$builder.AppendLine("  <text x=`"$([Math]::Round($labelX, 2))`" y=`"$([Math]::Round($labelY, 2))`" text-anchor=`"end`" transform=`"rotate(-40 $([Math]::Round($labelX, 2)) $([Math]::Round($labelY, 2)))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"11`" fill=`"#333333`">$escapedName</text>")
    }

    # Legend drawn last so it sits over the graph (single column, top-right of plot).
    $legendItemHeight = 18.0
    $legendBoxPadding = 8.0
    $legendBoxWidth = 110.0
    $legendBoxHeight = ($seriesCount * $legendItemHeight) + ($legendBoxPadding * 2)
    $legendBoxX = $leftMargin + $plotWidth - $legendBoxWidth - 12.0
    $legendBoxY = $topMargin + 12.0
    [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($legendBoxX, 2))`" y=`"$([Math]::Round($legendBoxY, 2))`" width=`"$([Math]::Round($legendBoxWidth, 2))`" height=`"$([Math]::Round($legendBoxHeight, 2))`" fill=`"#ffffff`" fill-opacity=`"0.9`" stroke=`"#cccccc`" stroke-width=`"1`"/>")

    for ($seriesIndex = 0; $seriesIndex -lt $seriesCount; $seriesIndex++)
    {
        $mapper = $mappers[$seriesIndex]
        $color = $script:BenchmarkMapperColors[$mapper]
        $itemY = $legendBoxY + $legendBoxPadding + ($seriesIndex * $legendItemHeight) + 12.0
        $swatchX = $legendBoxX + $legendBoxPadding
        [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($swatchX, 2))`" y=`"$([Math]::Round($itemY - 10, 2))`" width=`"12`" height=`"12`" fill=`"$color`"/>")
        [void]$builder.AppendLine("  <text x=`"$([Math]::Round($swatchX + 18, 2))`" y=`"$([Math]::Round($itemY, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" fill=`"#333333`">$mapper</text>")
    }

    [void]$builder.AppendLine("</svg>")

    $directory = Split-Path -Parent $OutputPath
    if (-not [string]::IsNullOrWhiteSpace($directory) -and -not (Test-Path -LiteralPath $directory))
    {
        New-Item -ItemType Directory -Path $directory | Out-Null
    }

    $fullOutputPath = $OutputPath
    if (-not [System.IO.Path]::IsPathRooted($OutputPath))
    {
        $fullOutputPath = Join-Path (Get-Location).Path $OutputPath
    }

    [System.IO.File]::WriteAllText($fullOutputPath, $builder.ToString())
}

$script:BenchmarkHistorySeriesColors = @(
    "#e41a1c", "#377eb8", "#4daf4a", "#984ea3", "#ff7f00",
    "#a65628", "#f781bf", "#66c2a5", "#fc8d62", "#8da0cb",
    "#e78ac3", "#a6d854", "#ffd92f", "#e5c494", "#b3b3b3",
    "#1b9e77", "#d95f02", "#7570b3", "#e7298a", "#66a61e"
)

function Test-BenchmarkHistoryMarkdownHeaderOrSeparator
{
    param([string[]]$Cells)

    if ($Cells.Count -lt 5)
    {
        return $true
    }

    if ($Cells[0] -eq "Timestamp" -or $Cells[3] -eq "Measure" -or $Cells[4] -eq "Value")
    {
        return $true
    }

    foreach ($cell in $Cells)
    {
        if ($cell -notmatch '^-+$')
        {
            return $false
        }
    }

    return $true
}

function ConvertFrom-BenchmarkHistoryMarkdown
{
    param([string]$Markdown)

    $rows = @()
    if ([string]::IsNullOrWhiteSpace($Markdown))
    {
        return $rows
    }

    foreach ($historyLine in ($Markdown -split "`r?`n"))
    {
        if ($historyLine -notmatch '^\|')
        {
            continue
        }

        $cells = @($historyLine.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" })
        if (Test-BenchmarkHistoryMarkdownHeaderOrSeparator -Cells $cells)
        {
            continue
        }

        if ($cells.Count -lt 5)
        {
            continue
        }

        $value = $null
        try
        {
            $value = [double]::Parse($cells[4], [System.Globalization.CultureInfo]::InvariantCulture)
        }
        catch
        {
            continue
        }

        $rows += [pscustomobject]@{
            Timestamp = $cells[0]
            Version   = $cells[1]
            Benchmark = $cells[2]
            Measure   = $cells[3]
            Value     = $value
        }
    }

    return $rows
}

function Get-BenchmarkHistoryTrendArrow
{
    param(
        [double]$Current,
        $Previous
    )

    if ($null -eq $Previous)
    {
        return ""
    }

    $previousValue = [double]$Previous
    $delta = [Math]::Round($Current - $previousValue, 3)
    if ($Current -lt $previousValue)
    {
        # Lower is better.
        return " &#9650;{0}" -f (("{0:0.###}" -f $delta))
    }

    if ($Current -gt $previousValue)
    {
        $deltaText = if ($delta -gt 0) { "+{0:0.###}" -f $delta } else { "{0:0.###}" -f $delta }
        return " &#9660;$deltaText"
    }

    return " &#61;"
}

function Get-BenchmarkHistoryNiceAxisMax
{
    param([double]$MaxValue)

    if ($MaxValue -le 0)
    {
        return 1.0
    }

    $exp = [Math]::Floor([Math]::Log10($MaxValue))
    $pow = [Math]::Pow(10.0, $exp)
    $mantissa = $MaxValue / $pow
    foreach ($step in @(1.0, 1.2, 1.5, 2.0, 2.5, 3.0, 4.0, 5.0, 6.0, 8.0, 10.0))
    {
        if ($mantissa -le $step)
        {
            return $step * $pow
        }
    }

    return 10.0 * $pow
}

function Format-BenchmarkHistoryAxisValue
{
    param([double]$Value)

    $abs = [Math]::Abs($Value)
    if ($abs -ge 1000)
    {
        return "{0:0.#}" -f $Value
    }

    if ($abs -ge 10)
    {
        return "{0:0.#}" -f $Value
    }

    if ($abs -ge 1)
    {
        return "{0:0.##}" -f $Value
    }

    return "{0:0.###}" -f $Value
}

function Get-BenchmarkHistoryLinearTrend
{
    param(
        [Parameter(Mandatory = $true)]
        [System.Collections.IList]$Values,

        [double]$StartX,
        [double]$EndX,
        [double]$AxisMin,
        [double]$AxisMax,
        [double]$PanelTopY,
        [double]$PanelBottomY
    )

    $n = $Values.Count
    if ($n -lt 2)
    {
        return $null
    }

    $sumX = 0.0
    $sumY = 0.0
    $sumXY = 0.0
    $sumXX = 0.0
    for ($index = 0; $index -lt $n; $index++)
    {
        $x = [double]$index
        $y = [double]$Values[$index]
        $sumX += $x
        $sumY += $y
        $sumXY += $x * $y
        $sumXX += $x * $x
    }

    $denominator = ($n * $sumXX) - ($sumX * $sumX)
    $span = $AxisMax - $AxisMin
    if ($span -le 0)
    {
        $span = 1.0
    }

    if ([Math]::Abs($denominator) -lt 1e-9)
    {
        $mean = $sumY / $n
        $meanY = [Math]::Round($PanelBottomY - (($mean - $AxisMin) * ($PanelBottomY - $PanelTopY) / $span), 2)
        return [pscustomobject]@{
            X1 = [Math]::Round($StartX, 2)
            Y1 = $meanY
            X2 = [Math]::Round($EndX, 2)
            Y2 = $meanY
        }
    }

    $slope = (($n * $sumXY) - ($sumX * $sumY)) / $denominator
    $intercept = ($sumY - ($slope * $sumX)) / $n
    $startValue = $intercept
    $endValue = $intercept + ($slope * ($n - 1))

    return [pscustomobject]@{
        X1 = [Math]::Round($StartX, 2)
        Y1 = [Math]::Round($PanelBottomY - (($startValue - $AxisMin) * ($PanelBottomY - $PanelTopY) / $span), 2)
        X2 = [Math]::Round($EndX, 2)
        Y2 = [Math]::Round($PanelBottomY - (($endValue - $AxisMin) * ($PanelBottomY - $PanelTopY) / $span), 2)
    }
}

function New-BenchmarkHistorySvg
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$HistoryMarkdownPath,

        [Parameter(Mandatory = $true)]
        [ValidateSet("TIME_NS", "ALLOC_B")]
        [string]$Measure,

        [Parameter(Mandatory = $true)]
        [string]$OutputPath,

        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [string]$YAxisUnitLabel,

        [string[]]$BenchmarkNames = $script:BenchmarkChartNames,

        [double]$YAxisTickStep = 50.0
    )

    if (-not (Test-Path -LiteralPath $HistoryMarkdownPath))
    {
        throw "History markdown file not found: $HistoryMarkdownPath"
    }

    if ($YAxisTickStep -le 0)
    {
        throw "YAxisTickStep must be greater than zero."
    }

    $markdown = Get-Content -Raw -LiteralPath $HistoryMarkdownPath
    $allRows = @(ConvertFrom-BenchmarkHistoryMarkdown -Markdown $markdown)
    $measureRows = @($allRows | Where-Object { $_.Measure -eq $Measure })
    if ($measureRows.Count -lt 1)
    {
        throw "No $Measure history rows found in $HistoryMarkdownPath"
    }

    $selectedNames = @($BenchmarkNames)
    if ($selectedNames.Count -lt 1)
    {
        throw "BenchmarkNames must contain at least one benchmark."
    }

    $seriesByBenchmark = @{}
    foreach ($name in $selectedNames)
    {
        $seriesByBenchmark[$name] = @()
    }

    $versions = @()
    foreach ($row in $measureRows)
    {
        if (-not $seriesByBenchmark.ContainsKey($row.Benchmark))
        {
            continue
        }

        $displayValue = if ($Measure -eq "TIME_NS") {
            [Math]::Round(([double]$row.Value) / 1000.0, 6)
        } else {
            [Math]::Round(([double]$row.Value) / 1024.0, 6)
        }

        $seriesByBenchmark[$row.Benchmark] += [pscustomobject]@{
            Version = $row.Version
            Value   = $displayValue
        }

        if ($versions -notcontains $row.Version)
        {
            $versions += $row.Version
        }
    }

    $benchmarkOrder = @($selectedNames | Where-Object { $seriesByBenchmark[$_].Count -gt 0 })
    if ($benchmarkOrder.Count -lt 1)
    {
        throw "No history rows matched the chart benchmark subset for $Measure."
    }

    $maxValue = 0.0
    foreach ($name in $benchmarkOrder)
    {
        foreach ($point in $seriesByBenchmark[$name])
        {
            if ([double]$point.Value -gt $maxValue)
            {
                $maxValue = [double]$point.Value
            }
        }
    }

    $tickCount = [int][Math]::Ceiling($maxValue / $YAxisTickStep)
    if ($tickCount -lt 1)
    {
        $tickCount = 1
    }

    $axisMin = 0.0
    $axisMax = $tickCount * $YAxisTickStep
    $span = $axisMax - $axisMin

    $numberOfVersions = $versions.Count
    $chartLeft = 90.0
    $chartRight = 690.0
    $plotTop = 50.0
    $plotBottom = 375.0
    $svgWidth = 800
    $svgHeight = 500
    $xSpacing = if ($numberOfVersions -le 1) { 0.0 } else { ($chartRight - $chartLeft) / ($numberOfVersions - 1) }
    $chartEndX = if ($numberOfVersions -le 1) { $chartLeft } else { $chartRight }
    $labelAllVersions = $numberOfVersions -le 8

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine('<svg version="1.2" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" style="height: 500px; width: 800px;font-family:''Open Sans'', sans-serif;background:white" role="img">')
    [void]$builder.AppendLine("    <title id=`"title`">$([System.Security.SecurityElement]::Escape($Title))</title>")
    [void]$builder.AppendLine("    <text x=`"400`" y=`"24`" text-anchor=`"middle`" style=`"font-weight:bold;font-size:16px;fill:black`">$([System.Security.SecurityElement]::Escape($Title))</text>")

    # Axes and guides (coverage-style).
    [void]$builder.AppendLine("    <g style=`"stroke:#CCCCCC;stroke-width: 2;`"><line x1=`"$chartLeft`" x2=`"$chartLeft`" y1=`"35`" y2=`"$plotBottom`"></line></g>")
    [void]$builder.AppendLine("    <g style=`"stroke:#CCCCCC;stroke-width: 2;`"><line x1=`"$chartLeft`" x2=`"705`" y1=`"$plotBottom`" y2=`"$plotBottom`"></line></g>")
    [void]$builder.AppendLine("    <g style=`"stroke:#DDDDDD;stroke-width: 1;stroke-dasharray: 4 4;`" aria-label=`"Horizontal guides`">")

    $yLabels = @()
    for ($tickIndex = 0; $tickIndex -le $tickCount; $tickIndex++)
    {
        $tickValue = $tickIndex * $YAxisTickStep
        $tickY = [Math]::Round($plotBottom - (($tickValue - $axisMin) * ($plotBottom - $plotTop) / $span), 2)
        if ($tickIndex -gt 0)
        {
            [void]$builder.AppendLine("        <line x1=`"$chartLeft`" x2=`"705`" y1=`"$tickY`" y2=`"$tickY`"></line>")
        }

        $yLabels += "<text x=`"80`" y=`"$tickY`" dy=`"4`">$(Format-BenchmarkHistoryAxisValue -Value $tickValue)</text>"
    }

    [void]$builder.AppendLine("    </g>")

    $yLabelMid = [Math]::Round(($plotTop + $plotBottom) / 2.0, 1)
    [void]$builder.AppendLine("    <g style=`"text-anchor: end;font-size: 13px;`">")
    [void]$builder.AppendLine("        $($yLabels -join '')")
    [void]$builder.AppendLine("        <text x=`"50`" y=`"$yLabelMid`" style=`"font-weight: bold;text-transform: uppercase;font-size: 12px;fill: black;`">$YAxisUnitLabel</text>")
    [void]$builder.AppendLine("    </g>")

    $xLabels = @()
    for ($versionIndex = 0; $versionIndex -lt $numberOfVersions; $versionIndex++)
    {
        $version = $versions[$versionIndex]
        $isMainRelease = -not $version.Contains("-")
        $isLast = $versionIndex -eq ($numberOfVersions - 1)
        if (-not ($labelAllVersions -or $isMainRelease -or $isLast))
        {
            continue
        }

        $x = [Math]::Round($chartLeft + ($versionIndex * $xSpacing), 2)
        $escapedVersion = [System.Security.SecurityElement]::Escape($version)
        $xLabels += "<text x=`"$x`" y=`"400`">$escapedVersion</text>"
    }

    [void]$builder.AppendLine("    <g style=`"text-anchor: middle;font-size: 13px;`">")
    [void]$builder.AppendLine("            $($xLabels -join '')")
    [void]$builder.AppendLine("            <text x=`"400`" y=`"440`" style=`"font-weight: bold;text-transform: uppercase;font-size: 12px;fill: black;`">Versions</text>")
    [void]$builder.AppendLine("    </g>")

    # Series lines.
    for ($seriesIndex = 0; $seriesIndex -lt $benchmarkOrder.Count; $seriesIndex++)
    {
        $benchmarkName = $benchmarkOrder[$seriesIndex]
        $points = @($seriesByBenchmark[$benchmarkName])
        $color = $script:BenchmarkHistorySeriesColors[$seriesIndex % $script:BenchmarkHistorySeriesColors.Count]
        $escapedBenchmark = [System.Security.SecurityElement]::Escape($benchmarkName)

        $valueByVersion = @{}
        foreach ($point in $points)
        {
            $valueByVersion[$point.Version] = [double]$point.Value
        }

        $polylinePoints = @()
        $circles = @()
        $orderedValues = @()
        for ($versionIndex = 0; $versionIndex -lt $numberOfVersions; $versionIndex++)
        {
            $version = $versions[$versionIndex]
            if (-not $valueByVersion.ContainsKey($version))
            {
                continue
            }

            $value = [double]$valueByVersion[$version]
            $orderedValues += $value
            $x = [Math]::Round($chartLeft + ($versionIndex * $xSpacing), 2)
            $y = [Math]::Round($plotBottom - (($value - $axisMin) * ($plotBottom - $plotTop) / $span), 2)
            $polylinePoints += "$x,$y"

            $isMainRelease = -not $version.Contains("-")
            $isLast = $versionIndex -eq ($numberOfVersions - 1)
            if ($labelAllVersions -or $isMainRelease -or $isLast)
            {
                $circles += "<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>"
            }
        }

        if ($polylinePoints.Count -lt 1)
        {
            continue
        }

        $trend = Get-BenchmarkHistoryLinearTrend `
            -Values $orderedValues `
            -StartX $chartLeft `
            -EndX $chartEndX `
            -AxisMin $axisMin `
            -AxisMax $axisMax `
            -PanelTopY $plotTop `
            -PanelBottomY $plotBottom

        $trendLine = ""
        if ($null -ne $trend)
        {
            $trendLine = "<line x1=`"$($trend.X1)`" y1=`"$($trend.Y1)`" x2=`"$($trend.X2)`" y2=`"$($trend.Y2)`" style=`"fill:none;stroke-dasharray:6 4;stroke-width:1.5`" />"
        }

        $lastValue = [double]$orderedValues[$orderedValues.Count - 1]
        $previousValue = if ($orderedValues.Count -gt 1) { [double]$orderedValues[$orderedValues.Count - 2] } else { $null }
        $lastY = [Math]::Round($plotBottom - (($lastValue - $axisMin) * ($plotBottom - $plotTop) / $span), 2)
        $arrow = Get-BenchmarkHistoryTrendArrow -Current $lastValue -Previous $previousValue
        $lastLabel = (Format-BenchmarkHistoryAxisValue -Value $lastValue) + $arrow

        [void]$builder.AppendLine("    <g style=`"stroke:$color; fill:$color`" data-setname=`"$escapedBenchmark`">")
        [void]$builder.AppendLine("        $trendLine")
        [void]$builder.AppendLine("        <polyline points=`"$($polylinePoints -join ' ')`" style=`"fill:none;stroke-width:2`" />")
        [void]$builder.AppendLine("        $($circles -join '')")
        [void]$builder.AppendLine("    </g>")
        [void]$builder.AppendLine("    <g style=`"font-size: 12px; font-weight: bold;`">")
        [void]$builder.AppendLine("        <text x=`"710`" y=`"$lastY`" dy=`"4`" style=`"fill:$color;text-anchor:start`">$lastLabel</text>")
        [void]$builder.AppendLine("    </g>")
    }

    # Legend (top-right, over the plot).
    $legendItemHeight = 18.0
    $legendBoxPadding = 8.0
    $legendBoxWidth = 210.0
    $legendBoxHeight = ($benchmarkOrder.Count * $legendItemHeight) + ($legendBoxPadding * 2)
    $legendBoxX = 470.0
    $legendBoxY = 40.0
    [void]$builder.AppendLine("    <g aria-label=`"Legend`">")
    [void]$builder.AppendLine("        <rect x=`"$legendBoxX`" y=`"$legendBoxY`" width=`"$legendBoxWidth`" height=`"$([Math]::Round($legendBoxHeight, 2))`" fill=`"#ffffff`" fill-opacity=`"0.92`" stroke=`"#cccccc`" stroke-width=`"1`"/>")
    for ($seriesIndex = 0; $seriesIndex -lt $benchmarkOrder.Count; $seriesIndex++)
    {
        $benchmarkName = $benchmarkOrder[$seriesIndex]
        $color = $script:BenchmarkHistorySeriesColors[$seriesIndex % $script:BenchmarkHistorySeriesColors.Count]
        $escapedBenchmark = [System.Security.SecurityElement]::Escape($benchmarkName)
        $itemY = $legendBoxY + $legendBoxPadding + ($seriesIndex * $legendItemHeight) + 12.0
        $lineY = [Math]::Round($itemY - 4, 2)
        [void]$builder.AppendLine("        <line x1=`"$([Math]::Round($legendBoxX + 10, 2))`" y1=`"$lineY`" x2=`"$([Math]::Round($legendBoxX + 34, 2))`" y2=`"$lineY`" style=`"stroke:$color;stroke-width:2`" />")
        [void]$builder.AppendLine("        <circle cx=`"$([Math]::Round($legendBoxX + 22, 2))`" cy=`"$lineY`" r=`"4`" style=`"fill:$color;stroke:$color`" />")
        [void]$builder.AppendLine("        <text x=`"$([Math]::Round($legendBoxX + 42, 2))`" y=`"$([Math]::Round($itemY, 2))`" style=`"fill:black;font-size:12px`">$escapedBenchmark</text>")
    }

    [void]$builder.AppendLine("    </g>")
    [void]$builder.AppendLine("</svg>")

    $directory = Split-Path -Parent $OutputPath
    if (-not [string]::IsNullOrWhiteSpace($directory) -and -not (Test-Path -LiteralPath $directory))
    {
        New-Item -ItemType Directory -Path $directory | Out-Null
    }

    $fullOutputPath = $OutputPath
    if (-not [System.IO.Path]::IsPathRooted($OutputPath))
    {
        $fullOutputPath = Join-Path (Get-Location).Path $OutputPath
    }

    [System.IO.File]::WriteAllText($fullOutputPath, $builder.ToString().TrimEnd())
}