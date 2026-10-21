// <copyright file="ProtobufWellKnownTypesMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for Protobuf well-known type strategies.
/// </summary>
internal sealed class ProtobufWellKnownTypesMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufWellKnownTypesMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation settings.</param>
    public ProtobufWellKnownTypesMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.context.MappaUserSettings.ProtobufWellKnownTypes is not BooleanSetting.Enable)
        {
            return false;
        }

        foreach (var candidate in this.GetProtobufMappingCandidates())
#pragma warning disable S3267 // Loops should be simplified using the "Where" LINQ method
        {
            if (!candidate.CanMap())
            {
                continue;
            }

            mapStrategy = candidate.CreateStrategy();
            return true;
        }
#pragma warning restore S3267 // Loops should be simplified using the "Where" LINQ method

        return false;
    }

    private IEnumerable<(Func<bool> CanMap, Func<MapStrategy> CreateStrategy)> GetProtobufMappingCandidates()
    {
        var targetType = this.context.TargetType;
        var sourceType = this.context.SourceType;
        yield return (this.CanMapTimestampToDateTime, () => new ProtobufTimestampToDateTimeMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimestampToDateTimeOffset, () => new ProtobufTimestampToDateTimeOffsetMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimestampToDateOnly, () => new ProtobufTimestampToDateOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimestampToTimeOnly, () => new ProtobufTimestampToTimeOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeToTimestamp, () => new DateTimeToProtobufTimestampMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeOffsetToTimestamp, () => new DateTimeOffsetToProtobufTimestampMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateOnlyToTimestamp, () => new DateOnlyToProtobufTimestampMapStrategy(targetType, sourceType));
        yield return (this.CanMapDurationToTimeSpan, () => new ProtobufDurationToTimeSpanMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimeSpanToDuration, () => new TimeSpanToProtobufDurationMapStrategy(targetType, sourceType));
    }

    private bool CanMapTimestampToDateTime()
    {
        var sourceIsTimestamp = this.context.SourceType.IsProtobufTimestamp(this.compilation);
        var targetIsDateTime = this.context.TargetType.IsDateTime();
        return sourceIsTimestamp && targetIsDateTime;
    }

    private bool CanMapTimestampToDateTimeOffset()
    {
        var sourceIsTimestamp = this.context.SourceType.IsProtobufTimestamp(this.compilation);
        var targetIsDateTimeOffset = this.context.TargetType.IsDateTimeOffset(this.compilation);
        return sourceIsTimestamp && targetIsDateTimeOffset;
    }

    private bool CanMapTimestampToDateOnly()
    {
        var sourceIsTimestamp = this.context.SourceType.IsProtobufTimestamp(this.compilation);
        var targetIsDateOnly = this.context.TargetType.IsDateOnly(this.compilation);
        return sourceIsTimestamp && targetIsDateOnly;
    }

    private bool CanMapTimestampToTimeOnly()
    {
        var sourceIsTimestamp = this.context.SourceType.IsProtobufTimestamp(this.compilation);
        var targetIsTimeOnly = this.context.TargetType.IsTimeOnly(this.compilation);
        return sourceIsTimestamp && targetIsTimeOnly;
    }

    private bool CanMapDateTimeToTimestamp()
    {
        var sourceIsDateTime = this.context.SourceType.IsDateTime();
        var targetIsTimestamp = this.context.TargetType.IsProtobufTimestamp(this.compilation);
        return sourceIsDateTime && targetIsTimestamp;
    }

    private bool CanMapDateTimeOffsetToTimestamp()
    {
        var sourceIsDateTimeOffset = this.context.SourceType.IsDateTimeOffset(this.compilation);
        var targetIsTimestamp = this.context.TargetType.IsProtobufTimestamp(this.compilation);
        return sourceIsDateTimeOffset && targetIsTimestamp;
    }

    private bool CanMapDateOnlyToTimestamp()
    {
        var sourceIsDateOnly = this.context.SourceType.IsDateOnly(this.compilation);
        var targetIsTimestamp = this.context.TargetType.IsProtobufTimestamp(this.compilation);
        return sourceIsDateOnly && targetIsTimestamp;
    }

    private bool CanMapDurationToTimeSpan()
    {
        var sourceIsDuration = this.context.SourceType.IsProtobufDuration(this.compilation);
        var targetIsTimeSpan = this.context.TargetType.IsTimeSpan(this.compilation);
        return sourceIsDuration && targetIsTimeSpan;
    }

    private bool CanMapTimeSpanToDuration()
    {
        var sourceIsTimeSpan = this.context.SourceType.IsTimeSpan(this.compilation);
        var targetIsDuration = this.context.TargetType.IsProtobufDuration(this.compilation);
        return sourceIsTimeSpan && targetIsDuration;
    }
}