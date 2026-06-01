// <copyright file="NullableToNullableMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="NullableToNullableMapper"/>.
/// </summary>
internal static class NullableToNullableMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="NullableToNullableMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(NullableToNullableMapper));
        var mapper = new NullableToNullableMapper();
        CountingValues? nullableInput = CountingValues.Two;
        var nonNullableInput = CountingValues.Two;

        report.RecordInvocation(
            nameof(NullableToNullableMapper.Map),
            "CountingValues?",
            "int?",
            nullableInput,
            mapper.Map(nullableInput));

        report.RecordInvocation(
            nameof(NullableToNullableMapper.Map),
            nameof(CountingValues),
            "int?",
            nonNullableInput,
            mapper.Map(nonNullableInput));

        report.RecordInvocation(
            nameof(NullableToNullableMapper.MapToNonNullable),
            "CountingValues?",
            "int",
            nullableInput,
            mapper.MapToNonNullable(nullableInput));
    }
}