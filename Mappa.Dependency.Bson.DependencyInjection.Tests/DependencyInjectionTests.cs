// <copyright file="DependencyInjectionTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

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
    /// Tests that it is possible to inject the Bson mapper as a singleton.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanInjectBsonMapperAsSingleton()
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
    /// Tests that it is possible to inject the Bson mapper interface as a singleton.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanInjectBsonMapperInterfaceAsSingleton()
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

    /// <summary>
    /// Tests that the concrete and interface registrations share the same singleton instance.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConcreteAndInterfaceResolveToSameSingletonInstance()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterMappaBson();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var concrete = serviceProvider.GetRequiredService<MappaBsonMapper>();
        var asInterface = serviceProvider.GetRequiredService<IMappaBsonMapper>();

        // Assert
        asInterface.Should().BeSameAs(concrete);
    }
}