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
    private readonly CancellationToken cancellationToken;
    private readonly bool nullableEnabled;
    private readonly bool notNullableEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public IdentityMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.compilation = compilation;
        this.cancellationToken = cancellationToken;
        this.nullableEnabled = this.context.IsNullableEnabled();
        this.notNullableEnabled = !this.nullableEnabled;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.CanMapUsingMapToSameTypeRule())
        {
            if (this.TryCreateSameTypeIdentityStrategy(out mapStrategy))
            {
                return true;
            }

            return false;
        }

        if (this.CanMapUsingMapToObjectRule() || this.CanMapUsingImplicitConversion())
        {
            mapStrategy = new IdentityMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
            return true;
        }

        return false;
    }

    private static IdentityMapDeepCopySetting GetEffectiveIdentityMapDeepCopySetting(
        IdentityMapDeepCopySetting identityMapDeepCopySetting)
        => identityMapDeepCopySetting is IdentityMapDeepCopySetting.Undefined
            ? IdentityMapDeepCopySetting.ShallowCopy
            : identityMapDeepCopySetting;

    private static bool IgnoresIdentityMapDeepCopySetting(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsString() || typeSymbol.IsEnum())
        {
            return true;
        }

        if (typeSymbol.IsValueTypeNullable())
        {
            var underlyingType = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];
            return IgnoresIdentityMapDeepCopySetting(underlyingType);
        }

        return typeSymbol is { IsValueType: true, TypeKind: not TypeKind.Struct };
    }

    private static bool IsArrayOrCollectionSameTypeRoot(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsString())
        {
            return false;
        }

        return typeSymbol.IsArray() || typeSymbol.IsOrImplementIEnumerable();
    }

    private bool TryCreateSameTypeIdentityStrategy(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var targetType = this.context.TargetType;
        var sourceType = this.context.SourceType;

        if (IgnoresIdentityMapDeepCopySetting(sourceType))
        {
            mapStrategy = new IdentityMapStrategy(targetType, sourceType);
            return true;
        }

        var effectiveSetting = GetEffectiveIdentityMapDeepCopySetting(this.context.MappaUserSettings.IdentityMapDeepCopy);
        var isStructRoot = sourceType is { TypeKind: TypeKind.Struct, IsValueType: true };

        if (IsArrayOrCollectionSameTypeRoot(sourceType))
        {
            if (effectiveSetting is IdentityMapDeepCopySetting.NestedDeepCopy)
            {
                return false;
            }

            if (effectiveSetting is IdentityMapDeepCopySetting.DeepCopy)
            {
                mapStrategy = new IdentityMapStrategy(
                    targetType,
                    sourceType,
                    effectiveSetting,
                    requiresMemberwiseClone: true);
                return true;
            }

            mapStrategy = new IdentityMapStrategy(targetType, sourceType);
            return true;
        }

        if (isStructRoot)
        {
            return this.TryCreateStructSameTypeIdentityStrategy(targetType, sourceType, effectiveSetting, out mapStrategy);
        }

        return this.TryCreateReferenceSameTypeIdentityStrategy(targetType, sourceType, effectiveSetting, out mapStrategy);
    }

    private bool TryCreateStructSameTypeIdentityStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IdentityMapDeepCopySetting effectiveSetting,
        out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(targetType, sourceType);

        if (effectiveSetting is IdentityMapDeepCopySetting.NestedDeepCopy)
        {
            if (!this.TryCreateNestedFieldStrategies(sourceType, out var nestedFieldStrategies))
            {
                return false;
            }

            mapStrategy = new IdentityMapStrategy(
                targetType,
                sourceType,
                effectiveSetting,
                isStructRoot: true,
                nestedFieldStrategies: nestedFieldStrategies);
            return true;
        }

        mapStrategy = new IdentityMapStrategy(
            targetType,
            sourceType,
            effectiveSetting,
            isStructRoot: true);
        return true;
    }

    private bool TryCreateReferenceSameTypeIdentityStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IdentityMapDeepCopySetting effectiveSetting,
        out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(targetType, sourceType);

        switch (effectiveSetting)
        {
            case IdentityMapDeepCopySetting.ShallowCopy:
                mapStrategy = new IdentityMapStrategy(targetType, sourceType);
                return true;
            case IdentityMapDeepCopySetting.DeepCopy:
                mapStrategy = new IdentityMapStrategy(
                    targetType,
                    sourceType,
                    effectiveSetting,
                    requiresMemberwiseClone: true);
                return true;
            case IdentityMapDeepCopySetting.NestedDeepCopy:
                if (!this.TryCreateNestedFieldStrategies(sourceType, out var nestedFieldStrategies))
                {
                    return false;
                }

                mapStrategy = new IdentityMapStrategy(
                    targetType,
                    sourceType,
                    effectiveSetting,
                    requiresMemberwiseClone: true,
                    nestedFieldStrategies: nestedFieldStrategies);
                return true;
            default:
                mapStrategy = new IdentityMapStrategy(targetType, sourceType);
                return true;
        }
    }

    private bool TryCreateNestedFieldStrategies(
        ITypeSymbol typeSymbol,
        out IReadOnlyList<IdentityMapNestedFieldStrategy> nestedFieldStrategies)
    {
        nestedFieldStrategies = [];
        var rootMapMethod = this.context.GetRootMapMethod();
        var within = rootMapMethod.ContainingType;
        var nestedFields = new List<IdentityMapNestedFieldStrategy>();

        foreach (var field in typeSymbol.GetAccessibleInstanceFields(this.compilation, within))
        {
            if (!this.TryGetNestedFieldStrategy(field.Type, out var fieldStrategy))
            {
                return false;
            }

            nestedFields.Add(new IdentityMapNestedFieldStrategy(field, fieldStrategy));
        }

        nestedFieldStrategies = nestedFields;
        return true;
    }

    private bool TryGetNestedFieldStrategy(ITypeSymbol fieldType, out MapStrategy fieldStrategy)
    {
        fieldStrategy = new NoMapStrategy(fieldType, fieldType);
        var derivedContext = new DerivedMappaMapAlgorithmContext(this.context, fieldType, fieldType);
        using (this.context.AlgorithmSettings.UseIdentityMapStrategyDetector.Apply(false))
        {
            var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
            fieldStrategy = algorithm.GetStrategy();
        }

        return fieldStrategy is not NoMapStrategy && !this.context.HasErrorDiagnostics;
    }

    private bool CanMapUsingMapToSameTypeRule()
        => this.CanMapSameTypeWhenNullableDisabled()
           || this.CanMapSameTypeWhenNullableEnabled()
           || this.CanMapReferenceSourceToAnnotatedSameType()
           || this.CanMapValueTypeToNullableGenericSameType();

    private bool CanMapSameTypeWhenNullableDisabled()
        => this.notNullableEnabled
           && SymbolEqualityComparer.Default.Equals(this.context.TargetType, this.context.SourceType);

    private bool CanMapSameTypeWhenNullableEnabled()
        => this.nullableEnabled
           && this.context.TargetType.IsEqualTo(this.context.SourceType, true);

    private bool CanMapReferenceSourceToAnnotatedSameType()
        => this.nullableEnabled
           && SymbolEqualityComparer.Default.Equals(this.context.TargetType, this.context.SourceType)
           && this.context.TargetType is
               { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: true };

    private bool CanMapValueTypeToNullableGenericSameType()
        => this.context.TargetType is
               { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: false }
           && this.context.TargetType.IsNullableGenericType(this.context.SourceType, this.nullableEnabled);

    private bool CanMapUsingMapToObjectRule()
        => this.CanMapToObjectWhenNullableDisabled()
           || this.CanMapToNullableObjectWhenNullableEnabled()
           || this.CanMapToNonAnnotatedObjectWhenBothNotAnnotated();

    private bool CanMapToObjectWhenNullableDisabled()
        => this.notNullableEnabled && this.context.TargetType.IsObject();

    private bool CanMapToNullableObjectWhenNullableEnabled()
        => this.nullableEnabled
           && this.context.TargetType.IsObject()
           && this.context.TargetType.NullableAnnotation == NullableAnnotation.Annotated;

    private bool CanMapToNonAnnotatedObjectWhenBothNotAnnotated()
        => this.nullableEnabled
           && this.context.TargetType.IsObject()
           && this.context.TargetType.NullableAnnotation == NullableAnnotation.NotAnnotated
           && this.context.SourceType.NullableAnnotation == NullableAnnotation.NotAnnotated;

    private bool CanMapUsingImplicitConversion()
    {
        var hasImplicitConversion = this.compilation.HasImplicitConversion(this.context.SourceType, this.context.TargetType);
        return hasImplicitConversion;
    }
}