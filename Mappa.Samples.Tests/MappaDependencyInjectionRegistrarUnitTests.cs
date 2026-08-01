// <copyright file="MappaDependencyInjectionRegistrarUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MappaDependencyInjectionRegistrar"/>.
/// </summary>
public sealed class MappaDependencyInjectionRegistrarUnitTests
{
    /// <summary>
    /// <c>RegisterMappaSamples</c> registers <see cref="MappaDependencyInjectionMapper"/>
    /// so it can be resolved from the service provider.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesRegistersDependencyInjectionMapper()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<MappaDependencyInjectionMapper>();

        // Assert
        mapper.Map(42).Should().Be("42");
    }

    /// <summary>
    /// <c>RegisterMappaSamples</c> also registers other same-assembly <c>[Mappa]</c> mappers.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesRegistersOtherSampleMappers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IdentityStrategyMapper>();

        // Assert
        mapper.MapStringToString("hello").Should().Be("hello");
    }
}