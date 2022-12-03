$MappaTestsAndCoveragePath = ".mappa-tests-and-coverage"

if (Test-Path $MappaTestsAndCoveragePath)
{
    Remove-Item -Recurse -Force $MappaTestsAndCoveragePath
}

New-Item -ItemType Directory -Name $MappaTestsAndCoveragePath
dotnet test -c Release Mappa.Tests --collect:"XPlat Code Coverage" --logger "html" --logger "xunit" --results-directory "$MappaTestsAndCoveragePath"
dotnet reportgenerator -reports:"$MappaTestsAndCoveragePath/**/*.xml" -targetdir:"$MappaTestsAndCoveragePath" -title:"Mappa" -reporttypes:"Html;MarkdownSummary"