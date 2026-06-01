// <copyright file="IdentityStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="IdentityStrategyMapper"/>.
/// </summary>
internal static class IdentityStrategyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="IdentityStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(IdentityStrategyMapper));
        var mapper = new IdentityStrategyMapper();
        const int intInput = 17;
        const string stringInput = "Test string";

        report.RecordInvocation(
            nameof(IdentityStrategyMapper.MapStringToString),
            "string?",
            "string?",
            stringInput,
            mapper.MapStringToString(stringInput));

        report.RecordInvocation(
            nameof(IdentityStrategyMapper.MapIntToIntWhenNullableIsDisabled),
            "int",
            "int",
            intInput,
            mapper.MapIntToIntWhenNullableIsDisabled(intInput));

        report.RecordInvocation(
            nameof(IdentityStrategyMapper.MapIntToNullableIntWhenNullableIsDisabled),
            "int",
            "int?",
            intInput,
            mapper.MapIntToNullableIntWhenNullableIsDisabled(intInput));

        report.RecordInvocation(
            nameof(IdentityStrategyMapper.MapIntToObjectWhenNullableIsDisabled),
            "int",
            "object",
            intInput,
            mapper.MapIntToObjectWhenNullableIsDisabled(intInput));

        report.RecordInvocation(
            nameof(IdentityStrategyMapper.MapIntToNullableObjectWhenNullableIsEnabled),
            "int",
            "object?",
            intInput,
            mapper.MapIntToNullableObjectWhenNullableIsEnabled(intInput));

        report.RecordInvocation(
            nameof(IdentityStrategyMapper.MapStringToObjectWhenNullableIsDisabled),
            "string",
            "object",
            stringInput,
            mapper.MapStringToObjectWhenNullableIsDisabled(stringInput));
    }
}