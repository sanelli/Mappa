// <copyright file="MappaDependencyInjectionRegistrarUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Dependency.Bson;
using Mappa.Dependency.Protobuf;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

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
    /// <c>RegisterMappaSamples</c> registers <see cref="MappaBsonMapper"/> via <c>InjectFromAssemblies</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesRegistersBsonMapperFromInjectFromAssemblies()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<MappaBsonMapper>();

        // Assert
        mapper.MapToString(ObjectId.Empty).Should().Be(ObjectId.Empty.ToString());
    }

    /// <summary>
    /// <c>RegisterMappaSamples</c> registers <see cref="MappaProtobufMapper"/> via <c>InjectFromAssemblies</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesRegistersProtobufMapperFromInjectFromAssemblies()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamples();
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<MappaProtobufMapper>();

        // Assert
        mapper.Should().NotBeNull();
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
    /// <c>RegisterMappaSamplesSameAssembly</c> does not discover mappers from referenced assemblies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void RegisterMappaSamplesSameAssemblyDoesNotRegisterExternalMappers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.RegisterMappaSamplesSameAssembly();
        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<MappaBsonMapper>().Should().BeNull();
        provider.GetService<MappaProtobufMapper>().Should().BeNull();
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