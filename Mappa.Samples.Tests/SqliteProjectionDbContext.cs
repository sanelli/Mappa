// <copyright file="SqliteProjectionDbContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace Mappa.Samples.Tests;

/// <summary>
/// EF Core Sqlite context for the queryable projection sample.
/// </summary>
/// <param name="options">The context options.</param>
internal sealed class SqliteProjectionDbContext(DbContextOptions<SqliteProjectionDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Gets the orders set.
    /// </summary>
    public DbSet<SqliteProjectionOrder> Orders => this.Set<SqliteProjectionOrder>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<SqliteProjectionOrder>(entity =>
        {
            entity.HasKey(order => order.Id);
            entity.HasMany(order => order.Children)
                .WithOne()
                .HasForeignKey(child => child.OrderId);
        });

        modelBuilder.Entity<SqliteProjectionChild>(entity =>
        {
            entity.HasKey(child => child.Id);
        });
    }
}