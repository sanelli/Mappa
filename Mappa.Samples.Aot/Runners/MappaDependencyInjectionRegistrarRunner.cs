// <copyright file="MappaDependencyInjectionRegistrarRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Dependency.Bson;
using Mappa.Dependency.Protobuf;
using Mappa.Samples;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for dependency-injection sample registrars.
/// </summary>
internal static class MappaDependencyInjectionRegistrarRunner
{
    /// <summary>
    /// Runs dependency injection registration for both sample registrars.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        RunInjectFromAssembliesRegistrar(report);
        RunSameAssemblyRegistrar(report);
    }

    private static void RunInjectFromAssembliesRegistrar(AotReport report)
    {
        report.BeginMapper(nameof(MappaDependencyInjectionRegistrar));

        var services = new ServiceCollection();
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<MappaDependencyInjectionMapper>();
        var bsonMapper = provider.GetRequiredService<MappaBsonMapper>();
        var protobufMapper = provider.GetRequiredService<MappaProtobufMapper>();

        report.RecordInvocation(
            nameof(MappaDependencyInjectionMapper.Map),
            "int",
            "string",
            42,
            mapper.Map(42));
        report.RecordInvocation(
            nameof(MappaBsonMapper.MapToString),
            "ObjectId",
            "string",
            ObjectId.Empty,
            bsonMapper.MapToString(ObjectId.Empty));
        report.RecordInvocation(
            nameof(MappaProtobufMapper),
            "resolved",
            "MappaProtobufMapper",
            "InjectFromAssemblies",
            protobufMapper.GetType().Name);
        report.RecordInvocation(
            "IgnoreType",
            "IdentityStrategyMapper",
            "null",
            "excluded",
            provider.GetService<IdentityStrategyMapper>() is null);
        report.RecordInvocation(
            "IgnoreType",
            "GuidStrategyMapper",
            "null",
            "excluded",
            provider.GetService<GuidStrategyMapper>() is null);
    }

    private static void RunSameAssemblyRegistrar(AotReport report)
    {
        report.BeginMapper(nameof(MappaDependencyInjectionSameAssemblyRegistrar));

        var services = new ServiceCollection();
        services.RegisterMappaSamplesSameAssembly();
        using var provider = services.BuildServiceProvider();
        var guidMapper = provider.GetRequiredService<GuidStrategyMapper>();

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapFromGuidToArray),
            "Guid",
            "byte[]",
            Guid.Empty,
            guidMapper.MapFromGuidToArray(Guid.Empty));
        report.RecordInvocation(
            "IgnoreType",
            "MappaDependencyInjectionMapper",
            "null",
            "excluded",
            provider.GetService<MappaDependencyInjectionMapper>() is null);
        report.RecordInvocation(
            "IgnoreType",
            "IdentityStrategyMapper",
            "null",
            "excluded",
            provider.GetService<IdentityStrategyMapper>() is null);
        report.RecordInvocation(
            "NoInjectFromAssemblies",
            "MappaBsonMapper",
            "null",
            "not-discovered",
            provider.GetService<MappaBsonMapper>() is null);
        report.RecordInvocation(
            "NoInjectFromAssemblies",
            "MappaProtobufMapper",
            "null",
            "not-discovered",
            provider.GetService<MappaProtobufMapper>() is null);
    }
}