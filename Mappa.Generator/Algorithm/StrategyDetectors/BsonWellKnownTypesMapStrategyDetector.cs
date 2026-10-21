// <copyright file="BsonWellKnownTypesMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for BSON well-known type strategies.
/// </summary>
internal sealed class BsonWellKnownTypesMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="BsonWellKnownTypesMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation settings.</param>
    public BsonWellKnownTypesMapStrategyDetector(
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

        if (this.context.MappaUserSettings.BsonWellKnownTypes is not BooleanSetting.Enable)
        {
            return false;
        }

        foreach (var candidate in this.GetBsonMappingCandidates())
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

    private IEnumerable<(Func<bool> CanMap, Func<MapStrategy> CreateStrategy)> GetBsonMappingCandidates()
    {
        var targetType = this.context.TargetType;
        var sourceType = this.context.SourceType;
        yield return (this.CanMapStringToObjectId, () => new StringToObjectIdMapStrategy(targetType, sourceType));
        yield return (this.CanMapObjectIdToString, () => new ObjectIdToStringMapStrategy(targetType, sourceType));
        yield return (this.CanMapByteArrayToObjectId, () => new ByteArrayToObjectIdMapStrategy(targetType, sourceType));
        yield return (this.CanMapObjectIdToByteArray, () => new ObjectIdToByteArrayMapStrategy(targetType, sourceType));
        yield return (this.CanMapDecimal128ToDecimal, () => new Decimal128ToDecimalMapStrategy(targetType, sourceType));
    }

    private bool CanMapStringToObjectId()
        => this.context.SourceType.IsString() && this.context.TargetType.IsObjectId(this.compilation);

    private bool CanMapObjectIdToString()
        => this.context.SourceType.IsObjectId(this.compilation) && this.context.TargetType.IsString();

    private bool CanMapByteArrayToObjectId()
        => this.context.SourceType.IsArray()
           && this.context.SourceType.GetElementType().IsByte()
           && this.context.TargetType.IsObjectId(this.compilation);

    private bool CanMapObjectIdToByteArray()
        => this.context.SourceType.IsObjectId(this.compilation)
           && this.context.TargetType.IsArray()
           && this.context.TargetType.GetElementType().IsByte();

    private bool CanMapDecimal128ToDecimal()
        => this.context.SourceType.IsDecimal128(this.compilation) && this.context.TargetType.IsDecimal();
}