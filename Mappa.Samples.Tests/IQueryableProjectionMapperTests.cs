// <copyright file="IQueryableProjectionMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="IQueryableProjectionMapper"/>.
/// </summary>
public sealed class IQueryableProjectionMapperTests
{
    private static readonly Guid PublicIdAlpha = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid PublicIdBeta = Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <summary>
    /// Test projecting an in-memory <see cref="IQueryable{T}"/> of orders with nested children, Guid, and tuple maps.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanProjectOrdersUsingAsQueryable()
    {
        // Arrange
        var orders = new List<ProjectionOrder>
        {
            new()
            {
                Id = 1,
                Name = "Alpha",
                CustomerName = "Alice",
                PublicId = PublicIdAlpha.ToString("D"),
                Summary = (2, "alpha-summary"),
                Children =
                [
                    new ProjectionChild { Id = 10, Label = "A1" },
                    new ProjectionChild { Id = 11, Label = "A2" },
                ],
            },
            new()
            {
                Id = 2,
                Name = "Beta",
                CustomerName = "Bob",
                PublicId = PublicIdBeta.ToString("D"),
                Summary = (1, "beta-summary"),
                Children =
                [
                    new ProjectionChild { Id = 20, Label = "B1" },
                ],
            },
        };

        // Act
        var actual = orders.AsQueryable().ProjectToDto().ToList();

        // Assert
        actual.Should().BeEquivalentTo(
        [
            new ProjectionOrderDto
            {
                Id = 1,
                Title = "Alpha",
                CustomerName = "Alice",
                PublicId = PublicIdAlpha,
                Summary = ("2", "alpha-summary"),
                Children =
                [
                    new ProjectionChildDto { Id = 10, Label = "A1" },
                    new ProjectionChildDto { Id = 11, Label = "A2" },
                ],
            },
            new ProjectionOrderDto
            {
                Id = 2,
                Title = "Beta",
                CustomerName = "Bob",
                PublicId = PublicIdBeta,
                Summary = ("1", "beta-summary"),
                Children =
                [
                    new ProjectionChildDto { Id = 20, Label = "B1" },
                ],
            },
        ]);
    }
}