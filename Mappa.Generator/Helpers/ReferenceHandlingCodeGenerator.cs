// <copyright file="ReferenceHandlingCodeGenerator.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Helpers for emitting runtime reference-handling code (MaxRuntimeDepth and later ReferenceReusing).
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
    /// Builds nested mapping source, wrapping with <c>using (IncreaseDepth())</c> when MaxRuntimeDepth is active
    /// and the strategy represents a nested reference-type mapping.
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
    {
        var (innerVariableName, innerCode) = strategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
        if (!ShouldIncreaseDepth(strategy, context))
        {
            return (innerVariableName, innerCode);
        }

        var builder = new PrettyCode.StringBuilder();
        var resultTemporary = context.NextTemporary();
        builder.AppendLine($"{strategy.TargetType.ToDisplayString()} {resultTemporary};");
        builder.AppendLine($"using ({GetReferenceManagerExpression(context)}.IncreaseDepth())");
        using (builder.CurlyBracesBlock())
        {
            if (!string.IsNullOrWhiteSpace(innerCode))
            {
                builder.AppendLine(innerCode);
            }

            builder.AppendLine($"{resultTemporary} = {innerVariableName};");
        }

        return (resultTemporary, builder.ToString());
    }

    private static bool ShouldIncreaseDepth(MapStrategy strategy, MappaBuilderContext context)
    {
        if (!context.IsMaxRuntimeDepthActive)
        {
            return false;
        }

        // Container / wrapper strategies wrap their elements/cases themselves.
        if (strategy is CollectionToCollectionMapStrategy
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
            or QueryableProjectionMapStrategy)
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
}