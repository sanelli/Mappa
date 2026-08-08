// <copyright file="BreakCompileTimeCyclesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="BreakCompileTimeCyclesMapper"/>.
/// </summary>
public sealed class BreakCompileTimeCyclesMapperUnitTests
{
    private readonly BreakCompileTimeCyclesMapper mapper = new();

    /// <summary>
    /// Closed A↔B cycle maps with a single root method and reuses the person reference.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapReusesMappedReferencesOnClosedCycle()
    {
        // Arrange
        var person = new ReferenceHandlingPersonSource { Id = 1 };
        var address = new ReferenceHandlingAddressSource { Id = 2, Owner = person };
        person.Address = address;
        var context = new MappaContext();

        // Act
        var result = this.mapper.Map(person, context);

        // Assert
        result.Id.Should().Be(1);
        result.Address.Should().NotBeNull();
        result.Address!.Id.Should().Be(2);
        result.Address.Owner.Should().BeSameAs(result);
    }

    /// <summary>
    /// A null cycle edge terminates without needing reference reuse beyond the first nest.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapSucceedsWhenCycleEdgeIsNull()
    {
        // Arrange
        var person = new ReferenceHandlingPersonSource
        {
            Id = 1,
            Address = new ReferenceHandlingAddressSource { Id = 2, Owner = null },
        };
        var context = new MappaContext();

        // Act
        var result = this.mapper.Map(person, context);

        // Assert
        result.Id.Should().Be(1);
        result.Address.Should().NotBeNull();
        result.Address!.Id.Should().Be(2);
        result.Address.Owner.Should().BeNull();
    }
}