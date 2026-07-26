// <copyright file="IQueryableProjectionMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="IQueryableProjectionMapper"/>.
/// </summary>
internal static class IQueryableProjectionMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="IQueryableProjectionMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "Sample smoke-tests ProjectToDto against an in-memory IQueryable; production ORM providers supply queryables without AsQueryable.")]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050",
        Justification = "Sample smoke-tests ProjectToDto against an in-memory IQueryable; production ORM providers supply queryables without AsQueryable.")]
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(IQueryableProjectionMapper));
        var orders = new List<ProjectionOrder>
        {
            new() { Id = 42, Name = "Gamma", CustomerName = "Carol" },
        };

        report.RecordInvocation(
            nameof(IQueryableProjectionMapper.ProjectToDto),
            "IQueryable<ProjectionOrder>",
            "IQueryable<ProjectionOrderDto>",
            orders,
            orders.AsQueryable().ProjectToDto().ToList());
    }
}