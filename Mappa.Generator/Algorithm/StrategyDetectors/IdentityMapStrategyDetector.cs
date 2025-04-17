// <copyright file="IdentityMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detect if strategy <see cref="IdentityMapStrategy"/> can be applied.
/// </summary>
internal sealed class IdentityMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly bool nullableEnabled;
    private readonly bool notNullableEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    public IdentityMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
        this.nullableEnabled = this.context.IsNullableEnabled();
        this.notNullableEnabled = !this.nullableEnabled;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. Map to the very same type.
        // 02. Map to object
        // 03. Implicit conversion.
        if (this.CanMapUsingMapToSameTypeRule()
            || this.CanMapUsingMapToObjectRule()
            || this.CanMapUsingImplicitConversion())
        {
            // TODO [#14] Add support for deep copy instead of shallow copy when the type is the same via attribute.
           mapStrategy = new IdentityMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapUsingMapToSameTypeRule()
    {
        // (non-nullable): T -> T
        // (nullable): T -> T or T? -> T?
        // (nullable) T -> T? && T is refType
        // (nullable || not-nullable) T -> T? && T is not refType
        return (this.notNullableEnabled &&
                SymbolEqualityComparer.Default.Equals(this.context.TargetType, this.context.SourceType))
               || (this.nullableEnabled &&
                   this.context.TargetType.IsEqualTo(this.context.SourceType, true))
               || (this.nullableEnabled && SymbolEqualityComparer.Default.Equals(
                                           this.context.TargetType,
                                           this.context.SourceType)
                                           && this.context.TargetType is
                                               { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: true })
               || (this.context.TargetType is
                       { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: false }
                   && this.context.TargetType.IsNullableGenericType(this.context.SourceType, this.nullableEnabled));
    }

    private bool CanMapUsingMapToObjectRule()
    {
        // (non-nullable): T -> object
        // (nullable): T -> object?
        // (nullable) T -> object (When T is not NOT nullable annotated)
        return (this.notNullableEnabled && this.context.TargetType.IsObject())
               || (this.nullableEnabled && this.context.TargetType.IsObject() &&
                   this.context.TargetType.NullableAnnotation == NullableAnnotation.Annotated)
               || (this.nullableEnabled
                   && this.context.TargetType.IsObject()
                   && this.context.TargetType.NullableAnnotation == NullableAnnotation.NotAnnotated
                   && this.context.SourceType.NullableAnnotation == NullableAnnotation.NotAnnotated);
    }

    private bool CanMapUsingImplicitConversion()
    {
        var hasImplicitConversion = this.compilation.HasImplicitConversion(this.context.SourceType, this.context.TargetType);
        return hasImplicitConversion;
    }
}