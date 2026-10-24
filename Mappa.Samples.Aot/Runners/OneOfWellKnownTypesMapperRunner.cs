// <copyright file="OneOfWellKnownTypesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

using OneOf;
using OneOf.Types;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="OneOfWellKnownTypesMapper"/>.
/// </summary>
internal static class OneOfWellKnownTypesMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="OneOfWellKnownTypesMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(OneOfWellKnownTypesMapper));
        var mapper = new OneOfWellKnownTypesMapper();
        var some = OneOf<string, None>.FromT0("aot");
        var none = OneOf<string, None>.FromT1(default(None));
        var stringOrInt = OneOf<string, int>.FromT0("union");
        var stringOrIntBase = new StringOrInt("base");

        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapOptionToString),
            "OneOf<string, None>",
            "string",
            some,
            mapper.MapOptionToString(some));
        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapOptionToNullableString),
            "OneOf<string, None>",
            "string?",
            none,
            mapper.MapOptionToNullableString(none));
        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapStringToOption),
            "string?",
            "OneOf<string, None>",
            "wrap",
            mapper.MapStringToOption("wrap"));
        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapStringOrIntToString),
            "OneOf<string, int>",
            "string",
            stringOrInt,
            mapper.MapStringOrIntToString(stringOrInt));
        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapStringToStringOrInt),
            "string",
            "OneOf<string, int>",
            "identity",
            mapper.MapStringToStringOrInt("identity"));
        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapStringOrIntBaseToString),
            "StringOrInt",
            "string",
            stringOrIntBase,
            mapper.MapStringOrIntBaseToString(stringOrIntBase));
        report.RecordInvocation(
            nameof(OneOfWellKnownTypesMapper.MapStringToStringOrIntBase),
            "string",
            "StringOrInt",
            "ctor",
            mapper.MapStringToStringOrIntBase("ctor"));
    }
}