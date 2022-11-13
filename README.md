# Mappa
Mapper using source generators

## Notes
In order to be able to compile you need to setup a `nuget.config` like the following:
```XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="NuGet.org" value="https://api.nuget.org/v3/index.json" />
        <add key="Local .packages" value="file://{PATH-TO-YOUR-REPO}/.packages" />
    </packageSources>
</configuration>
```

## References
- [Roslyn API FAQ](https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/APISamples/FAQ.cs)
- [Source generator Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)
