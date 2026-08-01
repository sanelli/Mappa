// <copyright file="MappaDependencyInjectionRegistrarRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

using Microsoft.Extensions.DependencyInjection;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="MappaDependencyInjectionRegistrar"/>.
/// </summary>
internal static class MappaDependencyInjectionRegistrarRunner
{
    /// <summary>
    /// Runs dependency injection registration and resolves <see cref="MappaDependencyInjectionMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(MappaDependencyInjectionRegistrar));

        var services = new ServiceCollection();
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<MappaDependencyInjectionMapper>();

        report.RecordInvocation(
            nameof(MappaDependencyInjectionMapper.Map),
            "int",
            "string",
            42,
            mapper.Map(42));
    }
}