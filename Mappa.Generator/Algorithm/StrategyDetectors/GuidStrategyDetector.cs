// <copyright file="GuidStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Strategy for some <see cref="Guid"/> mappings.
/// </summary>
internal sealed class GuidStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuidStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    public GuidStrategyDetector(
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

        // 1. Guid -> byte[] or Span<byte> or ReadOnlySpan<byte> or Memory<byte> or ReadOnlyMemory<byte>
        if (this.CanMapFromGuidToArray())
        {
            // TODO [#47] Implement me.
        }

        // 2. byte[] or Span<byte> or ReadOnlySpan<byte> or ReadOnlyMemory<byte> or Memory<byte?-> Guid
        else if (this.CanMapFromArrayToGuid())
        {
            // TODO [#47] Implement me.
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapFromGuidToArray()
    {
        var sourceIsGuid = this.context.SourceType.IsGuid(this.compilation);
        var targetIsArrayLike = this.context.TargetType.IsArray()
                                || this.context.TargetType.IsSpan(this.compilation)
                                || this.context.TargetType.IsReadOnlySpan(this.compilation)
                                || this.context.TargetType.IsMemory(this.compilation)
                                || this.context.TargetType.IsReadOnlyMemory(this.compilation);
        var isElementTypeByte = targetIsArrayLike && this.context.TargetType.GetElementType().IsByte();

        return sourceIsGuid && targetIsArrayLike && isElementTypeByte;
    }

    private bool CanMapFromArrayToGuid()
    {
        var targetIsGuid = this.context.TargetType.IsGuid(this.compilation);
        var sourceIsArrayLike = this.context.SourceType.IsArray()
                                || this.context.SourceType.IsSpan(this.compilation)
                                || this.context.SourceType.IsReadOnlySpan(this.compilation)
                                || this.context.SourceType.IsMemory(this.compilation)
                                || this.context.SourceType.IsReadOnlyMemory(this.compilation);
        var isElementTypeByte = sourceIsArrayLike && this.context.SourceType.GetElementType().IsByte();

        return targetIsGuid && sourceIsArrayLike && isElementTypeByte;
    }
}