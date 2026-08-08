// <copyright file="ProjectionExpressionBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Algorithm;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Expressions;

/// <summary>
/// Builds expression-tree-compatible mapping expressions for queryable projections.
/// </summary>
internal static class ProjectionExpressionBuilder
{
    private static readonly Dictionary<Type, Func<MapStrategy, string, ExpressionBuildContext, string>> ExpressionBuilders =
        new()
        {
            [typeof(IdentityMapStrategy)] = static (_, source, _) => BuildIdentityExpression(source),
            [typeof(NullableStrategy)] = static (strategy, source, context) =>
                BuildNullableExpression((NullableStrategy)strategy, source, context),
            [typeof(InvokeConstructorMapStrategy)] = static (strategy, source, context) =>
                BuildConstructorExpression((InvokeConstructorMapStrategy)strategy, source, context),
            [typeof(ParameterMapStrategy)] = static (strategy, source, context) =>
                BuildParameterExpression((ParameterMapStrategy)strategy, source, context),
            [typeof(PropertyMapStrategy)] = static (strategy, source, context) =>
                BuildPropertyExpression((PropertyMapStrategy)strategy, source, context),
            [typeof(EnumToEnumMapStrategy)] = static (strategy, source, _) =>
                EnumMapSwitchExpressionHelper.BuildSwitchExpression(((EnumToEnumMapStrategy)strategy).EnumMapConfiguration, source),
            [typeof(EnumToIntegralMapStrategy)] = static (strategy, source, _) =>
                EnumMapSwitchExpressionHelper.BuildSwitchExpression(((EnumToIntegralMapStrategy)strategy).EnumMapConfiguration, source),
            [typeof(EnumToStringMapStrategy)] = static (strategy, source, _) =>
                EnumMapSwitchExpressionHelper.BuildSwitchExpression(((EnumToStringMapStrategy)strategy).EnumMapConfiguration, source),
            [typeof(IntegralToEnumMapStrategy)] = static (strategy, source, _) =>
                BuildIntegralToEnumExpression((IntegralToEnumMapStrategy)strategy, source),
            [typeof(StringToEnumMapStrategy)] = static (strategy, source, _) =>
                BuildStringToEnumExpression((StringToEnumMapStrategy)strategy, source),
            [typeof(InvokeParseMethodMapStrategy)] = static (strategy, source, _) =>
                $"{((InvokeParseMethodMapStrategy)strategy).TargetType.ToDisplayString()}.Parse({source})",
            [typeof(InvokeToStringMapStrategy)] = static (strategy, source, _) =>
                BuildToStringExpression((InvokeToStringMapStrategy)strategy, source),
            [typeof(InvokeParseStringWithFormatMapStrategy)] = static (strategy, source, _) =>
                BuildParseWithFormatExpression((InvokeParseStringWithFormatMapStrategy)strategy, source),
            [typeof(InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy)] = static (strategy, source, _) =>
                BuildParseDateOnlyOrTimeOnlyExpression((InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy)strategy, source),
            [typeof(StringToNumberMapStrategy)] = static (strategy, source, _) =>
                BuildStringToNumberExpression((StringToNumberMapStrategy)strategy, source),
            [typeof(StringToUriMapStrategy)] = static (_, source, _) => $"new System.Uri({source})",
            [typeof(DateOnlyToDateTimeMapStrategy)] = static (_, source, _) =>
                $"new System.DateTime({source}, System.TimeOnly.MinValue, System.DateTimeKind.Utc)",
            [typeof(DateOnlyToLongMapStrategy)] = static (_, source, _) =>
                $"(long)new System.DateTime({source}, System.TimeOnly.MinValue, System.DateTimeKind.Utc).ToUniversalTime().Subtract(System.DateTime.UnixEpoch).TotalSeconds",
            [typeof(DateTimeOffsetToDateOnlyMapStrategy)] = static (_, source, _) =>
                $"System.DateOnly.FromDateTime({source}.DateTime)",
            [typeof(DateTimeOffsetToDateTimeMapStrategy)] = static (_, source, _) => $"{source}.DateTime",
            [typeof(DateTimeOffsetToLongMapStrategy)] = static (_, source, _) => $"{source}.ToUnixTimeSeconds()",
            [typeof(DateTimeOffsetToTimeOnlyMapStrategy)] = static (_, source, _) =>
                $"System.TimeOnly.FromDateTime({source}.DateTime)",
            [typeof(DateTimeToDateOnlyMapStrategy)] = static (_, source, _) => $"System.DateOnly.FromDateTime({source})",
            [typeof(DateTimeToLongMapStrategy)] = static (_, source, _) =>
                $"(long){source}.ToUniversalTime().Subtract(System.DateTime.UnixEpoch).TotalSeconds",
            [typeof(DateTimeToTimeOnlyMapStrategy)] = static (_, source, _) => $"System.TimeOnly.FromDateTime({source})",
            [typeof(DoubleToTimeSpanMapStrategy)] = static (_, source, _) => $"System.TimeSpan.FromDays({source})",
            [typeof(LongToDateTimeMapStrategy)] = static (_, source, _) => $"System.DateTime.UnixEpoch.AddSeconds({source})",
            [typeof(LongToDateTimeOffsetMapStrategy)] = static (_, source, _) =>
                $"System.DateTimeOffset.FromUnixTimeSeconds({source})",
            [typeof(TimeSpanToDoubleMapStrategy)] = static (_, source, _) => $"{source}.TotalDays",
        };

    /// <summary>
    /// Attempts to build a projection expression for the specified strategy.
    /// </summary>
    /// <param name="strategy">The mapping strategy.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="context">The expression build context.</param>
    /// <param name="expression">The built expression when the operation succeeds.</param>
    /// <returns><c>true</c> when the expression has been built.</returns>
    internal static bool TryBuildExpression(
        MapStrategy strategy,
        string source,
        ExpressionBuildContext context,
        out string expression)
    {
        if (!ProjectionCapabilityAnalyzer.IsSupported(strategy))
        {
            expression = string.Empty;
            return false;
        }

        if (!ExpressionBuilders.TryGetValue(strategy.GetType(), out var builder))
        {
            throw new MappaGeneratorException($"Unsupported projection strategy '{strategy.GetType().Name}'.");
        }

        expression = builder(strategy, source, context);
        return true;
    }

    /// <summary>
    /// Builds a nullable projection expression.
    /// </summary>
    /// <param name="strategy">The nullable strategy.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="context">The expression build context.</param>
    /// <returns>The projection expression.</returns>
    internal static string BuildNullableExpression(NullableStrategy strategy, string source, ExpressionBuildContext context)
    {
        if (!TryBuildExpression(strategy.ElementStrategy, GetNullableInnerSource(strategy, source), context, out var innerExpression))
        {
            throw new MappaGeneratorException("Nullable projection element strategy is not supported.");
        }

        if (strategy.SourceType.IsReferenceType)
        {
            return strategy.TargetType.IsNullable()
                ? $"{source} == null ? ({strategy.TargetType.ToDisplayString()})null : {innerExpression}"
                : $"{source} == null ? throw new System.NullReferenceException({CSharpLiteralHelper.ToStringLiteral($"\"{source}\" is null.")}) : {innerExpression}";
        }

        return strategy.TargetType.IsNullable()
            ? $"{source}.HasValue ? ({strategy.TargetType.ToDisplayString()}){innerExpression} : ({strategy.TargetType.ToDisplayString()})null"
            : $"{source}.HasValue ? {innerExpression} : throw new System.NullReferenceException({CSharpLiteralHelper.ToStringLiteral($"\"{source}\" is null.")})";
    }

    /// <summary>
    /// Builds a constructor-parameter projection expression.
    /// </summary>
    /// <param name="parameterMapStrategy">The parameter mapping strategy.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="context">The expression build context.</param>
    /// <returns>The projection expression.</returns>
    internal static string BuildParameterExpression(
        ParameterMapStrategy parameterMapStrategy,
        string source,
        ExpressionBuildContext context)
    {
        var sourceExpression = BuildSourceMemberExpression(
            parameterMapStrategy.SourceProperty,
            null,
            source,
            context);

        if (!TryBuildExpression(parameterMapStrategy.ParameterStrategy, sourceExpression, context, out var expression))
        {
            throw new MappaGeneratorException("Parameter projection strategy is not supported.");
        }

        return expression;
    }

    /// <summary>
    /// Builds a property projection expression.
    /// </summary>
    /// <param name="propertyMapStrategy">The property mapping strategy.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="context">The expression build context.</param>
    /// <returns>The projection expression.</returns>
    internal static string BuildPropertyExpression(
        PropertyMapStrategy propertyMapStrategy,
        string source,
        ExpressionBuildContext context)
    {
        var sourceExpression = BuildSourceMemberExpression(
            propertyMapStrategy.SourceProperty,
            propertyMapStrategy.ChainedSourcePropertyPath,
            source,
            context);

        if (!TryBuildExpression(propertyMapStrategy.PropertyStrategy, sourceExpression, context, out var expression))
        {
            throw new MappaGeneratorException("Property projection strategy is not supported.");
        }

        return expression;
    }

    private static string BuildIdentityExpression(string source)
        => source;

    private static string GetNullableInnerSource(NullableStrategy strategy, string source)
    {
        if (strategy.SourceType.IsValueType)
        {
            return $"{source}.Value";
        }

        return source;
    }

    private static string BuildConstructorExpression(
        InvokeConstructorMapStrategy strategy,
        string source,
        ExpressionBuildContext context)
    {
        var parameterExpressions = strategy.ParametersMapStrategies
            .Select(parameterMapStrategy => BuildParameterExpression(parameterMapStrategy, source, context))
            .ToArray();

        var initializerExpressions = strategy.InitializerStrategies
            .Where(propertyMapStrategy => !propertyMapStrategy.PostConstructorInitializer)
            .Select(propertyMapStrategy =>
            {
                var propertyExpression = BuildPropertyExpression(propertyMapStrategy, source, context);
                return $"{propertyMapStrategy.TargetProperty.Name} = {propertyExpression}";
            })
            .ToArray();

        var targetTypeName = strategy.TargetType.ToDisplayNameWithoutNullableAnnotation();
        if (initializerExpressions.Length == 0)
        {
            return $"new {targetTypeName}({string.Join(", ", parameterExpressions)})";
        }

        return $"new {targetTypeName}({string.Join(", ", parameterExpressions)}) {{ {string.Join(", ", initializerExpressions)} }}";
    }

    private static string BuildSourceMemberExpression(
        IPropertySymbol? sourceProperty,
        ChainedSourcePropertyPathInfo? chainedSourcePropertyPath,
        string source,
        ExpressionBuildContext context)
    {
        if (chainedSourcePropertyPath is not null)
        {
            var mapMethod = context.BuilderContext.GetMapMethod();
            var chainSource = source;
            var rootParameterName = mapMethod.SourceParameterName;
            var receiverPathPrefix = chainedSourcePropertyPath.ReceiverPathPrefix;

            if (!string.IsNullOrWhiteSpace(receiverPathPrefix)
                && (receiverPathPrefix.Equals(rootParameterName, StringComparison.Ordinal)
                    || receiverPathPrefix.StartsWith($"{rootParameterName}.", StringComparison.Ordinal)))
            {
                chainSource = rootParameterName;
            }

            return PropertyPathExpressionBuilder.BuildChainedAccessExpression(
                chainSource,
                receiverPathPrefix,
                chainedSourcePropertyPath.RemainingSourceSegments,
                chainedSourcePropertyPath.StartingSourceType,
                mapMethod.NullableEnabled,
                sourceProperty?.Type ?? chainedSourcePropertyPath.StartingSourceType,
                out _,
                chainedSourcePropertyPath.OriginalSourcePath);
        }

        if (sourceProperty is not null)
        {
            return $"{source}.{sourceProperty.Name}";
        }

        return source;
    }

    private static string BuildIntegralToEnumExpression(IntegralToEnumMapStrategy strategy, string source)
    {
        var enumUnderlyingType = ((INamedTypeSymbol)strategy.TargetType).EnumUnderlyingType
                                 ?? throw new MappaGeneratorException($"The enum \"{strategy.TargetType.ToDisplayString()}\" does not have an underlying type");

        return EnumMapSwitchExpressionHelper.BuildSwitchExpression(
            strategy.EnumMapConfiguration,
            source,
            enumUnderlyingType.ToDisplayString());
    }

    private static string BuildStringToEnumExpression(StringToEnumMapStrategy strategy, string source)
    {
        var switchSource = strategy.CaseInsensitiveEnumMap is BooleanSetting.Enable
            ? $"{source}.ToUpperInvariant()"
            : source;

        return EnumMapSwitchExpressionHelper.BuildSwitchExpression(
            strategy.EnumMapConfiguration,
            switchSource);
    }

    private static string BuildToStringExpression(InvokeToStringMapStrategy strategy, string source)
    {
        string parameters;
        if (strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
            strategy.CultureInfoSetting is not CultureInfoSetting.None &&
            !string.IsNullOrWhiteSpace(strategy.Format))
        {
            parameters = $"{CSharpLiteralHelper.ToRequiredStringLiteral(strategy.Format)}, {GetCulture(strategy.CultureInfoSetting, strategy.CultureName)}";
        }
        else if (strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
                 strategy.CultureInfoSetting is not CultureInfoSetting.None)
        {
            parameters = GetCulture(strategy.CultureInfoSetting, strategy.CultureName);
        }
        else if (!string.IsNullOrWhiteSpace(strategy.Format))
        {
            parameters = CSharpLiteralHelper.ToRequiredStringLiteral(strategy.Format);
        }
        else
        {
            parameters = string.Empty;
        }

        return string.IsNullOrWhiteSpace(parameters)
            ? $"{source}.ToString()"
            : $"{source}.ToString({parameters})";
    }

    private static string BuildParseWithFormatExpression(InvokeParseStringWithFormatMapStrategy strategy, string source)
    {
        var (parseMethod, parameters) = ParseDateTimeStylesCodeHelper.BuildDateTimeOrDateTimeOffsetParseInvocation(
            source,
            strategy.Format,
            strategy.CultureInfoSetting,
            strategy.CultureName,
            strategy.DateTimeStyle);

        return $"{strategy.TargetType.ToDisplayString()}.{parseMethod}({parameters})";
    }

    private static string BuildParseDateOnlyOrTimeOnlyExpression(
        InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy strategy,
        string source)
    {
        var (parseMethod, parameters) = ParseDateTimeStylesCodeHelper.BuildParseInvocation(
            source,
            strategy.Format,
            strategy.CultureInfoSetting,
            strategy.CultureName,
            strategy.DateTimeStyle);

        return $"{strategy.TargetType.ToDisplayString()}.{parseMethod}({parameters})";
    }

    private static string BuildStringToNumberExpression(StringToNumberMapStrategy strategy, string source)
    {
        var parameters = ParseNumberStylesCodeHelper.BuildParseInvocation(
            source,
            strategy.CultureInfoSetting,
            strategy.CultureName,
            strategy.NumberStyle);

        return $"{strategy.TargetType.ToDisplayString()}.Parse({parameters})";
    }

    private static string GetCulture(CultureInfoSetting cultureInfoSettings, string? cultureName)
        => cultureInfoSettings switch
        {
            CultureInfoSetting.CurrentCulture => "System.Globalization.CultureInfo.CurrentCulture",
            CultureInfoSetting.InvariantCulture => "System.Globalization.CultureInfo.InvariantCulture",
            CultureInfoSetting.UserDefined => $"System.Globalization.CultureInfo.GetCultureInfo({CSharpLiteralHelper.ToStringLiteral(cultureName ?? string.Empty)})",
            _ => throw new ArgumentOutOfRangeException(nameof(cultureInfoSettings), cultureInfoSettings, null),
        };
}