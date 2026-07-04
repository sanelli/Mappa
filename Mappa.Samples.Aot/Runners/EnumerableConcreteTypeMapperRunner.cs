// <copyright file="EnumerableConcreteTypeMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for the enumerable concrete type sample mappers.
/// </summary>
internal static class EnumerableConcreteTypeMapperRunner
{
    /// <summary>
    /// Runs all enumerable concrete type sample mappers.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        var input = AotSampleData.CountingValuesOneThreeEnumerable;

        report.BeginMapper(nameof(EnumerableConcreteTypeListMapper));
        var listMapper = new EnumerableConcreteTypeListMapper();
        var listResult = listMapper.Map(input);
        VerifyRuntimeType(listResult, typeof(List<int>));
        report.RecordInvocation(
            nameof(EnumerableConcreteTypeListMapper.Map),
            "IEnumerable<CountingValues>",
            "IEnumerable<int>",
            input,
            $"runtimeType={listResult.GetType().FullName};values=[{string.Join(", ", listResult)}]");

        report.BeginMapper(nameof(EnumerableConcreteTypeArrayMapper));
        var arrayMapper = new EnumerableConcreteTypeArrayMapper();
        var arrayResult = arrayMapper.Map(input);
        VerifyRuntimeType(arrayResult, typeof(int[]));
        report.RecordInvocation(
            nameof(EnumerableConcreteTypeArrayMapper.Map),
            "IEnumerable<CountingValues>",
            "IEnumerable<int>",
            input,
            $"runtimeType={arrayResult.GetType().FullName};values=[{string.Join(", ", arrayResult)}]");

        report.BeginMapper(nameof(EnumerableConcreteTypeExplicitListMapper));
        var explicitListMapper = new EnumerableConcreteTypeExplicitListMapper();
        var explicitListResult = explicitListMapper.Map(input);
        VerifyRuntimeType(explicitListResult, typeof(List<int>));
        report.RecordInvocation(
            nameof(EnumerableConcreteTypeExplicitListMapper.Map),
            "IEnumerable<CountingValues>",
            "List<int>",
            input,
            $"runtimeType={explicitListResult.GetType().FullName};values=[{string.Join(", ", explicitListResult)}]");

        report.BeginMapper(nameof(EnumerableConcreteTypeArrayInterfaceMapper));
        var arrayInterfaceMapper = new EnumerableConcreteTypeArrayInterfaceMapper();
        var arrayInterfaceResult = arrayInterfaceMapper.Map(input);
        VerifyRuntimeType(arrayInterfaceResult, typeof(int[]));
        report.RecordInvocation(
            nameof(EnumerableConcreteTypeArrayInterfaceMapper.Map),
            "IEnumerable<CountingValues>",
            "ICollection<int>",
            input,
            $"runtimeType={arrayInterfaceResult.GetType().FullName};values=[{string.Join(", ", arrayInterfaceResult)}]");
    }

    private static void VerifyRuntimeType(object result, Type expectedType)
    {
        if (result.GetType() != expectedType)
        {
            throw new InvalidOperationException(
                $"Expected runtime type {expectedType.FullName} but got {result.GetType().FullName}.");
        }
    }
}