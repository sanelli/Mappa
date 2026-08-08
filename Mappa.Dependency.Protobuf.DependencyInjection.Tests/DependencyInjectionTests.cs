// <copyright file="DependencyInjectionTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit.OpenCategories.V3;

namespace Mappa.Dependency.Protobuf.DependencyInjection.Tests;

/// <summary>
/// Tests for the protobuf dependency injection.
/// </summary>
public sealed class DependencyInjectionTests
{
    /// <summary>
    /// Tests that it is possible to inject the protobuf mapper as a singleton.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanInjectProtobufMapperAsSingleton()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterMappaProtobuf();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var service = serviceProvider.GetService<MappaProtobufMapper>();

        // Assert
        service.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that it is possible to inject the protobuf mapper interface as a singleton.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanInjectProtobufMapperInterfaceAsSingleton()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterMappaProtobuf();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var service = serviceProvider.GetService<IMappaProtobufMapper>();

        // Assert
        service.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the concrete and interface registrations share the same singleton instance.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConcreteAndInterfaceResolveToSameSingletonInstance()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterMappaProtobuf();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var concrete = serviceProvider.GetRequiredService<MappaProtobufMapper>();
        var asInterface = serviceProvider.GetRequiredService<IMappaProtobufMapper>();

        // Assert
        asInterface.Should().BeSameAs(concrete);
    }
}