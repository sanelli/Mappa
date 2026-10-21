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
    }

    private bool CanMapStringToObjectId()
    {
        var sourceIsString = this.context.SourceType.IsString();
        var targetIsObjectId = this.context.TargetType.IsObjectId(this.compilation);
        return sourceIsString && targetIsObjectId;
    }

    private bool CanMapObjectIdToString()
    {
        var sourceIsObjectId = this.context.SourceType.IsObjectId(this.compilation);
        var targetIsString = this.context.TargetType.IsString();
        return sourceIsObjectId && targetIsString;
    }

    private bool CanMapByteArrayToObjectId()
    {
        var sourceIsByteArray = this.context.SourceType.IsArray() && this.context.SourceType.GetElementType().IsByte();
        var targetIsObjectId = this.context.TargetType.IsObjectId(this.compilation);
        return sourceIsByteArray && targetIsObjectId;
    }

    private bool CanMapObjectIdToByteArray()
    {
        var sourceIsObjectId = this.context.SourceType.IsObjectId(this.compilation);
        var targetIsByteArray = this.context.TargetType.IsArray() && this.context.TargetType.GetElementType().IsByte();
        return sourceIsObjectId && targetIsByteArray;
    }
}