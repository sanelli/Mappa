# Shared helpers for Mappa code-coverage history SVG generation.

# Plot area: 80% at y=375, 100% at y=35 (padding above the 100% line).
$script:CoverageChartBottomY = 375.0
$script:CoverageChartTopY = 35.0
$script:CoverageChartMinPercent = 80.0
$script:CoverageChartMaxPercent = 100.0

function Get-CoverageHistoryY
{
    param([double]$Percentage)

    $percentSpan = $script:CoverageChartMaxPercent - $script:CoverageChartMinPercent
    $pixelSpan = $script:CoverageChartBottomY - $script:CoverageChartTopY
    return [Math]::Round(
        $script:CoverageChartBottomY - ($Percentage - $script:CoverageChartMinPercent) * $pixelSpan / $percentSpan,
        2)
}

function Get-CoverageTrendArrow
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
    if ($Current -gt $previousValue)
    {
        return " &#9650;"
    }

    if ($Current -lt $previousValue)
    {
        return " &#9660;"
    }

    return " &#61;"
}

function Get-CoverageLinearTrendLine
{
    param(
        [Parameter(Mandatory = $true)]
        [System.Collections.IList]$Values,

        [double]$StartX = 90.0,

        [double]$EndX = 690.0
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
    if ([Math]::Abs($denominator) -lt 1e-9)
    {
        $meanPercent = $sumY / $n
        $meanY = Get-CoverageHistoryY -Percentage $meanPercent
        return [pscustomobject]@{
            X1 = [Math]::Round($StartX, 2)
            Y1 = $meanY
            X2 = [Math]::Round($EndX, 2)
            Y2 = $meanY
        }
    }

    $slope = (($n * $sumXY) - ($sumX * $sumY)) / $denominator
    $intercept = ($sumY - ($slope * $sumX)) / $n
    $startPercent = $intercept
    $endPercent = $intercept + ($slope * ($n - 1))

    return [pscustomobject]@{
        X1 = [Math]::Round($StartX, 2)
        Y1 = Get-CoverageHistoryY -Percentage $startPercent
        X2 = [Math]::Round($EndX, 2)
        Y2 = Get-CoverageHistoryY -Percentage $endPercent
    }
}

function New-CodeCoverageHistorySvg
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$HistoryMarkdownPath,

        [Parameter(Mandatory = $true)]
        [string]$OutputPath
    )

    if (-not (Test-Path -LiteralPath $HistoryMarkdownPath))
    {
        throw "History markdown file not found: $HistoryMarkdownPath"
    }

    $markdown = ConvertFrom-Markdown -Path $HistoryMarkdownPath
    [xml]$html = $markdown.html
    $rows = @($html.table.tbody.tr)
    $numberOfItems = $rows.Count / 3
    if ($numberOfItems -lt 1)
    {
        throw "No coverage history rows found in $HistoryMarkdownPath"
    }

    $xSpacing = if ($numberOfItems -eq 1) { 0 } else { 600 / ($numberOfItems - 1) }

    $linePoints = New-Object System.Collections.Generic.List[string]
    $branchPoints = New-Object System.Collections.Generic.List[string]
    $methodPoints = New-Object System.Collections.Generic.List[string]
    $lineCircles = New-Object System.Collections.Generic.List[string]
    $branchCircles = New-Object System.Collections.Generic.List[string]
    $methodCircles = New-Object System.Collections.Generic.List[string]
    $xLabels = New-Object System.Collections.Generic.List[string]

    $lineValues = New-Object System.Collections.Generic.List[double]
    $branchValues = New-Object System.Collections.Generic.List[double]
    $methodValues = New-Object System.Collections.Generic.List[double]

    $lineX = 90.0
    $branchX = 90.0
    $methodX = 90.0
    $lineIndex = 0
    $branchIndex = 0
    $methodIndex = 0

    foreach ($row in $rows)
    {
        $cells = @($row.td)
        $version = [string]$cells[1]
        $type = [string]$cells[2]
        $percentage = [double]::Parse([string]$cells[3], [System.Globalization.CultureInfo]::InvariantCulture)
        $y = Get-CoverageHistoryY -Percentage $percentage
        $isMainRelease = -not $version.Contains("-")

        switch ($type)
        {
            "LINE"
            {
                $lineIndex++
                $isLast = $lineIndex -eq $numberOfItems
                $x = [Math]::Round($lineX, 2)
                $linePoints.Add("$x,$y")
                $lineValues.Add($percentage)
                if ($isMainRelease -or $isLast)
                {
                    $lineCircles.Add("<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>")
                }

                if ($isMainRelease)
                {
                    $xLabels.Add("<text x=`"$x`" y=`"400`">$version</text>")
                }

                $lineX += $xSpacing
            }
            "BRANCH"
            {
                $branchIndex++
                $isLast = $branchIndex -eq $numberOfItems
                $x = [Math]::Round($branchX, 2)
                $branchPoints.Add("$x,$y")
                $branchValues.Add($percentage)
                if ($isMainRelease -or $isLast)
                {
                    $branchCircles.Add("<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>")
                }

                $branchX += $xSpacing
            }
            "METHOD"
            {
                $methodIndex++
                $isLast = $methodIndex -eq $numberOfItems
                $x = [Math]::Round($methodX, 2)
                $methodPoints.Add("$x,$y")
                $methodValues.Add($percentage)
                if ($isMainRelease -or $isLast)
                {
                    $methodCircles.Add("<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>")
                }

                $methodX += $xSpacing
            }
        }
    }

    $lastLine = $lineValues[$lineValues.Count - 1]
    $lastBranch = $branchValues[$branchValues.Count - 1]
    $lastMethod = $methodValues[$methodValues.Count - 1]
    $previousLine = if ($lineValues.Count -gt 1) { $lineValues[$lineValues.Count - 2] } else { $null }
    $previousBranch = if ($branchValues.Count -gt 1) { $branchValues[$branchValues.Count - 2] } else { $null }
    $previousMethod = if ($methodValues.Count -gt 1) { $methodValues[$methodValues.Count - 2] } else { $null }

    $lineY = Get-CoverageHistoryY -Percentage $lastLine
    $branchY = Get-CoverageHistoryY -Percentage $lastBranch
    $methodY = Get-CoverageHistoryY -Percentage $lastMethod

    $lineArrow = Get-CoverageTrendArrow -Current $lastLine -Previous $previousLine
    $branchArrow = Get-CoverageTrendArrow -Current $lastBranch -Previous $previousBranch
    $methodArrow = Get-CoverageTrendArrow -Current $lastMethod -Previous $previousMethod

    $lineLabel = ("{0:0.#}%" -f $lastLine) + $lineArrow
    $branchLabel = ("{0:0.#}%" -f $lastBranch) + $branchArrow
    $methodLabel = ("{0:0.#}%" -f $lastMethod) + $methodArrow

    $linePointsText = ($linePoints -join " ")
    $branchPointsText = ($branchPoints -join " ")
    $methodPointsText = ($methodPoints -join " ")
    $lineCirclesText = ($lineCircles -join "")
    $branchCirclesText = ($branchCircles -join "")
    $methodCirclesText = ($methodCircles -join "")
    $xLabelsText = ($xLabels -join "")

    $chartStartX = 90.0
    $chartEndX = if ($numberOfItems -eq 1) { 90.0 } else { 690.0 }
    $lineTrend = Get-CoverageLinearTrendLine -Values $lineValues -StartX $chartStartX -EndX $chartEndX
    $branchTrend = Get-CoverageLinearTrendLine -Values $branchValues -StartX $chartStartX -EndX $chartEndX
    $methodTrend = Get-CoverageLinearTrendLine -Values $methodValues -StartX $chartStartX -EndX $chartEndX

    $lineTrendLine = if ($null -ne $lineTrend) {
        "<line x1=`"$($lineTrend.X1)`" y1=`"$($lineTrend.Y1)`" x2=`"$($lineTrend.X2)`" y2=`"$($lineTrend.Y2)`" style=`"fill:none;stroke-dasharray:6 4;stroke-width:1.5`" />"
    } else { "" }
    $branchTrendLine = if ($null -ne $branchTrend) {
        "<line x1=`"$($branchTrend.X1)`" y1=`"$($branchTrend.Y1)`" x2=`"$($branchTrend.X2)`" y2=`"$($branchTrend.Y2)`" style=`"fill:none;stroke-dasharray:6 4;stroke-width:1.5`" />"
    } else { "" }
    $methodTrendLine = if ($null -ne $methodTrend) {
        "<line x1=`"$($methodTrend.X1)`" y1=`"$($methodTrend.Y1)`" x2=`"$($methodTrend.X2)`" y2=`"$($methodTrend.Y2)`" style=`"fill:none;stroke-dasharray:6 4;stroke-width:1.5`" />"
    } else { "" }

    $y100 = Get-CoverageHistoryY -Percentage 100
    $y975 = Get-CoverageHistoryY -Percentage 97.5
    $y95 = Get-CoverageHistoryY -Percentage 95
    $y925 = Get-CoverageHistoryY -Percentage 92.5
    $y90 = Get-CoverageHistoryY -Percentage 90
    $y875 = Get-CoverageHistoryY -Percentage 87.5
    $y85 = Get-CoverageHistoryY -Percentage 85
    $y825 = Get-CoverageHistoryY -Percentage 82.5
    $y80 = Get-CoverageHistoryY -Percentage 80
    $yAxisTop = 10
    $yLabelMid = [Math]::Round(($y100 + $y80) / 2, 1)

    $svg = @"
<svg version="1.2" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" style="height: 500px; width: 800px;font-family:'Open Sans', sans-serif;background:white" role="img">
    <title id="title">Mappa Code Coverage</title>
    <g style="stroke:#CCCCCC;stroke-width: 2;"><line x1="90" x2="90" y1="$yAxisTop" y2="$y80"></line></g>
    <g style="stroke:#CCCCCC;stroke-width: 2;"><line x1="90" x2="705" y1="$y80" y2="$y80"></line></g>
    <g style="stroke:#DDDDDD;stroke-width: 1;stroke-dasharray: 4 4;" aria-label="Horizontal guides">
        <line x1="90" x2="705" y1="$y100" y2="$y100"></line>
        <line x1="90" x2="705" y1="$y975" y2="$y975"></line>
        <line x1="90" x2="705" y1="$y95" y2="$y95"></line>
        <line x1="90" x2="705" y1="$y925" y2="$y925"></line>
        <line x1="90" x2="705" y1="$y90" y2="$y90"></line>
        <line x1="90" x2="705" y1="$y875" y2="$y875"></line>
        <line x1="90" x2="705" y1="$y85" y2="$y85"></line>
        <line x1="90" x2="705" y1="$y825" y2="$y825"></line>
    </g>
    <g style="text-anchor: middle;font-size: 13px;">
            $xLabelsText
            <text x="400" y="440" style="font-weight: bold;text-transform: uppercase;font-size: 12px;fill: black;">Versions</text>
    </g>
    <g style="text-anchor: end;font-size: 13px;">
        <text x="80" y="$y100" dy="4">100</text><text x="80" y="$y95" dy="4">95</text><text x="80" y="$y90" dy="4">90</text><text x="80" y="$y85" dy="4">85</text><text x="80" y="$y80" dy="4">80</text>
        <text x="50" y="$yLabelMid" style="font-weight: bold;text-transform: uppercase;font-size: 12px;fill: black;">%</text>
    </g>
    <g style="font-size: 12px;" aria-label="Legend">
        <line x1="520" y1="310" x2="550" y2="310" style="stroke:red;stroke-width:2" /><circle cx="535" cy="310" r="4" style="fill:red;stroke:red" /><text x="560" y="314" style="fill:black">Line coverage</text>
        <line x1="520" y1="330" x2="550" y2="330" style="stroke:blue;stroke-width:2" /><circle cx="535" cy="330" r="4" style="fill:blue;stroke:blue" /><text x="560" y="334" style="fill:black">Branch coverage</text>
        <line x1="520" y1="350" x2="550" y2="350" style="stroke:green;stroke-width:2" /><circle cx="535" cy="350" r="4" style="fill:green;stroke:green" /><text x="560" y="354" style="fill:black">Method coverage</text>
    </g>
    <g style="stroke:red; fill:red" data-setname="Line coverage">
        $lineTrendLine
        <polyline points="$linePointsText" style="fill:none" />
        $lineCirclesText
    </g>
    <g style="stroke:blue; fill:blue" data-setname="Branch coverage">
        $branchTrendLine
        <polyline points="$branchPointsText" style="fill:none" />
        $branchCirclesText
    </g>
    <g style="stroke:green; fill:green" data-setname="Method coverage">
        $methodTrendLine
        <polyline points="$methodPointsText" style="fill:none" />
        $methodCirclesText
    </g>
    <g style="font-size: 12px; font-weight: bold;" aria-label="Current coverage">
        <text x="710" y="$lineY" dy="4" style="fill:red;text-anchor:start">$lineLabel</text>
        <text x="710" y="$branchY" dy="4" style="fill:blue;text-anchor:start">$branchLabel</text>
        <text x="710" y="$methodY" dy="4" style="fill:green;text-anchor:start">$methodLabel</text>
    </g>
</svg>
"@

    $outputDirectory = Split-Path -Parent $OutputPath
    if ($outputDirectory -and -not (Test-Path -LiteralPath $outputDirectory))
    {
        New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
    }

    $svg.TrimEnd() | Out-File -FilePath $OutputPath -Encoding utf8
}
