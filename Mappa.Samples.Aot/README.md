# Mappa.Samples.Aot
This project generates an executable that verifies all sample mappers compile and run under [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot).

It mirrors the mappers defined in [Mappa.Samples](../Mappa.Samples) and exercises them at publish time.

## How to compile AOT
Run the following command:

```powershell
dotnet publish -c Release --self-contained ./Mappa.Samples.Aot/
```
