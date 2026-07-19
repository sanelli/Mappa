param([switch]$DryRun)

$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"

function Out-Svg
{
    $markdown = ConvertFrom-Markdown "./$MappaTestsAndCoveragePath/full-history-table.md"
    [xml]$html = $markdown.html
    $numberOfItems = $html.table.tbody.tr.td.Length / (3 * 4)

    $xSpacing = 600 / ($numberOfItems - 1);

    # Generate line coverage (and text version)
    $LineCoveragePoints = ""
    $LineCoverageCircles = ""
    $xLabels = ""
    $x = 90
    foreach ($line in $html.table.tbody.tr)
    {
        $line = $line.td
        if ($line[2] -eq "LINE")
        {
            # Plot every version (main + intermediate); label only main releases on the X axis.
            if (-not ($line[1].Contains("-")))
            {
                $xLabels = "$xLabels<text x=`"$x`" y=`"400`">$( $line[1] )</text>"
            }

            [double]$y = 375.00 - ([System.Double]::Parse($line[3]) - 80.00) * 370.00 / 20.00
            $LineCoveragePoints = "$LineCoveragePoints$x,$y "
            if (-not ($line[1].Contains("-")))
            {
                $LineCoverageCircles = "$LineCoverageCircles<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>"
            }

            $x = $x + $xSpacing
        }
    }

    # Generate branch coverage
    $BranchCoveragePoints = ""
    $BranchCoverageCircles = ""
    $x = 90
    foreach ($line in $html.table.tbody.tr)
    {
        $line = $line.td
        if ($line[2] -eq "BRANCH")
        {
            [double]$y = 375.00 - ([System.Double]::Parse($line[3]) - 80.00) * 370.00 / 20.00
            $BranchCoveragePoints = "$BranchCoveragePoints$x,$y "
            if (-not ($line[1].Contains("-")))
            {
                $BranchCoverageCircles = "$BranchCoverageCircles<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>"
            }

            $x = $x + $xSpacing
        }
    }

    # Generate method coverage
    $MethodCoveragePoints = ""
    $MethodCoverageCircles = ""
    $x = 90
    foreach ($line in $html.table.tbody.tr)
    {
        $line = $line.td
        if ($line[2] -eq "METHOD")
        {
            [double]$y = 375.00 - ([System.Double]::Parse($line[3]) - 80.00) * 370.00 / 20.00
            $MethodCoveragePoints = "$MethodCoveragePoints$x,$y "
            if (-not ($line[1].Contains("-")))
            {
                $MethodCoverageCircles = "$MethodCoverageCircles<circle cx=`"$x`" cy=`"$y`" r=`"4`"/>"
            }

            $x = $x + $xSpacing
        }
    }

    $SvgFilename = "./$MappaTestsAndCoveragePath/history.svg"
    "<svg version=`"1.2`" xmlns=`"http://www.w3.org/2000/svg`" xmlns:xlink=`"http://www.w3.org/1999/xlink`" style=`"height: 500px; width: 800px;font-family:'Open Sans', sans-serif;background:white`" role=`"img`">" | Out-File $SvgFilename
    "    <title id=`"title`">Mappa Code Coverage</title>" | Out-File -Append $SvgFilename
    "    <g style=`"stroke:#CCCCCC;stroke-width: 2;`"><line x1=`"90`" x2=`"90`" y1=`"5`" y2=`"375`"></line></g>" | Out-File -Append $SvgFilename
    "    <g style=`"stroke:#CCCCCC;stroke-width: 2;`"><line x1=`"90`" x2=`"705`" y1=`"375`" y2=`"375`"></line></g>" | Out-File -Append $SvgFilename
    "    <g style=`"text-anchor: middle;font-size: 13px;`">" | Out-File -Append $SvgFilename
    "            $xLabels" | Out-File -Append $SvgFilename
    "            <text x=`"400`" y=`"440`" style=`"font-weight: bold;text-transform: uppercase;font-size: 12px;fill: black;`">Versions</text>" | Out-File -Append $SvgFilename
    "    </g>" | Out-File -Append $SvgFilename
    "    <g style=`"text-anchor: end;font-size: 13px;`">" | Out-File -Append $SvgFilename
    "        <text x=`"80`" y=`"15`">100</text><text x=`"80`" y=`"107.5`">95</text><text x=`"80`" y=`"200`">90</text><text x=`"80`" y=`"292.5`">85</text><text x=`"80`" y=`"375`">80</text>" | Out-File -Append $SvgFilename
    "        <text x=`"50`" y=`"200`" style=`"font-weight: bold;text-transform: uppercase;font-size: 12px;fill: black;`">%</text>" | Out-File -Append $SvgFilename
    "    </g>"  | Out-File -Append $SvgFilename
    "    <g style=`"font-size: 12px;`" aria-label=`"Legend`">" | Out-File -Append $SvgFilename
    "        <line x1=`"520`" y1=`"310`" x2=`"550`" y2=`"310`" style=`"stroke:red;stroke-width:2`" /><circle cx=`"535`" cy=`"310`" r=`"4`" style=`"fill:red;stroke:red`" /><text x=`"560`" y=`"314`" style=`"fill:black`">Line coverage</text>" | Out-File -Append $SvgFilename
    "        <line x1=`"520`" y1=`"330`" x2=`"550`" y2=`"330`" style=`"stroke:blue;stroke-width:2`" /><circle cx=`"535`" cy=`"330`" r=`"4`" style=`"fill:blue;stroke:blue`" /><text x=`"560`" y=`"334`" style=`"fill:black`">Branch coverage</text>" | Out-File -Append $SvgFilename
    "        <line x1=`"520`" y1=`"350`" x2=`"550`" y2=`"350`" style=`"stroke:green;stroke-width:2`" /><circle cx=`"535`" cy=`"350`" r=`"4`" style=`"fill:green;stroke:green`" /><text x=`"560`" y=`"354`" style=`"fill:black`">Method coverage</text>" | Out-File -Append $SvgFilename
    "    </g>" | Out-File -Append $SvgFilename
    "    <g style=`"stroke:red; fill:red`" data-setname=`"Line coverage`">" | Out-File -Append $SvgFilename
    "        <polyline points=`"$LineCoveragePoints`" style=`"fill:none`" />" | Out-File -Append $SvgFilename
    "        $LineCoverageCircles" | Out-File -Append $SvgFilename
    "    </g>"  | Out-File -Append $SvgFilename
    "    <g style=`"stroke:blue; fill:blue`" data-setname=`"Branch coverage`">" | Out-File -Append $SvgFilename
    "        <polyline points=`"$BranchCoveragePoints`" style=`"fill:none`" />" | Out-File -Append $SvgFilename
    "        $BranchCoverageCircles" | Out-File -Append $SvgFilename
    "    </g>"  | Out-File -Append $SvgFilename
    "    <g style=`"stroke:green; fill:green`" data-setname=`"Method coverage`">" | Out-File -Append $SvgFilename
    "        <polyline points=`"$MethodCoveragePoints`" style=`"fill:none`" />" | Out-File -Append $SvgFilename
    "        $MethodCoverageCircles" | Out-File -Append $SvgFilename
    "    </g>"  | Out-File -Append $SvgFilename
    "</svg>" | Out-File -Append $SvgFilename
}

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

Out-Svg
Get-Content -Raw "./$MappaTestsAndCoveragePath/history.svg"

if (-not $DryRun)
{
    gh gist edit "7f4a85bc809328b4821b03125f9190cb" -f "MAPPA-CODE-COVERAGE-HISTORY.svg" "./$MappaTestsAndCoveragePath/history.svg"
}