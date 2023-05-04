if (Test-Path "BenchmarkDotNet.Artifacts")
{
    Remove-Item -Recurse -Force "BenchmarkDotNet.Artifacts" > $null
}

dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Html" "GitHub" "Csv" --filter "*SpotifyBenchmark*"
dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Html" "GitHub" "Csv" --filter "*EnumToIntBenchmark*"
dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Html" "GitHub" "Csv" --filter "*EnumToStringBenchmark*"
dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Html" "GitHub" "Csv" --filter "*IntToEnumBenchmark*"
dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Html" "GitHub" "Csv" --filter "*StringToEnumBenchmark*"