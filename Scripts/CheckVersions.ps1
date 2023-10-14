# Check that the versions of the packages used by Benchmark
# and samples matches the one epoxsed by Mappa and Mappa.Generator

[xml]$MappaCsproj = Get-Content ./Mappa.targets
$ExpectedMappaVersion = "$($MappaCsproj.Project.PropertyGroup.Version)".Trim()

# [xml]$MappaGeneratorCsproj = Get-Content ./Mappa.targets
# $ExpectedMappaGeneratorVersion = "$($MappaGeneratorCsproj.Project.PropertyGroup.Version)".Trim()

[xml]$BenchmatkCsproj = Get-Content ./Mappa.Benchmark/Mappa.Benchmark.csproj
$MappaVersionInBenchmark = "$($($BenchmatkCsproj.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Mappa" }).Version)".Trim()
$MappaGeneratorVersionInBenchmark = "$($($BenchmatkCsproj.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Mappa.Generator" }).Version)".Trim()

[xml]$SamplesCsproj = Get-Content ./Mappa.Samples/Mappa.Samples.csproj
$MappaVersionInSample = "$($($SamplesCsproj.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Mappa" }).Version)".Trim()
$MappaGeneratorVersionInSample = "$($($SamplesCsproj.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Mappa.Generator" }).Version)".Trim()

Write-Host "- Expected: $ExpectedMappaVersion"

$Success = $true

# Is Mappa version correct in Mappa.Benchmarks
Write-Host "- Actual"
Write-Host "  - Mappa.Benchmarks:"
Write-Host "    - Mappa: " -NoNewline
if($ExpectedMappaVersion -ne $MappaVersionInBenchmark)
{
    Write-Host "[KO] (actual: $MappaVersionInBenchmark)" -ForegroundColor Red
    $Success = $false
}
else
{
    Write-Host "[OK]" -ForegroundColor Green
}

# Is Mappa.Generator version correct in Mappa.Benchmarks
Write-Host "    - Mappa.Generator: " -NoNewline
if($ExpectedMappaVersion -ne $MappaGeneratorVersionInBenchmark)
{
    Write-Host "[KO] (actual: $MappaGeneratorVersionInBenchmark)" -ForegroundColor Red
    $Success = $false
}
else
{
    Write-Host "[OK]" -ForegroundColor Green
}

# Is Mappa version correct in Mappa.Samples
Write-Host "  - Mappa.Samples:"
Write-Host "    - Mappa: " -NoNewline
if($ExpectedMappaVersion -ne $MappaVersionInSample)
{
    Write-Host "[KO] (actual: $MappaVersionInSample)" -ForegroundColor Red
    $Success = $false
}
else
{
    Write-Host "[OK]" -ForegroundColor Green
}

# Is Mappa.Generator version correct in Mappa.Benchmarks
Write-Host "    - Mappa.Generator: " -NoNewline
if($ExpectedMappaVersion -ne $MappaGeneratorVersionInSample)
{
    Write-Host "[KO] (actual: $MappaGeneratorVersionInSample)" -ForegroundColor Red
    $Success = $false
}
else
{
    Write-Host "[OK]" -ForegroundColor Green
}

# All done, return the correct error code
if($Success)
{
    Write-Host "Versions match!" -ForegroundColor Green
    exit 0
}
else
{
    Write-Host "Versions do not match!" -ForegroundColor Red
    exit 1
}