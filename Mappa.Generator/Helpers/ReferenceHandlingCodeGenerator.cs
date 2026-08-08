// <copyright file="ReferenceHandlingCodeGenerator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Helpers for emitting runtime reference-handling code (MaxRuntimeDepth and ReferenceReusing).
/// </summary>
internal static class ReferenceHandlingCodeGenerator
{
    /// <summary>
    /// Gets the file-local accessor type name for <see cref="MappaContext"/>'s private reference manager.
    /// </summary>
    internal const string AccessorTypeName = "__MappaContextReferenceManagerAccessor";

    /// <summary>
    /// Gets the accessor method name that returns the private reference manager.
    /// </summary>
    internal const string AccessorMethodName = "GetReferenceManager";

    /// <summary>
    /// Returns <c>true</c> when ReferenceReusing or MaxRuntimeDepth is requested on <paramref name="settings"/>.
    /// </summary>
    /// <param name="settings">The user settings.</param>
    /// <returns><c>true</c> when reference handling is requested.</returns>
    internal static bool IsReferenceHandlingRequested(IMappaUserSettings settings)
        => settings.ReferenceReusing is BooleanSetting.Enable || settings.MaxRuntimeDepth > 0;

    /// <summary>
    /// Returns <c>true</c> when ReferenceReusing or MaxRuntimeDepth is requested on <paramref name="mapMethod"/>.
    /// </summary>
    /// <param name="mapMethod">The map method.</param>
    /// <returns><c>true</c> when reference handling is requested.</returns>
    internal static bool IsReferenceHandlingRequested(MapMethod mapMethod)
        => mapMethod.ReferenceReusing is BooleanSetting.Enable || mapMethod.MaxRuntimeDepth > 0;

    /// <summary>
    /// Builds the file-local UnsafeAccessor type used to reach <c>MappaContext.ReferenceManager</c>.
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <returns>The accessor source, or an empty string when not required.</returns>
    internal static string BuildAccessorSource(MappaBuilderContext context)
    {
        if (!context.ReferenceManagerAccessorRequired)
        {
            return string.Empty;
        }

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"file static class {AccessorTypeName}");
        using (builder.CurlyBracesBlock())
        {
            builder.AppendLine("[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = \"get_ReferenceManager\")]");
            builder.AppendLine($"public static extern global::Mappa.MappaReferenceManager {AccessorMethodName}(global::Mappa.MappaContext context);");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Gets the expression that retrieves the reference manager for the current map method's context parameter.
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <returns>The accessor invocation expression.</returns>
    internal static string GetReferenceManagerExpression(MappaBuilderContext context)
    {
        var contextParameterName = context.GetMapMethod().GetMappaContextParameterName();
        return $"{AccessorTypeName}.{AccessorMethodName}({contextParameterName})";
    }

    /// <summary>
    /// Builds the statement that assigns <see cref="MappaReferenceManager.MaxDepth"/> for the root map method.
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <returns>The initialization statement, or <c>null</c> when MaxRuntimeDepth is inactive.</returns>
    internal static string? BuildMaxRuntimeDepthInitialization(MappaBuilderContext context)
    {
        if (!context.IsMaxRuntimeDepthActive)
        {
            return null;
        }

        return $"{GetReferenceManagerExpression(context)}.MaxDepth = {context.EffectiveMaxRuntimeDepth};";
    }

    /// <summary>
    /// Returns <c>true</c> when an early <c>AddReferencePair</c> should be emitted after constructing a target.
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <returns><c>true</c> when the pair should be registered early.</returns>
    internal static bool ShouldRegisterReferencePairEarly(
        MappaBuilderContext context,
        string source,
        ITypeSymbol targetType,
        ITypeSymbol sourceType)
        => context.IsReferenceReusingActive
           && !string.IsNullOrWhiteSpace(source)
           && AreReferenceTypesEligibleForReuse(targetType, sourceType);

    /// <summary>
    /// Builds the statement that registers a source/target pair after the target instance exists
    /// (used early after construction so cycles can resolve).
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <param name="targetTemporary">The target temporary variable.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <returns>The AddReferencePair statement, or <c>null</c> when reusing does not apply.</returns>
    internal static string? BuildEarlyAddReferencePairStatement(
        MappaBuilderContext context,
        string targetTemporary,
        string source,
        ITypeSymbol targetType,
        ITypeSymbol sourceType)
    {
        if (!ShouldRegisterReferencePairEarly(context, source, targetType, sourceType))
        {
            return null;
        }

        return $"{GetReferenceManagerExpression(context)}.AddReferencePair({targetTemporary}, {source});";
    }

    /// <summary>
    /// Builds nested mapping source with optional ReferenceReusing and MaxRuntimeDepth wraps.
    /// </summary>
    /// <param name="strategy">The nested strategy.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="context">The builder context.</param>
    /// <param name="mappaGlobalOptions">The global options.</param>
    /// <returns>The mapped variable name and supporting code.</returns>
    internal static (string VariableName, string Code) BuildNestedSource(
        MapStrategy strategy,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
        => BuildWithReferenceHandling(
            strategy,
            source,
            context,
            mappaGlobalOptions,
            increaseDepth: ShouldIncreaseDepth(strategy, context));

    /// <summary>
    /// Builds root mapping source with optional ReferenceReusing (never increases runtime depth).
    /// </summary>
    /// <param name="strategy">The root strategy.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="context">The builder context.</param>
    /// <param name="mappaGlobalOptions">The global options.</param>
    /// <returns>The mapped variable name and supporting code.</returns>
    internal static (string VariableName, string Code) BuildRootSource(
        MapStrategy strategy,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
        => BuildWithReferenceHandling(
            strategy,
            source,
            context,
            mappaGlobalOptions,
            increaseDepth: false);

    private static (string VariableName, string Code) BuildWithReferenceHandling(
        MapStrategy strategy,
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        bool increaseDepth)
    {
        var reuse = ShouldReuseReferences(strategy, context, source);
        if (!reuse && !increaseDepth)
        {
            return strategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
        }

        var (innerVariableName, innerCode) = strategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
        var builder = new PrettyCode.StringBuilder();
        var resultTemporary = context.NextTemporary();
        var targetTypeDisplay = strategy.TargetType.ToDisplayString();
        var referenceManagerExpression = GetReferenceManagerExpression(context);

        builder.AppendLine($"{targetTypeDisplay} {resultTemporary};");
        if (reuse)
        {
            builder.AppendLine($"if (!{referenceManagerExpression}.TryGetReference<{targetTypeDisplay}>({source}, out {resultTemporary}))");
            using (builder.CurlyBracesBlock())
            {
                AppendInnerMapping(builder, innerCode, resultTemporary, innerVariableName, increaseDepth, referenceManagerExpression);
                builder.AppendLine($"{referenceManagerExpression}.AddReferencePair({resultTemporary}, {source});");
            }
        }
        else
        {
            AppendInnerMapping(builder, innerCode, resultTemporary, innerVariableName, increaseDepth, referenceManagerExpression);
        }

        return (resultTemporary, builder.ToString());
    }

    private static void AppendInnerMapping(
        PrettyCode.StringBuilder builder,
        string innerCode,
        string resultTemporary,
        string innerVariableName,
        bool increaseDepth,
        string referenceManagerExpression)
    {
        if (increaseDepth)
        {
            builder.AppendLine($"using ({referenceManagerExpression}.IncreaseDepth())");
            using (builder.CurlyBracesBlock())
            {
                AppendInnerCode(builder, innerCode, resultTemporary, innerVariableName);
            }
        }
        else
        {
            AppendInnerCode(builder, innerCode, resultTemporary, innerVariableName);
        }
    }

    private static void AppendInnerCode(
        PrettyCode.StringBuilder builder,
        string innerCode,
        string resultTemporary,
        string innerVariableName)
    {
        if (!string.IsNullOrWhiteSpace(innerCode))
        {
            builder.AppendLine(innerCode);
        }

        builder.AppendLine($"{resultTemporary} = {innerVariableName};");
    }

    private static bool ShouldReuseReferences(MapStrategy strategy, MappaBuilderContext context, string source)
    {
        if (!context.IsReferenceReusingActive || string.IsNullOrWhiteSpace(source))
        {
            return false;
        }

        if (IsContainerOrWrapperStrategy(strategy))
        {
            return false;
        }

        if (strategy is IdentityMapStrategy identity
            && (identity.NestedFieldStrategies.Count > 0 || !identity.RequiresMemberwiseClone))
        {
            return false;
        }

        return AreReferenceTypesEligibleForReuse(strategy.TargetType, strategy.SourceType);
    }

    private static bool ShouldIncreaseDepth(MapStrategy strategy, MappaBuilderContext context)
    {
        if (!context.IsMaxRuntimeDepthActive)
        {
            return false;
        }

        if (IsContainerOrWrapperStrategy(strategy))
        {
            return false;
        }

        if (strategy is IdentityMapStrategy identity
            && (identity.NestedFieldStrategies.Count > 0 || !identity.RequiresMemberwiseClone))
        {
            return false;
        }

        var targetType = strategy.TargetType;
        if (targetType.IsString() || targetType.IsEnum())
        {
            return false;
        }

        if (targetType.IsValueTypeNullable())
        {
            return false;
        }

        if (targetType.IsValueType && !targetType.IsReferenceType)
        {
            return false;
        }

        return true;
    }

    private static bool IsContainerOrWrapperStrategy(MapStrategy strategy)
        => strategy is CollectionToCollectionMapStrategy
            or DictionaryToDictionaryMapStrategy
            or NullableStrategy
            or TupleToTupleMapStrategy
            or PolymorphismMapStrategy
            or OptionalTargetPropertyMapStrategy
            or OptionalSourcePropertyMapStrategy
            or ReadonlyDictionaryPropertyMapStrategy
            or ReadonlyCollectionPropertyMapStrategy
            or ReadonlyAddCollectionPropertyMapStrategy
            or ReadonlyQueuePropertyMapStrategy
            or ReadonlyStackPropertyMapStrategy
            or QueryableProjectionMapStrategy;

    private static bool AreReferenceTypesEligibleForReuse(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        if (targetType.IsString() || sourceType.IsString()
            || targetType.IsEnum() || sourceType.IsEnum())
        {
            return false;
        }

        if (targetType.IsValueTypeNullable() || sourceType.IsValueTypeNullable())
        {
            return false;
        }

        if ((targetType.IsValueType && !targetType.IsReferenceType)
            || (sourceType.IsValueType && !sourceType.IsReferenceType))
        {
            return false;
        }

        return targetType.IsReferenceType && sourceType.IsReferenceType;
    }
}