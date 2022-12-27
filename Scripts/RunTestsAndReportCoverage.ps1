$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"

if (Test-Path $MappaTestsAndCoveragePath)
{
    Remove-Item -Recurse -Force $MappaTestsAndCoveragePath > $null
}

New-Item -ItemType Directory -Name $MappaTestsAndCoveragePath > $null
dotnet test -c Release --collect:"XPlat Code Coverage" --logger "html" --logger "xunit" --results-directory "$MappaTestsAndCoveragePath"
if(-not $?)
{
    Exit 1
}

dotnet reportgenerator -reports:"$MappaTestsAndCoveragePath/**/*.xml" -targetdir:"$MappaTestsAndCoveragePath" -title:"Mappa" -reporttypes:"Html;MarkdownSummary" -filefilters:"-*.g.cs" -assemblyfilters:"-Mappa.Samples"
if(-not $?)
{
    Exit 1
}
