// <copyright file="MappaDependencyInjectionRegistrarUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for dependency-injection sample registrars.
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
    /// <c>RegisterMappaSamples</c> registers well-known settings sample mappers.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesRegistersWellKnownTypesMappers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetRequiredService<ProtobufWellKnownTypesMapper>().Should().NotBeNull();
        provider.GetRequiredService<BsonWellKnownTypesMapper>().Should().NotBeNull();
    }

    /// <summary>
    /// <c>RegisterMappaSamples</c> does not register types listed in <c>IgnoreType</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesDoesNotRegisterIgnoredTypes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<IdentityStrategyMapper>().Should().BeNull();
        provider.GetService<GuidStrategyMapper>().Should().BeNull();
    }

    /// <summary>
    /// <c>RegisterMappaSamplesSameAssembly</c> registers same-assembly mappers that are not ignored.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesSameAssemblyRegistersNonIgnoredMapper()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamplesSameAssembly();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<GuidStrategyMapper>();

        // Assert
        mapper.MapFromGuidToArray(Guid.Empty).Should().Equal(Guid.Empty.ToByteArray());
    }

    /// <summary>
    /// <c>RegisterMappaSamplesSameAssembly</c> does not register types listed in <c>IgnoreType</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesSameAssemblyDoesNotRegisterIgnoredTypes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamplesSameAssembly();
        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<MappaDependencyInjectionMapper>().Should().BeNull();
        provider.GetService<IdentityStrategyMapper>().Should().BeNull();
    }
}