// <copyright file="TupleToTupleMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="TupleToTupleMapper"/>.
/// </summary>
internal static class TupleToTupleMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="TupleToTupleMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(TupleToTupleMapper));
        var mapper = new TupleToTupleMapper();
        var systemTuple = AotSampleData.IntCountingLongSystemTuple;
        var valueTuple = AotSampleData.IntCountingLongValueTuple;
        var namedValueTuple = AotSampleData.NamedIntCountingLongValueTuple;
        var fourElementValueTuple = AotSampleData.IntCountingLongStringValueTuple;

        report.RecordInvocation(
            nameof(TupleToTupleMapper.MapSystemTupleToSystemTuple),
            "Tuple<int, CountingValues, long>",
            "Tuple<string, string, string>",
            systemTuple,
            mapper.MapSystemTupleToSystemTuple(systemTuple));

        report.RecordInvocation(
            nameof(TupleToTupleMapper.MapTupleToTuple),
            "(int, CountingValues, long)",
            "(string, string, string)",
            valueTuple,
            mapper.MapTupleToTuple(valueTuple));

        report.RecordInvocation(
            nameof(TupleToTupleMapper.MapTupleWithNamesElementsToTupleWithNamesElements),
            "(int Alpha, CountingValues Beta, long Gamma)",
            "(string First, string Second, string Third)",
            namedValueTuple,
            mapper.MapTupleWithNamesElementsToTupleWithNamesElements(namedValueTuple));

        report.RecordInvocation(
            nameof(TupleToTupleMapper.MapSystemValueTupleToSystemValueTuple),
            "ValueTuple<int, CountingValues, long, string>",
            "ValueTuple<string, string, string, string>",
            fourElementValueTuple,
            mapper.MapSystemValueTupleToSystemValueTuple(fourElementValueTuple));
    }
}