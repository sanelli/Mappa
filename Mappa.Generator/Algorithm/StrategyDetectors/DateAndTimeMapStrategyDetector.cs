// <copyright file="DateAndTimeMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for date and time related strategies.
/// </summary>
internal sealed class DateAndTimeMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateAndTimeMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation settings.</param>
    public DateAndTimeMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.TargetType);

        foreach (var candidate in this.GetDateTimeMappingCandidates())
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

    private IEnumerable<(Func<bool> CanMap, Func<MapStrategy> CreateStrategy)> GetDateTimeMappingCandidates()
    {
        var targetType = this.context.TargetType;
        var sourceType = this.context.SourceType;
        yield return (this.CanMapDateTimeToDateOnly, () => new DateTimeToDateOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeToTimeOnly, () => new DateTimeToTimeOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateOnlyToDateTime, () => new DateOnlyToDateTimeMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeToLong, () => new DateTimeToLongMapStrategy(targetType, sourceType));
        yield return (this.CanMapLongOrSmallerNumericTypeToDateTime, () => new LongToDateTimeMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateOnlyToLong, () => new DateOnlyToLongMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimeSpanToDouble, () => new TimeSpanToDoubleMapStrategy(targetType, sourceType));
        yield return (this.CanMapDoubleToTimeSpan, () => new DoubleToTimeSpanMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeOffsetToDateOnly, () => new DateTimeOffsetToDateOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeOffsetToTimeOnly, () => new DateTimeOffsetToTimeOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeOffsetToLong, () => new DateTimeOffsetToLongMapStrategy(targetType, sourceType));
        yield return (this.CanMapLongOrSmallerNumericTypeToDateTimeOffset, () => new LongToDateTimeOffsetMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeOffsetToDateTime, () => new DateTimeOffsetToDateTimeMapStrategy(targetType, sourceType));
    }

    private bool CanMapDateTimeToDateOnly()
    {
        var sourceIsDateTime = this.context.SourceType.IsDateTime();
        var targetIsDateOnly = this.context.TargetType.IsDateOnly(this.compilation);
        return sourceIsDateTime && targetIsDateOnly;
    }

    private bool CanMapDateTimeToTimeOnly()
    {
        var sourceIsDateTime = this.context.SourceType.IsDateTime();
        var targetIsTimeOnly = this.context.TargetType.IsTimeOnly(this.compilation);
        return sourceIsDateTime && targetIsTimeOnly;
    }

    private bool CanMapDateOnlyToDateTime()
    {
        var sourceIsDateOnly = this.context.TargetType.IsDateTime();
        var targetIsDateTime = this.context.SourceType.IsDateOnly(this.compilation);
        return sourceIsDateOnly && targetIsDateTime;
    }

    private bool CanMapDateTimeToLong()
    {
        var sourceIsDateTime = this.context.SourceType.IsDateTime();
        var targetIsLong = this.context.TargetType.IsLong();
        return sourceIsDateTime && targetIsLong;
    }

    private bool CanMapLongOrSmallerNumericTypeToDateTime()
    {
        var sourceIsLongOrSmallerNumericType = this.context.SourceType.IsLongOrNumericCanBeImplicitlyCastedToLong();
        var targetIsDateTime = this.context.TargetType.IsDateTime();
        return sourceIsLongOrSmallerNumericType && targetIsDateTime;
    }

    private bool CanMapDateOnlyToLong()
    {
        var sourceIsDateOnly = this.context.SourceType.IsDateOnly(this.compilation);
        var targetIsLong = this.context.TargetType.IsLong();
        return sourceIsDateOnly && targetIsLong;
    }

    private bool CanMapTimeSpanToDouble()
    {
        var sourceIsDateOnly = this.context.SourceType.IsTimeSpan(this.compilation);
        var targetIsLong = this.context.TargetType.IsDouble();
        return sourceIsDateOnly && targetIsLong;
    }

    private bool CanMapDoubleToTimeSpan()
    {
        var targetIsTimeSpan = this.context.TargetType.IsTimeSpan(this.compilation);
        var sourceIsDouble = this.context.SourceType.IsDoubleOrNumericImplicitlyConvertible();
        return targetIsTimeSpan && sourceIsDouble;
    }

    private bool CanMapDateTimeOffsetToDateOnly()
    {
        var sourceIsDateTimeOffset = this.context.SourceType.IsDateTimeOffset(this.compilation);
        var targetIsDateOnly = this.context.TargetType.IsDateOnly(this.compilation);
        return sourceIsDateTimeOffset && targetIsDateOnly;
    }

    private bool CanMapDateTimeOffsetToTimeOnly()
    {
        var sourceIsDateTimeOffset = this.context.SourceType.IsDateTimeOffset(this.compilation);
        var targetIsTimeOnly = this.context.TargetType.IsTimeOnly(this.compilation);
        return sourceIsDateTimeOffset && targetIsTimeOnly;
    }

    private bool CanMapDateTimeOffsetToLong()
    {
        var sourceIsDateTimeOffset = this.context.SourceType.IsDateTimeOffset(this.compilation);
        var targetIsLong = this.context.TargetType.IsLong();
        return sourceIsDateTimeOffset && targetIsLong;
    }

    private bool CanMapLongOrSmallerNumericTypeToDateTimeOffset()
    {
        var sourceIsLongOrSmallerNumericType = this.context.SourceType.IsLongOrNumericCanBeImplicitlyCastedToLong();
        var targetIsDateTimeOffset = this.context.TargetType.IsDateTimeOffset(this.compilation);
        return sourceIsLongOrSmallerNumericType && targetIsDateTimeOffset;
    }

    private bool CanMapDateTimeOffsetToDateTime()
    {
        var sourceIsDateTimeOffset = this.context.SourceType.IsDateTimeOffset(this.compilation);
        var targetIsDateTime = this.context.TargetType.IsDateTime();
        return sourceIsDateTimeOffset && targetIsDateTime;
    }
}