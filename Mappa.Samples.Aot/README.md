# Mappa.Samples.Aot
This project generates an executable that verifies all sample mappers compile and run under [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot).

It mirrors the mappers defined in [Mappa.Samples](../Mappa.Samples) and exercises them at publish time. [`IQueryableProjectionMapper`](../Mappa.Samples/IQueryableProjectionMapper.cs) is excluded because queryable projection uses expression trees annotated with `[RequiresDynamicCode]` and is not compatible with Native AOT.

## How to compile AOT
Run the following command:

```powershell
dotnet publish -c Release --self-contained ./Mappa.Samples.Aot/
```
