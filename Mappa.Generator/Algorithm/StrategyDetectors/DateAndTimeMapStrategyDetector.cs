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
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.TargetType);

        // 01. DateTime -> DateOnly.
        if (this.CanMapDateTimeToDateOnly())
        {
            mapStrategy = new DateTimeToDateOnlyMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 02. DateTime -> TimeOnly
        if (this.CanMapDateTimeToTimeOnly())
        {
            mapStrategy = new DateTimeToTimeOnlyMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 03. DateOnly -> DateTime
        if (this.CanMapDateOnlyToDateTime())
        {
            mapStrategy = new DateOnlyToDateTimeMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 04. DateTime -> long.
        if (this.CanMapDateTimeToLong())
        {
            mapStrategy = new DateTimeToLongMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // TODO [#44] long -> DateTime.
        // TODO [#44] DateOnly -> long.
        // TODO [#44] TimeSpan -> double.
        // TODO [#44] double -> TimeSpan.
        // TODO [#44] DateTimeOffset -> DateOnly.
        // TODO [#44] DateTimeOffset -> TimeOnly.
        // TODO [#44] DateTimeOffset -> long.
        // TODO [#44] long -> DateTimeOffset.
        return mapStrategy is not NoMapStrategy;
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
}