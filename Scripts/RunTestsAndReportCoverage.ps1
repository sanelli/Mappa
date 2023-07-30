$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"

if (Test-Path $MappaTestsAndCoveragePath)
{
    Remove-Item -Recurse -Force $MappaTestsAndCoveragePath > $null
}

New-Item -ItemType Directory -Name $MappaTestsAndCoveragePath > $null
dotnet test -c Release --collect:"XPlat Code Coverage" --logger "html" --logger "xunit;LogFileName=mappa.{assembly}.test-results.xml" --results-directory "$MappaTestsAndCoveragePath"
if(-not $?)
{
    Write-Host "Test failed" -ForegroundColor Red
    Exit 1
}

dotnet tool restore
dotnet reportgenerator -reports:"$MappaTestsAndCoveragePath/**/*.xml" -targetdir:"$MappaTestsAndCoveragePath" -title:"Mappa" -reporttypes:"Html;MarkdownSummary;XmlSummary" -filefilters:"-*.g.cs" -assemblyfilters:"-Mappa.Samples"
if(-not $?)
{
    Write-Host "Report failed" -ForegroundColor Red
    Exit 1
}

[xml]$Report = Get-Content -Raw "./$MappaTestsAndCoveragePath/Summary.xml"
[double]$LineCoverage = [double]::Parse($Report.CoverageReport.Summary.LineCoverage)
[double]$Branchcoverage = [double]::Parse($Report.CoverageReport.Summary.Branchcoverage)
[double]$Methodcoverage = [double]::Parse($Report.CoverageReport.Summary.Methodcoverage)

if(($LineCoverage -lt 90.0) -or ($Branchcoverage -lt 80.0) -or ($Methodcoverage -lt 80.0))
{
    Write-Host "Poor coverage:" -ForegroundColor Red
    Write-Host " - Line Coverage: $LineCoverage" -ForegroundColor Red
    Write-Host " - Branch Coverage: $Branchcoverage" -ForegroundColor Red
    Write-Host " - Method Coverage: $Methodcoverage" -ForegroundColor Red
    Exit 1
}

Write-Host "Coverage:" -ForegroundColor Green
Write-Host " - Line Coverage: $LineCoverage %" -ForegroundColor Green
Write-Host " - Branch Coverage: $Branchcoverage %" -ForegroundColor Green
Write-Host " - Method Coverage: $Methodcoverage %" -ForegroundColor Green