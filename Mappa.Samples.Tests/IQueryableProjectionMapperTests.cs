// <copyright file="IQueryableProjectionMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="IQueryableProjectionMapper"/>.
/// </summary>
public sealed class IQueryableProjectionMapperTests
{
    /// <summary>
    /// Test projecting an in-memory <see cref="IQueryable{T}"/> of orders.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanProjectOrdersUsingAsQueryable()
    {
        // Arrange
        var orders = new List<ProjectionOrder>
        {
            new() { Id = 1, Name = "Alpha", CustomerName = "Alice" },
            new() { Id = 2, Name = "Beta", CustomerName = "Bob" },
        };

        // Act
        var actual = orders.AsQueryable().ProjectToDto().ToList();

        // Assert
        actual.Should().BeEquivalentTo(
        [
            new ProjectionOrderDto { Id = 1, Title = "Alpha", CustomerName = "Alice" },
            new ProjectionOrderDto { Id = 2, Title = "Beta", CustomerName = "Bob" },
        ]);
    }
}