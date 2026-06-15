// <copyright file="DependencyInjectionTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit.OpenCategories.V3;

namespace Mappa.Dependency.Protobuf.DependencyInjection.Tests;

/// <summary>
/// Tests for the protobuf dependency injection.
/// </summary>
public sealed class DependencyInjectionTests
{
    /// <summary>
    /// That that is possible inject the protobuf mappers as singletons.
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
    /// That that is possible inject the protobuf mappers interface as singletons.
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
}