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
    "ListToArrayBenchmark",
    "FastListToArrayBenchmark",
    "DictionaryBenchmark",
    "NestedDtoBenchmark",
    "IQueryableProjectionBenchmark",
    "StringToEnumBenchmark",
    "EnumToStringBenchmark",
    "ReferenceReusingSharedDagBenchmark"
)

# Absolute and percentage memory charts omit these (near-zero / undefined vs Mappa).
$script:BenchmarkMemoryChartExcludedNames = @(
    "StringToEnumBenchmark",
    "EnumToStringBenchmark"
)

# Pretty labels for grouped-bar SVG X-axis (history/PR keys keep type names).
$script:BenchmarkChartDisplayNames = @{
    "ArrayToListBenchmark"               = "Array To List"
    "ListToArrayBenchmark"               = "List To Array"
    "FastListToArrayBenchmark"           = "Fast List To Array"
    "DictionaryBenchmark"                = "Dictionary"
    "NestedDtoBenchmark"                 = "Objects mapping"
    "IQueryableProjectionBenchmark"      = "IQueryable Projection"
    "StringToEnumBenchmark"              = "String To Enum"
    "EnumToStringBenchmark"              = "Enum To String"
    "ReferenceReusingSharedDagBenchmark" = "Reference Reusing"
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

function Format-BenchmarkChartNumber
{
    param(
        [double]$Value,
        [string]$Format = "0.##"
    )

    $formatted = $Value.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
    if ([Math]::Abs($Value) -lt 1000.0)
    {
        return $formatted
    }

    $sign = ""
    $body = $formatted
    if ($body.StartsWith("-"))
    {
        $sign = "-"
        $body = $body.Substring(1)
    }

    $integerPart = $body
    $fractionPart = ""
    $dotIndex = $body.IndexOf('.')
    if ($dotIndex -ge 0)
    {
        $integerPart = $body.Substring(0, $dotIndex)
        $fractionPart = $body.Substring($dotIndex)
    }

    # Insert a comma as thousands separator (e.g. 11566.8 -> 11,566.8).
    $chars = $integerPart.ToCharArray()
    $grouped = New-Object System.Text.StringBuilder
    $digitCount = 0
    for ($i = $chars.Length - 1; $i -ge 0; $i--)
    {
        if ($digitCount -gt 0 -and (($digitCount % 3) -eq 0))
        {
            [void]$grouped.Insert(0, ',')
        }

        [void]$grouped.Insert(0, $chars[$i])
        $digitCount++
    }

    return $sign + $grouped.ToString() + $fractionPart
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
        [switch]$BoldValueLabelsAboveGuide,

        # When set, force the Y-axis maximum (overflowing bars use a broken-top stub).
        [Nullable[double]]$YAxisMax = $null,

        # When set with YAxisMax, continuous scale / break line ends here; YAxisMax is labeled at the overflow top.
        [Nullable[double]]$YAxisBreakAt = $null,

        # When set with overflow stubs, scale stub height by overflow amount (max keeps OverflowStubMaxHeight).
        [switch]$ProportionalOverflowStubs,

        # Squiggle style for broken bars: sharp zigzag (default) or rounded waves.
        [ValidateSet("Zigzag", "RoundedWave")]
        [string]$BreakLineStyle = "Zigzag",

        # Draw unlabeled lighter dashed guides halfway between each major Y tick.
        [switch]$DrawMinorYAxisGuides
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

    $overflowStubMaxHeight = 18.0
    $overflowStubMinHeight = 4.0
    $breakGap = 8.0
    $useAxisCap = ($null -ne $YAxisMax) -and ([double]$YAxisMax -gt 0)
    $useAxisBreak = ($null -ne $YAxisBreakAt) -and ([double]$YAxisBreakAt -gt 0)
    if ($useAxisBreak -and -not $useAxisCap)
    {
        throw "YAxisBreakAt requires YAxisMax."
    }

    if ($useAxisBreak -and ([double]$YAxisBreakAt -ge [double]$YAxisMax))
    {
        throw "YAxisBreakAt must be less than YAxisMax."
    }

    # Continuous scale ends at the break (when set) or at YAxisMax / auto max.
    if ($useAxisBreak)
    {
        $axisMax = [double]$YAxisBreakAt
        $tickCount = [int][Math]::Ceiling($axisMax / $YAxisTickStep)
        if ($tickCount -lt 1)
        {
            $tickCount = 1
        }

        $axisMax = $tickCount * $YAxisTickStep
        if ($axisMax -ge [double]$YAxisMax)
        {
            throw "YAxisBreakAt snapped to $axisMax which is not below YAxisMax ($([double]$YAxisMax)). Adjust YAxisTickStep."
        }

        $chartMax = [double]$YAxisMax
    }
    elseif ($useAxisCap)
    {
        $axisMax = [double]$YAxisMax
        $tickCount = [int][Math]::Ceiling($axisMax / $YAxisTickStep)
        if ($tickCount -lt 1)
        {
            $tickCount = 1
        }

        $axisMax = $tickCount * $YAxisTickStep
        $chartMax = $axisMax
    }
    else
    {
        $tickCount = [int][Math]::Ceiling($maxValue / $YAxisTickStep)
        if ($tickCount -lt 1)
        {
            $tickCount = 1
        }

        $axisMax = $tickCount * $YAxisTickStep
        $chartMax = $axisMax
    }

    $overflowThreshold = $axisMax
    $hasOverflow = ($useAxisCap -or $useAxisBreak) -and ($maxValue -gt $overflowThreshold)
    $scaleHeight = if ($hasOverflow) { $plotHeight - $overflowStubMaxHeight - $breakGap } else { $plotHeight }

    $maxOverflowAmount = 0.0
    if ($hasOverflow)
    {
        $overflowCeiling = [Math]::Max($maxValue, $chartMax)
        $maxOverflowAmount = $overflowCeiling - $overflowThreshold
        if ($maxOverflowAmount -lt 0)
        {
            $maxOverflowAmount = 0.0
        }
    }

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine('<?xml version="1.0" encoding="UTF-8" standalone="no"?>')
    [void]$builder.AppendLine("<svg xmlns=`"http://www.w3.org/2000/svg`" width=`"$svgWidth`" height=`"$svgHeight`" viewBox=`"0 0 $svgWidth $svgHeight`">")
    [void]$builder.AppendLine("  <rect width=`"100%`" height=`"100%`" fill=`"#ffffff`"/>")
    [void]$builder.AppendLine("  <text x=`"$([Math]::Round($svgWidth / 2, 1))`" y=`"28`" text-anchor=`"middle`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"18`" fill=`"#222222`">$Title</text>")
    [void]$builder.AppendLine("  <text x=`"20`" y=`"$([Math]::Round($topMargin + ($plotHeight / 2), 1))`" text-anchor=`"middle`" transform=`"rotate(-90 20 $([Math]::Round($topMargin + ($plotHeight / 2), 1)))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" fill=`"#444444`">$YAxisLabel</text>")

    $overflowYOffset = if ($hasOverflow) { $overflowStubMaxHeight + $breakGap } else { 0.0 }

    if ($DrawMinorYAxisGuides)
    {
        for ($tickIndex = 0; $tickIndex -lt $tickCount; $tickIndex++)
        {
            $minorValue = ($tickIndex + 0.5) * $YAxisTickStep
            if (($null -ne $EmphasizeGuideAt) -and ([Math]::Abs($minorValue - [double]$EmphasizeGuideAt) -lt 1e-9))
            {
                continue
            }

            $minorRatio = 1.0 - ($minorValue / $axisMax)
            $minorY = $topMargin + ($scaleHeight * $minorRatio) + $overflowYOffset
            [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$([Math]::Round($minorY, 2))`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$([Math]::Round($minorY, 2))`" stroke=`"#eeeeee`" stroke-width=`"1`" stroke-dasharray=`"4 4`"/>")
        }
    }

    for ($tickIndex = 0; $tickIndex -le $tickCount; $tickIndex++)
    {
        $value = $tickIndex * $YAxisTickStep
        $ratio = 1.0 - ($value / $axisMax)
        $y = $topMargin + ($scaleHeight * $ratio) + $overflowYOffset
        if ($YAxisTickStep -ge 1.0)
        {
            $label = Format-BenchmarkChartNumber -Value $value -Format "0"
        }
        else
        {
            $label = Format-BenchmarkChartNumber -Value $value -Format "0.##"
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

    # When the continuous scale breaks below YAxisMax, label the overflow top with the chart max.
    if ($hasOverflow -and $useAxisBreak -and ($chartMax -gt $axisMax))
    {
        $topLabel = if ($YAxisTickStep -ge 1.0) {
            Format-BenchmarkChartNumber -Value $chartMax -Format "0"
        } else {
            Format-BenchmarkChartNumber -Value $chartMax -Format "0.##"
        }

        [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$topMargin`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$topMargin`" stroke=`"#dddddd`" stroke-width=`"1`"/>")
        [void]$builder.AppendLine("  <text x=`"$([Math]::Round($leftMargin - 8, 2))`" y=`"$([Math]::Round($topMargin + 4, 2))`" text-anchor=`"end`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"11`" fill=`"#555555`">$topLabel</text>")
    }

    # Draw EmphasizeGuideAt even when it is not on a tick (e.g. 100% with 500-step ticks).
    if (($null -ne $EmphasizeGuideAt) -and ([double]$EmphasizeGuideAt -ge 0) -and ([double]$EmphasizeGuideAt -le $axisMax))
    {
        $guideOnTick = $false
        for ($tickIndex = 0; $tickIndex -le $tickCount; $tickIndex++)
        {
            if ([Math]::Abs(($tickIndex * $YAxisTickStep) - [double]$EmphasizeGuideAt) -lt 1e-9)
            {
                $guideOnTick = $true
                break
            }
        }

        if (-not $guideOnTick)
        {
            $guideRatio = 1.0 - ([double]$EmphasizeGuideAt / $axisMax)
            $guideY = $topMargin + ($scaleHeight * $guideRatio) + $overflowYOffset
            $guideLabel = if ($YAxisTickStep -ge 1.0) {
                Format-BenchmarkChartNumber -Value ([double]$EmphasizeGuideAt) -Format "0"
            } else {
                Format-BenchmarkChartNumber -Value ([double]$EmphasizeGuideAt) -Format "0.##"
            }
            [void]$builder.AppendLine("  <line x1=`"$leftMargin`" y1=`"$([Math]::Round($guideY, 2))`" x2=`"$([Math]::Round($leftMargin + $plotWidth, 2))`" y2=`"$([Math]::Round($guideY, 2))`" stroke=`"#cc0000`" stroke-width=`"1.5`" stroke-dasharray=`"6 4`"/>")
            [void]$builder.AppendLine("  <text x=`"$([Math]::Round($leftMargin - 8, 2))`" y=`"$([Math]::Round($guideY + 4, 2))`" text-anchor=`"end`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"11`" fill=`"#cc0000`">$guideLabel</text>")
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
            $x = $groupStartX + ($seriesIndex * $barWidth)
            $color = $script:BenchmarkMapperColors[$mapper]
            $plotBottom = $topMargin + $plotHeight
            $isOverflowBar = $hasOverflow -and ($metric -gt $axisMax)

            if ($isOverflowBar)
            {
                $scaleTop = $topMargin + $overflowStubMaxHeight + $breakGap
                $bottomBarHeight = $scaleHeight
                $bottomBarY = $plotBottom - $bottomBarHeight
                [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($x, 2))`" y=`"$([Math]::Round($bottomBarY, 2))`" width=`"$barWidth`" height=`"$([Math]::Round($bottomBarHeight, 2))`" fill=`"$color`"/>")

                $overflowAmount = $metric - $axisMax
                if ($ProportionalOverflowStubs -and ($maxOverflowAmount -gt 0))
                {
                    $stubHeight = $overflowStubMaxHeight * ($overflowAmount / $maxOverflowAmount)
                    if ($stubHeight -lt $overflowStubMinHeight)
                    {
                        $stubHeight = $overflowStubMinHeight
                    }
                }
                else
                {
                    $stubHeight = $overflowStubMaxHeight
                }

                # Bottom-align stub to the break gap so max overflow still reaches chart top.
                $gapY = $topMargin + $overflowStubMaxHeight
                $stubY = $gapY - $stubHeight
                [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($x, 2))`" y=`"$([Math]::Round($stubY, 2))`" width=`"$barWidth`" height=`"$([Math]::Round($stubHeight, 2))`" fill=`"$color`"/>")

                # White gap between stub and main bar, with squiggly edges on both faces.
                [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($x, 2))`" y=`"$([Math]::Round($gapY, 2))`" width=`"$barWidth`" height=`"$([Math]::Round($breakGap, 2))`" fill=`"#ffffff`"/>")
                if ($BreakLineStyle -eq "RoundedWave")
                {
                    $waveBottom = Get-BenchmarkBarBreakWavePath -X $x -Y $scaleTop -Width $barWidth -Amplitude 2.5 -FillColor $color -BackgroundColor "#ffffff"
                    $waveTop = Get-BenchmarkBarBreakWavePath -X $x -Y $gapY -Width $barWidth -Amplitude 2.5 -FillColor $color -BackgroundColor "#ffffff" -Flip
                    [void]$builder.AppendLine($waveBottom)
                    [void]$builder.AppendLine($waveTop)
                }
                else
                {
                    $zigBottom = Get-BenchmarkBarBreakZigzagPath -X $x -Y $scaleTop -Width $barWidth -Amplitude 2.5 -FillColor $color -BackgroundColor "#ffffff"
                    $zigTop = Get-BenchmarkBarBreakZigzagPath -X $x -Y $gapY -Width $barWidth -Amplitude 2.5 -FillColor $color -BackgroundColor "#ffffff" -Flip
                    [void]$builder.AppendLine($zigBottom)
                    [void]$builder.AppendLine($zigTop)
                }

                # Overflow bars always put the label inside the main rectangle, just below the break.
                $labelInside = $true
                $labelAnchorY = $scaleTop
            }
            else
            {
                $barHeight = ($metric / $axisMax) * $scaleHeight
                $y = $plotBottom - $barHeight
                [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($x, 2))`" y=`"$([Math]::Round($y, 2))`" width=`"$barWidth`" height=`"$([Math]::Round($barHeight, 2))`" fill=`"$color`"/>")
                $labelInside = ($axisMax -gt 0) -and (($metric / $axisMax) -ge 0.95)
                $labelAnchorY = $y
            }

            $valueText = (Format-BenchmarkChartNumber -Value $metric -Format $ValueLabelFormat) + $ValueLabelSuffix
            $textX = $x + ($barWidth / 2.0)
            $fontWeightAttr = ""
            if ($BoldValueLabelsAboveGuide -and ($null -ne $EmphasizeGuideAt) -and ($metric -gt [double]$EmphasizeGuideAt))
            {
                $fontWeightAttr = " font-weight=`"bold`""
            }

            if ($labelInside)
            {
                # text-anchor=end with -90 rotation grows downward into the bar from the pivot.
                $textY = $labelAnchorY + 6.0
                $textFill = "#ffffff"
                $textAnchor = "end"
            }
            else
            {
                $textY = $labelAnchorY - 4.0
                $textFill = $color
                $textAnchor = "start"
            }

            [void]$builder.AppendLine("  <text x=`"$([Math]::Round($textX, 2))`" y=`"$([Math]::Round($textY, 2))`" text-anchor=`"$textAnchor`" dominant-baseline=`"middle`" transform=`"rotate(-90 $([Math]::Round($textX, 2)) $([Math]::Round($textY, 2)))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"9`"$fontWeightAttr fill=`"$textFill`">$valueText</text>")
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

function Get-BenchmarkBarBreakZigzagPath
{
    param(
        [double]$X,
        [double]$Y,
        [double]$Width,
        [double]$Amplitude,
        [string]$FillColor,
        [string]$BackgroundColor,
        [switch]$Flip
    )

    $segments = 6
    $step = $Width / $segments
    $points = New-Object System.Collections.Generic.List[string]
    for ($i = 0; $i -le $segments; $i++)
    {
        $px = $X + ($i * $step)
        $sign = if (($i % 2) -eq 0) { 1.0 } else { -1.0 }
        if ($Flip)
        {
            $sign = -$sign
        }

        $py = $Y + ($sign * $Amplitude)
        [void]$points.Add("$([Math]::Round($px, 2)),$([Math]::Round($py, 2))")
    }

    $poly = ($points -join " ")
    return "  <polyline points=`"$poly`" fill=`"none`" stroke=`"$BackgroundColor`" stroke-width=`"5`" stroke-linejoin=`"round`"/>`n  <polyline points=`"$poly`" fill=`"none`" stroke=`"$FillColor`" stroke-width=`"1.5`" stroke-linejoin=`"round`"/>"
}

function Get-BenchmarkBarBreakWavePath
{
    param(
        [double]$X,
        [double]$Y,
        [double]$Width,
        [double]$Amplitude,
        [string]$FillColor,
        [string]$BackgroundColor,
        [switch]$Flip,
        [int]$WaveCount = 2
    )

    if ($WaveCount -lt 1)
    {
        $WaveCount = 1
    }

    $sign = if ($Flip) { -1.0 } else { 1.0 }
    # Two full sine waves via cubic Bezier segments (one half-wave per cubic).
    $halfWaves = $WaveCount * 2
    $segmentWidth = $Width / $halfWaves
    # Control offset approximates a sine half-period.
    $controlOffset = $segmentWidth * 0.45

    $d = "M $([Math]::Round($X, 2)) $([Math]::Round($Y, 2))"
    for ($i = 0; $i -lt $halfWaves; $i++)
    {
        $x0 = $X + ($i * $segmentWidth)
        $x1 = $x0 + $segmentWidth
        $peakSign = if (($i % 2) -eq 0) { $sign } else { -$sign }
        $peakY = $Y + ($peakSign * $Amplitude)
        $c1x = $x0 + $controlOffset
        $c2x = $x1 - $controlOffset
        $d += " C $([Math]::Round($c1x, 2)) $([Math]::Round($peakY, 2)), $([Math]::Round($c2x, 2)) $([Math]::Round($peakY, 2)), $([Math]::Round($x1, 2)) $([Math]::Round($Y, 2))"
    }

    return "  <path d=`"$d`" fill=`"none`" stroke=`"$BackgroundColor`" stroke-width=`"5`" stroke-linecap=`"round`" stroke-linejoin=`"round`"/>`n  <path d=`"$d`" fill=`"none`" stroke=`"$FillColor`" stroke-width=`"1.5`" stroke-linecap=`"round`" stroke-linejoin=`"round`"/>"
}

function Get-BenchmarkBestMappers
{
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Mappers,

        [Parameter(Mandatory = $true)]
        [string]$MetricProperty,

        [string[]]$MapperOrder = $script:BenchmarkMapperOrder
    )

    $bestMappers = [System.Collections.Generic.List[string]]::new()
    $bestValue = $null
    foreach ($mapper in $MapperOrder)
    {
        if (-not $Mappers.ContainsKey($mapper))
        {
            continue
        }

        $entry = $Mappers[$mapper]
        if ($null -eq $entry)
        {
            continue
        }

        $value = $entry.$MetricProperty
        if ($null -eq $value)
        {
            continue
        }

        $numeric = [double]$value
        if ($null -eq $bestValue)
        {
            $bestValue = $numeric
            $bestMappers.Clear()
            $bestMappers.Add($mapper) | Out-Null
            continue
        }

        if ($numeric -lt $bestValue)
        {
            $bestValue = $numeric
            $bestMappers.Clear()
            $bestMappers.Add($mapper) | Out-Null
            continue
        }

        if ($numeric -eq $bestValue)
        {
            $bestMappers.Add($mapper) | Out-Null
        }
    }

    return [pscustomobject]@{
        Mappers = @($bestMappers.ToArray())
        BestValue = $bestValue
    }
}

function Format-BenchmarkWinnerDeltaPercent
{
    param(
        [Nullable[double]]$BestValue,
        [Nullable[double]]$MappaValue
    )

    if (($null -eq $BestValue) -or ($null -eq $MappaValue) -or ($MappaValue -eq 0.0))
    {
        return $null
    }

    $delta = [Math]::Abs((($BestValue / $MappaValue) - 1.0) * 100.0)
    return $delta.ToString("0.#", [System.Globalization.CultureInfo]::InvariantCulture) + "%"
}

function Format-BenchmarkSecondBestDeltaPercent
{
    param(
        [Nullable[double]]$MappaValue,
        [Nullable[double]]$SecondBestValue
    )

    if (($null -eq $MappaValue) -or ($null -eq $SecondBestValue) -or ($MappaValue -eq 0.0))
    {
        return $null
    }

    $delta = [Math]::Abs((($SecondBestValue / $MappaValue) - 1.0) * 100.0)
    return $delta.ToString("0.#", [System.Globalization.CultureInfo]::InvariantCulture) + "%"
}

function Get-BenchmarkMappersWithMappaFirst
{
    param([string[]]$Mappers)

    if (($null -eq $Mappers) -or ($Mappers.Count -eq 0))
    {
        return @()
    }

    if (($Mappers -contains "Mappa") -and ($Mappers[0] -ne "Mappa"))
    {
        return @("Mappa") + @($Mappers | Where-Object { $_ -ne "Mappa" })
    }

    return @($Mappers)
}

function Get-BenchmarkWinnerCell
{
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Mappers,

        [Parameter(Mandatory = $true)]
        [string]$MetricProperty,

        [switch]$Unavailable
    )

    if ($Unavailable)
    {
        return [pscustomobject]@{
            Mappers = @()
            Label = "n/a"
            ShowDelta = $false
            DeltaPercent = $null
            SecondBestMapper = $null
        }
    }

    $best = Get-BenchmarkBestMappers -Mappers $Mappers -MetricProperty $MetricProperty
    if (($null -eq $best.BestValue) -or ($best.Mappers.Count -eq 0))
    {
        return [pscustomobject]@{
            Mappers = @()
            Label = "n/a"
            ShowDelta = $false
            DeltaPercent = $null
            SecondBestMapper = $null
        }
    }

    $orderedWinners = @(Get-BenchmarkMappersWithMappaFirst -Mappers $best.Mappers)
    $includeMappa = $orderedWinners -contains "Mappa"
    $mappaIsSoleWinner = $includeMappa -and ($orderedWinners.Count -eq 1)
    $showDelta = $false
    $deltaPercent = $null
    $secondBestMapper = $null

    if ($mappaIsSoleWinner)
    {
        $otherMappers = @{}
        foreach ($mapperName in $Mappers.Keys)
        {
            if ($mapperName -ne "Mappa")
            {
                $otherMappers[$mapperName] = $Mappers[$mapperName]
            }
        }

        $secondBest = Get-BenchmarkBestMappers -Mappers $otherMappers -MetricProperty $MetricProperty
        $mappaEntry = $Mappers["Mappa"]
        $mappaValue = if ($null -eq $mappaEntry) { $null } else { $mappaEntry.$MetricProperty }
        if (($null -ne $secondBest.BestValue) -and
            ($secondBest.Mappers.Count -gt 0) -and
            ($null -ne $mappaValue) -and
            ([double]$secondBest.BestValue -ne [double]$mappaValue))
        {
            $showDelta = $true
            $secondBestMapper = [string]$secondBest.Mappers[0]
            $deltaPercent = Format-BenchmarkSecondBestDeltaPercent -MappaValue $mappaValue -SecondBestValue $secondBest.BestValue
        }
    }
    elseif (-not $includeMappa)
    {
        $showDelta = $true
        $mappaEntry = $Mappers["Mappa"]
        $mappaValue = if ($null -eq $mappaEntry) { $null } else { $mappaEntry.$MetricProperty }
        $deltaPercent = Format-BenchmarkWinnerDeltaPercent -BestValue $best.BestValue -MappaValue $mappaValue
        $secondBestMapper = "Mappa"
    }

    $label = ($orderedWinners -join ", ")
    if ($showDelta -and (-not [string]::IsNullOrWhiteSpace($deltaPercent)) -and (-not [string]::IsNullOrWhiteSpace($secondBestMapper)))
    {
        $label = "$label ($deltaPercent vs $secondBestMapper)"
    }

    return [pscustomobject]@{
        Mappers = $orderedWinners
        Label = $label
        ShowDelta = $showDelta
        DeltaPercent = $deltaPercent
        SecondBestMapper = $secondBestMapper
    }
}

function Get-BenchmarkComparisonRows
{
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Benchmarks,

        [string[]]$BenchmarkNames = $script:BenchmarkChartNames
    )

    $rows = [System.Collections.Generic.List[object]]::new()
    foreach ($name in $BenchmarkNames)
    {
        $match = @($Benchmarks | Where-Object { $_.Name -eq $name } | Select-Object -First 1)
        if ($match.Count -eq 0 -or $null -eq $match[0])
        {
            continue
        }

        $benchmark = $match[0]
        $bestTime = Get-BenchmarkWinnerCell -Mappers $benchmark.Mappers -MetricProperty "MeanNs"
        if ($script:BenchmarkMemoryChartExcludedNames -contains $name)
        {
            $bestMemory = Get-BenchmarkWinnerCell -Mappers $benchmark.Mappers -MetricProperty "AllocatedBytes" -Unavailable
        }
        else
        {
            $bestMemory = Get-BenchmarkWinnerCell -Mappers $benchmark.Mappers -MetricProperty "AllocatedBytes"
        }

        $rows.Add([pscustomobject]@{
                Name = $name
                DisplayName = (Get-BenchmarkChartDisplayName -BenchmarkName $name)
                BestTime = $bestTime
                BestMemory = $bestMemory
                BestTimeLabel = $bestTime.Label
                BestMemoryLabel = $bestMemory.Label
            }) | Out-Null
    }

    return $rows.ToArray()
}

function Add-BenchmarkWinnerSvgText
{
    param(
        [Parameter(Mandatory = $true)]
        [System.Text.StringBuilder]$Builder,

        [Parameter(Mandatory = $true)]
        [double]$X,

        [Parameter(Mandatory = $true)]
        [double]$Y,

        [Parameter(Mandatory = $true)]
        $WinnerCell
    )

    if (($null -eq $WinnerCell) -or ($WinnerCell.Mappers.Count -eq 0))
    {
        $label = [System.Security.SecurityElement]::Escape([string]$WinnerCell.Label)
        [void]$Builder.AppendLine("  <text x=`"$([Math]::Round($X, 2))`" y=`"$([Math]::Round($Y, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" fill=`"#555555`">$label</text>")
        return
    }

    [void]$Builder.Append("  <text x=`"$([Math]::Round($X, 2))`" y=`"$([Math]::Round($Y, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`">")
    for ($i = 0; $i -lt $WinnerCell.Mappers.Count; $i++)
    {
        if ($i -gt 0)
        {
            [void]$Builder.Append('<tspan fill="#555555">, </tspan>')
        }

        $mapper = [string]$WinnerCell.Mappers[$i]
        $color = "#555555"
        if ($script:BenchmarkMapperColors.ContainsKey($mapper))
        {
            $color = $script:BenchmarkMapperColors[$mapper]
        }

        $escapedMapper = [System.Security.SecurityElement]::Escape($mapper)
        [void]$Builder.Append("<tspan font-weight=`"bold`" fill=`"$color`">$escapedMapper</tspan>")
    }

    if ($WinnerCell.ShowDelta -and
        (-not [string]::IsNullOrWhiteSpace([string]$WinnerCell.DeltaPercent)) -and
        (-not [string]::IsNullOrWhiteSpace([string]$WinnerCell.SecondBestMapper)))
    {
        $vsMapper = [string]$WinnerCell.SecondBestMapper
        $vsColor = "#555555"
        if ($script:BenchmarkMapperColors.ContainsKey($vsMapper))
        {
            $vsColor = $script:BenchmarkMapperColors[$vsMapper]
        }

        $escapedPrefix = [System.Security.SecurityElement]::Escape(" ($($WinnerCell.DeltaPercent) vs ")
        $escapedVsMapper = [System.Security.SecurityElement]::Escape($vsMapper)
        [void]$Builder.Append("<tspan font-size=`"9`" fill=`"#555555`">$escapedPrefix</tspan>")
        [void]$Builder.Append("<tspan font-size=`"9`" font-weight=`"bold`" fill=`"$vsColor`">$escapedVsMapper</tspan>")
        [void]$Builder.Append('<tspan font-size="9" fill="#555555">)</tspan>')
    }

    [void]$Builder.AppendLine("</text>")
}

function New-BenchmarkComparisonSvg
{
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Rows,

        [Parameter(Mandatory = $true)]
        [string]$OutputPath,

        [string]$Title = "Benchmark winners"
    )

    $leftMargin = 16.0
    $rightMargin = 16.0
    $topMargin = 44.0
    $bottomMargin = 16.0
    $rowHeight = 26.0
    $headerHeight = 28.0
    $colBenchmarkWidth = 150.0
    $colTimeWidth = 220.0
    $colMemoryWidth = 220.0
    $tableWidth = $colBenchmarkWidth + $colTimeWidth + $colMemoryWidth
    $width = $leftMargin + $tableWidth + $rightMargin
    $tableBottom = $topMargin + $headerHeight + ($Rows.Count * $rowHeight)
    $height = $tableBottom + $bottomMargin
    $dividerColor = "#e0e0e0"

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine('<?xml version="1.0" encoding="UTF-8" standalone="no"?>')
    [void]$builder.AppendLine("<svg xmlns=`"http://www.w3.org/2000/svg`" width=`"$([Math]::Round($width, 2))`" height=`"$([Math]::Round($height, 2))`" viewBox=`"0 0 $([Math]::Round($width, 2)) $([Math]::Round($height, 2))`">")
    [void]$builder.AppendLine('  <rect width="100%" height="100%" fill="#ffffff"/>')
    [void]$builder.AppendLine("  <text x=`"$([Math]::Round($width / 2.0, 2))`" y=`"26`" text-anchor=`"middle`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"16`" fill=`"#222222`">$Title</text>")

    $tableX = $leftMargin
    $headerY = $topMargin
    [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($tableX, 2))`" y=`"$([Math]::Round($headerY, 2))`" width=`"$([Math]::Round($tableWidth, 2))`" height=`"$([Math]::Round($headerHeight, 2))`" fill=`"#f3f3f3`" stroke=`"#cccccc`" stroke-width=`"1`"/>")

    $headerTextY = $headerY + 18.0
    $col1X = $tableX + 8.0
    $col2X = $tableX + $colBenchmarkWidth + 8.0
    $col3X = $tableX + $colBenchmarkWidth + $colTimeWidth + 8.0
    [void]$builder.AppendLine("  <text x=`"$([Math]::Round($col1X, 2))`" y=`"$([Math]::Round($headerTextY, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" font-weight=`"bold`" fill=`"#333333`">Benchmark</text>")
    [void]$builder.AppendLine("  <text x=`"$([Math]::Round($col2X, 2))`" y=`"$([Math]::Round($headerTextY, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" font-weight=`"bold`" fill=`"#333333`">Best time</text>")
    [void]$builder.AppendLine("  <text x=`"$([Math]::Round($col3X, 2))`" y=`"$([Math]::Round($headerTextY, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" font-weight=`"bold`" fill=`"#333333`">Best memory</text>")

    $rowIndex = 0
    foreach ($row in $Rows)
    {
        $rowY = $topMargin + $headerHeight + ($rowIndex * $rowHeight)
        $fill = if (($rowIndex % 2) -eq 0) { "#ffffff" } else { "#fafafa" }
        [void]$builder.AppendLine("  <rect x=`"$([Math]::Round($tableX, 2))`" y=`"$([Math]::Round($rowY, 2))`" width=`"$([Math]::Round($tableWidth, 2))`" height=`"$([Math]::Round($rowHeight, 2))`" fill=`"$fill`" stroke=`"#dddddd`" stroke-width=`"1`"/>")

        $textY = $rowY + 17.0
        $displayName = [System.Security.SecurityElement]::Escape([string]$row.DisplayName)
        [void]$builder.AppendLine("  <text x=`"$([Math]::Round($col1X, 2))`" y=`"$([Math]::Round($textY, 2))`" font-family=`"Segoe UI, Arial, sans-serif`" font-size=`"12`" fill=`"#222222`">$displayName</text>")

        Add-BenchmarkWinnerSvgText -Builder $builder -X $col2X -Y $textY -WinnerCell $row.BestTime
        Add-BenchmarkWinnerSvgText -Builder $builder -X $col3X -Y $textY -WinnerCell $row.BestMemory

        $rowIndex++
    }

    $divider1X = $tableX + $colBenchmarkWidth
    $divider2X = $tableX + $colBenchmarkWidth + $colTimeWidth
    [void]$builder.AppendLine("  <line x1=`"$([Math]::Round($divider1X, 2))`" y1=`"$([Math]::Round($topMargin, 2))`" x2=`"$([Math]::Round($divider1X, 2))`" y2=`"$([Math]::Round($tableBottom, 2))`" stroke=`"$dividerColor`" stroke-width=`"1`"/>")
    [void]$builder.AppendLine("  <line x1=`"$([Math]::Round($divider2X, 2))`" y1=`"$([Math]::Round($topMargin, 2))`" x2=`"$([Math]::Round($divider2X, 2))`" y2=`"$([Math]::Round($tableBottom, 2))`" stroke=`"$dividerColor`" stroke-width=`"1`"/>")

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

function Write-BenchmarkComparisonMarkdown
{
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Rows,

        [Parameter(Mandatory = $true)]
        [string]$OutputPath
    )

    $builder = New-Object System.Text.StringBuilder
    [void]$builder.AppendLine("# Benchmark comparison")
    [void]$builder.AppendLine()
    [void]$builder.AppendLine("| Benchmark | Best time | Best memory |")
    [void]$builder.AppendLine("| --- | --- | --- |")
    foreach ($row in $Rows)
    {
        [void]$builder.AppendLine("| $($row.DisplayName) | $($row.BestTimeLabel) | $($row.BestMemoryLabel) |")
    }

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

    [System.IO.File]::WriteAllText($fullOutputPath, $builder.ToString().TrimEnd() + "`n")
}
