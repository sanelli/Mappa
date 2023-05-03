if (Test-Path "BenchmarkDotNet.Artifacts")
{
    Remove-Item -Recurse -Force "BenchmarkDotNet.Artifacts" > $null
}

dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -e "Html" "GitHub" "Csv" --filter "*SpotifyBenchmark*"

#dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -f "*MapIntToIntBenchmark*" -e "Html" "GitHub" "Csv"
#dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -f "*MapIntToObjectBenchmark*" -e "Html" "GitHub" "Csv"
#dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -f "*MapStringToObjectBenchmark*" -e "Html" "GitHub" "Csv"
#dotnet run -c Release --project ./Mappa.Benchmark/ -- -j Short -f "*MapStringToStringBenchmark*" -e "Html" "GitHub" "Csv"