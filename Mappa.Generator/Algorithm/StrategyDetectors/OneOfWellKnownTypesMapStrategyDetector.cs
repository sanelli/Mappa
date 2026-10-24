// <copyright file="OneOfWellKnownTypesMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for OneOf / OneOfBase well-known type strategies.
/// </summary>
internal sealed class OneOfWellKnownTypesMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly bool nullableEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfWellKnownTypesMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    public OneOfWellKnownTypesMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
        this.nullableEnabled = this.context.IsNullableEnabled();
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.context.MappaUserSettings.OneOfWellKnownTypes is not BooleanSetting.Enable)
        {
            return false;
        }

        if (this.TryDetectOptionUnwrap(out mapStrategy)
            || this.TryDetectOptionWrap(out mapStrategy)
            || this.TryDetectGeneralUnwrap(out mapStrategy)
            || this.TryDetectOneOfBaseWrap(out mapStrategy))
        {
            return true;
        }

        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return false;
    }

    private static ITypeSymbol Normalize(ITypeSymbol typeSymbol)
        => typeSymbol.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

    private static bool AreSameIgnoringNullability(ITypeSymbol left, ITypeSymbol right)
        => SymbolEqualityComparer.Default.Equals(Normalize(left), Normalize(right));

    private bool TryDetectOptionUnwrap(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!OneOfWellKnownHelper.IsOneOfOptionShape(this.context.SourceType, this.compilation, out var valueType))
        {
            return false;
        }

        if (!this.TryGetOptionAbsentBehavior(valueType, out var absentBehavior))
        {
            return false;
        }

        mapStrategy = new OneOfOptionToValueMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            valueType,
            absentBehavior);
        return true;
    }

    private bool TryGetOptionAbsentBehavior(ITypeSymbol valueType, out OneOfOptionAbsentBehavior absentBehavior)
    {
        absentBehavior = OneOfOptionAbsentBehavior.Throw;
        var targetType = this.context.TargetType;

        if (targetType.IsValueTypeNullable())
        {
            if (!AreSameIgnoringNullability(targetType.GetElementType(), valueType))
            {
                return false;
            }

            absentBehavior = this.nullableEnabled
                ? OneOfOptionAbsentBehavior.ReturnNull
                : OneOfOptionAbsentBehavior.ReturnDefault;
            return true;
        }

        if (!AreSameIgnoringNullability(targetType, valueType))
        {
            return false;
        }

        if (this.nullableEnabled
            && targetType.IsReferenceType
            && targetType.NullableAnnotation == NullableAnnotation.Annotated)
        {
            absentBehavior = OneOfOptionAbsentBehavior.ReturnNull;
            return true;
        }

        if (this.nullableEnabled)
        {
            absentBehavior = OneOfOptionAbsentBehavior.Throw;
            return true;
        }

        absentBehavior = OneOfOptionAbsentBehavior.ReturnDefault;
        return true;
    }

    private bool TryDetectOptionWrap(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!OneOfWellKnownHelper.IsOneOfOptionShape(this.context.TargetType, this.compilation, out var valueType))
        {
            return false;
        }

        if (!this.TryGetOptionWrapContext(valueType, out var wrapBehavior, out var optionStructType, out var isOneOfBaseTarget)
            || optionStructType is null)
        {
            return false;
        }

        mapStrategy = new OneOfOptionFromValueMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            valueType,
            optionStructType,
            wrapBehavior,
            isOneOfBaseTarget);
        return true;
    }

    private bool TryGetOptionWrapContext(
        ITypeSymbol valueType,
        out OneOfOptionWrapBehavior wrapBehavior,
        out INamedTypeSymbol? optionStructType,
        out bool isOneOfBaseTarget)
    {
        wrapBehavior = OneOfOptionWrapBehavior.AlwaysSome;
        optionStructType = null;
        isOneOfBaseTarget = false;

        if (!this.TryResolveOptionStructTarget(out optionStructType, out isOneOfBaseTarget)
            || optionStructType is null)
        {
            return false;
        }

        // Option FromValue builder emits new Target(OneOf) for OneOfBase; require that ctor.
        // Implicit-only AlwaysSome can still succeed via the earlier identity detector.
        if (isOneOfBaseTarget
            && (!OneOfWellKnownHelper.TryGetOneOfBaseWrapConstruction(
                    this.context.TargetType,
                    armIndex: 0,
                    this.compilation,
                    out var constructionKind,
                    out _)
                || constructionKind is not OneOfBaseConstructionKind.ConstructorFromOneOf))
        {
            return false;
        }

        return this.TryGetOptionWrapBehavior(valueType, out wrapBehavior);
    }

    private bool TryResolveOptionStructTarget(out INamedTypeSymbol? optionStructType, out bool isOneOfBaseTarget)
    {
        optionStructType = null;
        isOneOfBaseTarget = false;
        var targetType = this.context.TargetType;

        if (OneOfWellKnownHelper.IsOneOf(targetType) && targetType is INamedTypeSymbol oneOfType)
        {
            optionStructType = oneOfType;
            return true;
        }

        if (OneOfWellKnownHelper.IsOneOfBase(targetType)
            && OneOfWellKnownHelper.TryGetOptionStructTypeForOneOfBase(targetType, this.compilation, out var resolved)
            && resolved is not null)
        {
            optionStructType = resolved;
            isOneOfBaseTarget = true;
            return true;
        }

        return false;
    }

    private bool TryGetOptionWrapBehavior(ITypeSymbol valueType, out OneOfOptionWrapBehavior wrapBehavior)
    {
        wrapBehavior = OneOfOptionWrapBehavior.AlwaysSome;
        var sourceType = this.context.SourceType;

        if (sourceType.IsValueTypeNullable())
        {
            if (!AreSameIgnoringNullability(sourceType.GetElementType(), valueType))
            {
                return false;
            }

            wrapBehavior = OneOfOptionWrapBehavior.SomeWhenHasValue;
            return true;
        }

        if (!AreSameIgnoringNullability(sourceType, valueType))
        {
            return false;
        }

        if (this.nullableEnabled
            && sourceType.IsReferenceType
            && sourceType.NullableAnnotation == NullableAnnotation.Annotated)
        {
            wrapBehavior = OneOfOptionWrapBehavior.SomeWhenNotNull;
            return true;
        }

        if (this.nullableEnabled)
        {
            wrapBehavior = OneOfOptionWrapBehavior.AlwaysSome;
            return true;
        }

        if (sourceType.IsReferenceType)
        {
            wrapBehavior = OneOfOptionWrapBehavior.SomeWhenNotNull;
            return true;
        }

        wrapBehavior = OneOfOptionWrapBehavior.SomeWhenNotDefault;
        return true;
    }

    private bool TryDetectGeneralUnwrap(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!OneOfWellKnownHelper.TryGetOneOfOrOneOfBaseTypeArguments(this.context.SourceType, out var armTypes))
        {
            return false;
        }

        // Option → exact T / T? is handled by Option unwrap; still allow Option → assignable S here.
        if (OneOfWellKnownHelper.IsOneOfOptionShape(this.context.SourceType, this.compilation, out var valueType)
            && this.IsExactOrNullableOfValue(valueType))
        {
            return false;
        }

        if (!OneOfWellKnownHelper.TrySelectUniqueArmIndex(armTypes, this.context.TargetType, out var armIndex))
        {
            return false;
        }

        mapStrategy = new OneOfToValueMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            armIndex,
            armTypes[armIndex]);
        return true;
    }

    private bool IsExactOrNullableOfValue(ITypeSymbol valueType)
    {
        var targetType = this.context.TargetType;
        if (targetType.IsValueTypeNullable())
        {
            return AreSameIgnoringNullability(targetType.GetElementType(), valueType);
        }

        return AreSameIgnoringNullability(targetType, valueType);
    }

    private bool TryDetectOneOfBaseWrap(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Option shape wrap is handled separately; plain OneOf<> wrap stays with identity.
        if (OneOfWellKnownHelper.IsOneOf(this.context.TargetType)
            || !OneOfWellKnownHelper.IsOneOfBase(this.context.TargetType)
            || OneOfWellKnownHelper.IsOneOfOptionShape(this.context.TargetType, this.compilation, out _))
        {
            return false;
        }

        if (!OneOfWellKnownHelper.TryGetOneOfBaseTypeArguments(this.context.TargetType, out var armTypes)
            || !OneOfWellKnownHelper.TrySelectUniqueArmIndexForWrap(armTypes, this.context.SourceType, out var armIndex)
            || !OneOfWellKnownHelper.TryGetOneOfBaseWrapConstruction(
                this.context.TargetType,
                armIndex,
                this.compilation,
                out var constructionKind,
                out var optionStructType))
        {
            return false;
        }

        mapStrategy = new OneOfBaseFromValueMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            armIndex,
            armTypes[armIndex],
            constructionKind,
            optionStructType);
        return true;
    }
}