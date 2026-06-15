// <copyright file="DependencyInjectionTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Dependency.Bson.DependencyInjection.Tests;

/// <summary>
/// Tests for the Bson dependency injection.
/// </summary>
public sealed class DependencyInjectionTests
{
    /// <summary>
    /// That that is possible inject the Bson mappers as singletons.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanInjectProtobufMapperAsSingleton()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterMappaBson();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var service = serviceProvider.GetService<MappaBsonMapper>();

        // Assert
        service.Should().NotBeNull();
    }

    /// <summary>
    /// That that is possible inject the Bson mappers interface as singletons.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanInjectProtobufMapperInterfaceAsSingleton()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterMappaBson();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var service = serviceProvider.GetService<IMappaBsonMapper>();

        // Assert
        service.Should().NotBeNull();
    }
}