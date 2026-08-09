// <copyright file="ReferenceHandlingMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the reference-handling sample mappers.
/// </summary>
public sealed class ReferenceHandlingMapperUnitTests
{
    private readonly ReferenceReusingCycleMapper cycleMapper = new();
    private readonly MaxRuntimeDepthMapper depthMapper = new();

    /// <summary>
    /// Closed A↔B cycle reuses the already-mapped person when returning through the address.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapPersonReusesMappedReferencesOnClosedCycle()
    {
        // Arrange
        var person = new ReferenceHandlingPersonSource { Id = 1 };
        var address = new ReferenceHandlingAddressSource { Id = 2, Owner = person };
        person.Address = address;
        var context = new MappaContext();

        // Act
        var result = this.cycleMapper.MapPerson(person, context);

        // Assert
        result.Id.Should().Be(1);
        result.Address.Should().NotBeNull();
        result.Address.Id.Should().Be(2);
        result.Address.Owner.Should().BeSameAs(result);
    }

    /// <summary>
    /// A null cycle edge terminates recurrence without needing reuse.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapPersonSucceedsWhenCycleEdgeIsNull()
    {
        // Arrange
        var person = new ReferenceHandlingPersonSource
        {
            Id = 1,
            Address = new ReferenceHandlingAddressSource { Id = 2, Owner = null },
        };
        var context = new MappaContext();

        // Act
        var result = this.cycleMapper.MapPerson(person, context);

        // Assert
        result.Id.Should().Be(1);
        result.Address.Should().NotBeNull();
        result.Address.Id.Should().Be(2);
        result.Address.Owner.Should().BeNull();
    }

    /// <summary>
    /// Mapping within <c>MaxRuntimeDepth</c> succeeds.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapWithinMaxRuntimeDepthSucceeds()
    {
        // Arrange
        var source = new ReferenceHandlingLevel0Source
        {
            Child = new ReferenceHandlingLevel1Source
            {
                Child = new ReferenceHandlingLevel2Source { Value = 42 },
            },
        };
        var context = new MappaContext();

        // Act
        var result = this.depthMapper.Map(source, context);

        // Assert
        result.Child.Child.Value.Should().Be(42);
    }

    /// <summary>
    /// Exceeding <c>MaxRuntimeDepth</c> throws <see cref="MappaException"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapExceedingMaxRuntimeDepthThrowsMappaException()
    {
        // Arrange
        var source = new ReferenceHandlingLevel0Source
        {
            Child = new ReferenceHandlingLevel1Source
            {
                Child = new ReferenceHandlingLevel2Source { Value = 7 },
            },
        };
        var overflowMapper = new MaxRuntimeDepthOverflowMapper();

        // Act
        var act = () => overflowMapper.Map(source, new MappaContext());

        // Assert
        act.Should().Throw<MappaException>()
            .Which.Message.Should().Contain("1");
    }
}