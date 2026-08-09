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

    private static readonly Type[] ContainerOrWrapperStrategyTypes =
    [
        typeof(CollectionToCollectionMapStrategy),
        typeof(DictionaryToDictionaryMapStrategy),
        typeof(NullableStrategy),
        typeof(TupleToTupleMapStrategy),
        typeof(PolymorphismMapStrategy),
        typeof(OptionalTargetPropertyMapStrategy),
        typeof(OptionalSourcePropertyMapStrategy),
        typeof(ReadonlyDictionaryPropertyMapStrategy),
        typeof(ReadonlyCollectionPropertyMapStrategy),
        typeof(ReadonlyAddCollectionPropertyMapStrategy),
        typeof(ReadonlyQueuePropertyMapStrategy),
        typeof(ReadonlyStackPropertyMapStrategy),
        typeof(QueryableProjectionMapStrategy),
    ];

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
    /// Builds the statement that caches the reference manager in a local for the current map method.
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <returns>The local declaration statement, or <c>null</c> when reference handling is inactive.</returns>
    internal static string? BuildReferenceManagerLocalDeclaration(MappaBuilderContext context)
    {
        if (!context.IsReferenceHandlingActive)
        {
            return null;
        }

        var localName = context.GetOrCreateReferenceManagerLocalName();
        var contextParameterName = context.GetMapMethod().GetMappaContextParameterName();
        return $"global::Mappa.MappaReferenceManager {localName} = {AccessorTypeName}.{AccessorMethodName}({contextParameterName});";
    }

    /// <summary>
    /// Gets the expression that retrieves the reference manager for the current map method.
    /// </summary>
    /// <param name="context">The builder context.</param>
    /// <returns>The cached local name (or allocates one if needed).</returns>
    internal static string GetReferenceManagerExpression(MappaBuilderContext context)
        => context.GetOrCreateReferenceManagerLocalName();

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

        context.MarkEarlyReferencePairRegistered();
        return BuildAddReferencePairStatement(context, targetTemporary, source, targetType, sourceType);
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

        using (context.PushEarlyReferencePairRegistrationScope())
        {
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
                    if (!ShouldOmitExitAddReferencePair(strategy, context))
                    {
                        builder.AppendLine(BuildAddReferencePairStatement(
                            context,
                            resultTemporary,
                            source,
                            strategy.TargetType,
                            strategy.SourceType));
                    }
                }
            }
            else
            {
                AppendInnerMapping(builder, innerCode, resultTemporary, innerVariableName, increaseDepth, referenceManagerExpression);
            }

            return (resultTemporary, builder.ToString());
        }
    }

    private static string BuildAddReferencePairStatement(
        MappaBuilderContext context,
        string targetTemporary,
        string source,
        ITypeSymbol targetType,
        ITypeSymbol sourceType)
        => $"{GetReferenceManagerExpression(context)}.AddReferencePair<{GetReferenceReuseTypeArgument(targetType)}, {GetReferenceReuseTypeArgument(sourceType)}>({targetTemporary}, {source});";

    private static string GetReferenceReuseTypeArgument(ITypeSymbol type)
        => type.ToDisplayNameWithoutNullableAnnotation();

    private static bool ShouldOmitExitAddReferencePair(MapStrategy strategy, MappaBuilderContext context)
    {
        if (context.EarlyReferencePairRegistered)
        {
            return true;
        }

        return GetInvokedMapMethod(strategy) is { ReferenceReusing: BooleanSetting.Enable };
    }

    private static MapMethod? GetInvokedMapMethod(MapStrategy strategy)
        => strategy switch
        {
            MethodMapStrategy methodMap => methodMap.MapMethod,
            CompatibleMethodMapStrategy compatibleMethodMap => compatibleMethodMap.MapMethod,
            PolymorphicMethodMapStrategy polymorphicMethodMap => polymorphicMethodMap.MapMethod,
            _ => null,
        };

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

        if (IsContainerOrWrapperStrategy(strategy) || IsIdentityStrategyIneligibleForReferenceHandling(strategy))
        {
            return false;
        }

        return AreReferenceTypesEligibleForReuse(strategy.TargetType, strategy.SourceType);
    }

    private static bool IsContainerOrWrapperStrategy(MapStrategy strategy)
        => ContainerOrWrapperStrategyTypes.Any(strategyType => strategyType.IsInstanceOfType(strategy));

    private static bool ShouldIncreaseDepth(MapStrategy strategy, MappaBuilderContext context)
    {
        if (!context.IsMaxRuntimeDepthActive)
        {
            return false;
        }

        if (IsContainerOrWrapperStrategy(strategy) || IsIdentityStrategyIneligibleForReferenceHandling(strategy))
        {
            return false;
        }

        return IsReferenceTypeEligibleForRuntimeDepth(strategy.TargetType);
    }

    private static bool IsIdentityStrategyIneligibleForReferenceHandling(MapStrategy strategy)
        => strategy is IdentityMapStrategy identity
           && (identity.NestedFieldStrategies.Count > 0 || !identity.RequiresMemberwiseClone);

    private static bool IsReferenceTypeEligibleForRuntimeDepth(ITypeSymbol targetType)
    {
        if (IsIneligiblePrimitiveOrNullableReferenceType(targetType))
        {
            return false;
        }

        if (targetType.IsValueType && !targetType.IsReferenceType)
        {
            return false;
        }

        return true;
    }

    private static bool IsIneligiblePrimitiveOrNullableReferenceType(ITypeSymbol type)
        => type.IsString() || type.IsEnum() || type.IsValueTypeNullable();

    private static bool AreReferenceTypesEligibleForReuse(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        if (IsIneligiblePrimitiveOrNullableReferenceType(targetType)
            || IsIneligiblePrimitiveOrNullableReferenceType(sourceType))
        {
            return false;
        }

        if (IsNonReferenceValueType(targetType) || IsNonReferenceValueType(sourceType))
        {
            return false;
        }

        return targetType.IsReferenceType && sourceType.IsReferenceType;
    }

    private static bool IsNonReferenceValueType(ITypeSymbol type)
        => type.IsValueType && !type.IsReferenceType;
}