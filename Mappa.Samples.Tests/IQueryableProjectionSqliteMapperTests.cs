// <copyright file="IQueryableProjectionSqliteMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Mappa.Samples.Tests;

/// <summary>
/// Runtime tests for <see cref="IQueryableProjectionSqliteMapper"/> against EF Core Sqlite.
/// </summary>
public sealed class IQueryableProjectionSqliteMapperTests
{
    /// <summary>
    /// Test that nested-collection projection translates and materializes against EF Core Sqlite.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectNestedCollectionsAgainstSqlite()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync(CancellationToken.None).ConfigureAwait(true);
        var options = new DbContextOptionsBuilder<SqliteProjectionDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new SqliteProjectionDbContext(options);
        try
        {
            await context.Database.EnsureCreatedAsync(CancellationToken.None).ConfigureAwait(true);

            context.Orders.Add(new SqliteProjectionOrder
            {
                Id = 1,
                Name = "Alpha",
                CustomerName = "Alice",
                Children =
                [
                    new SqliteProjectionChild { Id = 10, OrderId = 1, Label = "A1" },
                    new SqliteProjectionChild { Id = 11, OrderId = 1, Label = "A2" },
                ],
            });
            await context.SaveChangesAsync(CancellationToken.None).ConfigureAwait(true);

            // Act
            var query = context.Orders.AsNoTracking().ProjectToDto();
            var sql = query.ToQueryString();
            var actual = await query.ToListAsync(CancellationToken.None).ConfigureAwait(true);

            // Assert
            sql.Should().Contain("Name", because: "the Title mapping projects the Name column");
            sql.Should().Contain("Label", because: "nested children are projected in SQL");
            actual.Should().BeEquivalentTo(
            [
                new SqliteProjectionOrderDto
                {
                    Id = 1,
                    Title = "Alpha",
                    CustomerName = "Alice",
                    Children =
                    [
                        new SqliteProjectionChildDto { Id = 10, Label = "A1" },
                        new SqliteProjectionChildDto { Id = 11, Label = "A2" },
                    ],
                },
            ]);
        }
        finally
        {
            await context.DisposeAsync().ConfigureAwait(true);
            await connection.DisposeAsync().ConfigureAwait(true);
        }
    }
}