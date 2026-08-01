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

# Pretty labels for grouped-bar SVG X-axis (history/PR keys keep type names).
$script:BenchmarkChartDisplayNames = @{
    "ArrayToListBenchmark"           = "Array To List"
    "DictionaryBenchmark"            = "Dictionary"
    "ListToArrayBenchmark"           = "List To Array"
    "FastListToArrayBenchmark"       = "Fast List To Array"
    "IQueryableProjectionBenchmark"  = "IQueryable Projection"
    "NestedDtoBenchmark"             = "NestedDto"
    "ListToHashSetBenchmark"         = "List To HashSet"
}

function Get-BenchmarkChartDisplayName
{
    param([string]$BenchmarkName)

    if ([string]::IsNullOrWhiteSpace($BenchmarkName))
    {
        return $BenchmarkName
    }

    if ($script:BenchmarkChartDisplayNames.ContainsKey($BenchmarkName))
    {
        return $script:BenchmarkChartDisplayNames[$BenchmarkName]
    }

    return $BenchmarkName
}

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

        [string]$ValueLabelFormat = "0.##",

        [string]$ValueLabelSuffix = "",

        [string[]]$MapperNames = $script:BenchmarkMapperOrder,

        # When set (e.g. 100 for percentage charts), draw that Y guide in red dashed style.
        [Nullable[double]]$EmphasizeGuideAt = $null,

        # When EmphasizeGuideAt is set, bold bar value labels strictly greater than that guide.
        [switch]$BoldValueLabelsAboveGuide
    )

    if ($Benchmarks.Count -eq 0)
    {
        throw "No benchmark rows available to chart."
    }

    if ($YAxisTickStep -le 0)
    {
        throw "YAxisTickStep must be greater than zero."
    }

    $mappers = @($MapperNames)
    if ($mappers.Count -lt 1)
    {
        throw "MapperNames must contain at least one mapper."
    }

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

        $isEmphasizedGuide = ($null -ne $EmphasizeGuideAt) -and ([Math]::Abs($value - [double]$EmphasizeGuideAt) -lt 1e-9)
        if ($isEmphasizedGuide)
        {
            [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$([Math]::Round($y, 2))`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$([Math]::Round($y, 2))`" stroke=`"#cc0000`" stroke-width=`"1.5`" stroke-dasharray=`"6 4`"/>")
            [void]$builder.AppendLine("  <text x=`"$([Math]::Round($leftMargin - 8, 2))`" y=`"$([Math]::Round($y + 4, 2))`" text-anchor=`"end`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"11`" fill=`"#cc0000`">$label</text>")
        }
        else
        {
            [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$([Math]::Round($y, 2))`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$([Math]::Round($y, 2))`" stroke=`"#dddddd`" stroke-width=`"1`"/>")
            [void]$builder.AppendLine("  <text x=`"$([Math]::Round($leftMargin - 8, 2))`" y=`"$([Math]::Round($y + 4, 2))`" text-anchor=`"end`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"11`" fill=`"#555555`">$label</text>")
        }
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

            $valueText = $metric.ToString($ValueLabelFormat, [System.Globalization.CultureInfo]::InvariantCulture) + $ValueLabelSuffix
            $textX = $x + ($barWidth / 2.0)
            $textY = $y - 4.0
            $fontWeightAttr = ""
            if ($BoldValueLabelsAboveGuide -and ($null -ne $EmphasizeGuideAt) -and ($metric -gt [double]$EmphasizeGuideAt))
            {
                $fontWeightAttr = " font-weight=`"bold`""
            }

            # Always place the value vertically above the bar, using the bar color.
            [void]$builder.AppendLine("  <text x=`"$([Math]::Round($textX, 2))`" y=`"$([Math]::Round($textY, 2))`" text-anchor=`"start`" dominant-baseline=`"middle`" transform=`"rotate(-90 $([Math]::Round($textX, 2)) $([Math]::Round($textY, 2)))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"9`"$fontWeightAttr fill=`"$color`">$valueText</text>")
        }

        $labelX = $groupStartX + ($groupWidth / 2.0)
        $labelY = $topMargin + $plotHeight + 16
        $displayName = Get-BenchmarkChartDisplayName -BenchmarkName $benchmark.Name
        $escapedName = [System.Security.SecurityElement]::Escape($displayName)
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