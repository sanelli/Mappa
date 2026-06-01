// <copyright file="GuidStrategyMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="GuidStrategyMapper"/>.
/// </summary>
internal static class GuidStrategyMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="GuidStrategyMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(GuidStrategyMapper));
        var mapper = new GuidStrategyMapper();
        var guid = AotSampleData.SampleGuid;
        var bytes = AotSampleData.SampleGuidBytes;
        Span<byte> span = bytes;
        ReadOnlySpan<byte> readOnlySpan = bytes;
        Memory<byte> memory = bytes;
        ReadOnlyMemory<byte> readOnlyMemory = bytes;

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapFromGuidToArray),
            "Guid",
            "byte[]",
            guid,
            mapper.MapFromGuidToArray(guid));

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapFromGuidToSpan),
            "Guid",
            "Span<byte>",
            guid,
            mapper.MapFromGuidToSpan(guid).ToArray());

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapFromGuidToReadOnlySpan),
            "Guid",
            "ReadOnlySpan<byte>",
            guid,
            mapper.MapFromGuidToReadOnlySpan(guid).ToArray());

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapFromGuidToMemory),
            "Guid",
            "Memory<byte>",
            guid,
            mapper.MapFromGuidToMemory(guid).ToArray());

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapFromGuidToReadOnlyMemory),
            "Guid",
            "ReadOnlyMemory<byte>",
            guid,
            mapper.MapFromGuidToReadOnlyMemory(guid).ToArray());

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapArrayToGuid),
            "byte[]",
            "Guid",
            bytes,
            mapper.MapArrayToGuid(bytes));

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapSpanToGuid),
            "Span<byte>",
            "Guid",
            bytes,
            mapper.MapSpanToGuid(span));

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapReadOnlySpanToGuid),
            "ReadOnlySpan<byte>",
            "Guid",
            bytes,
            mapper.MapReadOnlySpanToGuid(readOnlySpan));

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapMemoryToGuid),
            "Memory<byte>",
            "Guid",
            bytes,
            mapper.MapMemoryToGuid(memory));

        report.RecordInvocation(
            nameof(GuidStrategyMapper.MapReadOnlyMemoryToGuid),
            "ReadOnlyMemory<byte>",
            "Guid",
            bytes,
            mapper.MapReadOnlyMemoryToGuid(readOnlyMemory));
    }
}