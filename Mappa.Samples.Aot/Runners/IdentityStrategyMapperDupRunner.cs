// <copyright file="IdentityStrategyMapperDupRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="IdentityStrategyMapperDup"/>.
/// </summary>
internal static class IdentityStrategyMapperDupRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="IdentityStrategyMapperDup"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(IdentityStrategyMapperDup));
        var mapper = new IdentityStrategyMapperDup();
        const int intInput = 17;
        const string stringInput = "Test string";

        report.RecordInvocation(
            nameof(IdentityStrategyMapperDup.MapIntToNullableIntWhenNullableIsEnabled),
            "int",
            "int?",
            intInput,
            mapper.MapIntToNullableIntWhenNullableIsEnabled(intInput));

        report.RecordInvocation(
            nameof(IdentityStrategyMapperDup.MapStringToObjectWhenNullableIsEnabled),
            "string",
            "object",
            stringInput,
            mapper.MapStringToObjectWhenNullableIsEnabled(stringInput));
    }
}