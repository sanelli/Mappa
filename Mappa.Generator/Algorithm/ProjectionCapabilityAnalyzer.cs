// <copyright file="ProjectionCapabilityAnalyzer.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Determines whether a mapping strategy can be expressed as a queryable projection expression.
/// </summary>
internal static class ProjectionCapabilityAnalyzer
{
    private enum AnalysisFailureKind
    {
        UnsupportedConstruct,
        InvokeMethodNotInlinable,
        NestedQueryable,
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="strategy"/> can be translated into a projection expression.
    /// </summary>
    /// <param name="strategy">The strategy to analyze.</param>
    /// <returns><c>true</c> when the strategy is supported.</returns>
    internal static bool IsSupported(MapStrategy strategy)
        => TryAnalyzeCore(strategy, analysisContext: null, out _, out _, out _);

    /// <summary>
    /// Analyzes <paramref name="strategy"/> for queryable projection support, reporting diagnostics when requested.
    /// </summary>
    /// <param name="strategy">The strategy to analyze.</param>
    /// <param name="analysisContext">The analysis context.</param>
    /// <param name="normalizedStrategy">The strategy with inlined map methods when analysis succeeds.</param>
    /// <returns><c>true</c> when the strategy is supported.</returns>
    internal static bool TryAnalyze(
        MapStrategy strategy,
        ProjectionCapabilityAnalysisContext analysisContext,
        out MapStrategy normalizedStrategy)
    {
        if (TryAnalyzeCore(strategy, analysisContext, out normalizedStrategy, out var failureKind, out var failureMember))
        {
            return true;
        }

        if (failureKind is null)
        {
            return false;
        }

        ReportFailure(analysisContext, failureKind.Value, failureMember);
        return false;
    }

    private static bool TryAnalyzeCore(
        MapStrategy strategy,
        ProjectionCapabilityAnalysisContext? analysisContext,
        out MapStrategy normalizedStrategy,
        out AnalysisFailureKind? failureKind,
        out string? failureMember)
    {
        failureKind = null;
        failureMember = null;
        normalizedStrategy = strategy;

        switch (strategy)
        {
            case IdentityMapStrategy identityMapStrategy:
                return IsIdentitySupported(identityMapStrategy);

            case NullableStrategy nullableStrategy:
                if (!TryAnalyzeCore(
                        nullableStrategy.ElementStrategy,
                        analysisContext,
                        out var normalizedElementStrategy,
                        out failureKind,
                        out failureMember))
                {
                    return false;
                }

                normalizedStrategy = new NullableStrategy(
                    nullableStrategy.TargetType,
                    nullableStrategy.SourceType,
                    normalizedElementStrategy);
                return true;

            case InvokeConstructorMapStrategy invokeConstructorMapStrategy:
                return TryAnalyzeConstructor(invokeConstructorMapStrategy, analysisContext, out normalizedStrategy, out failureKind, out failureMember);

            case InvokeObjectFactoryMapStrategy invokeObjectFactoryMapStrategy:
                failureKind = AnalysisFailureKind.InvokeMethodNotInlinable;
                failureMember = invokeObjectFactoryMapStrategy.ObjectFactory.Method.Name;
                return false;

            case ParameterMapStrategy parameterMapStrategy:
                if (!TryAnalyzeCore(
                        parameterMapStrategy.ParameterStrategy,
                        analysisContext,
                        out var normalizedParameterStrategy,
                        out failureKind,
                        out failureMember))
                {
                    return false;
                }

                normalizedStrategy = new ParameterMapStrategy(
                    parameterMapStrategy.TargetParameter,
                    parameterMapStrategy.SourceProperty,
                    normalizedParameterStrategy,
                    parameterMapStrategy.RequiresUnsafeAccessorOnSource);
                return true;

            case PropertyMapStrategy propertyMapStrategy:
                return TryAnalyzeProperty(propertyMapStrategy, analysisContext, out normalizedStrategy, out failureKind, out failureMember);

            case MethodMapStrategy methodMapStrategy:
                return TryAnalyzeMethodMap(methodMapStrategy, analysisContext, out normalizedStrategy, out failureKind, out failureMember);

            case MappaInvokeMethodAttributeStrategy mappaInvokeMethodAttributeStrategy:
                return TryAnalyzeInvokeMethodAttribute(mappaInvokeMethodAttributeStrategy, analysisContext, out normalizedStrategy, out failureKind, out failureMember);

            case EnumToEnumMapStrategy enumToEnumMapStrategy:
                TryReportEnumWarning(enumToEnumMapStrategy.EnumToEnumMapSetting, enumToEnumMapStrategy.CaseInsensitiveEnumMap, analysisContext);
                return true;

            case EnumToStringMapStrategy enumToStringMapStrategy:
                TryReportEnumStringWarning(
                    enumToStringMapStrategy.EnumStringMapSetting,
                    analysisContext?.AlgorithmContext.MappaUserSettings.CaseInsensitiveEnumMap ?? BooleanSetting.Undefined,
                    analysisContext);
                return true;

            case StringToEnumMapStrategy stringToEnumMapStrategy:
                TryReportEnumStringWarning(stringToEnumMapStrategy.EnumStringMapSetting, stringToEnumMapStrategy.CaseInsensitiveEnumMap, analysisContext);
                return true;

            default:
                return IsBuiltInTranslatableStrategy(strategy)
                    || TryAnalyzeUnsupported(strategy, out failureKind, out failureMember);
        }
    }

    private static bool IsBuiltInTranslatableStrategy(MapStrategy strategy)
        => strategy is EnumToIntegralMapStrategy
            or IntegralToEnumMapStrategy
            or InvokeParseMethodMapStrategy
            or InvokeToStringMapStrategy
            or InvokeParseStringWithFormatMapStrategy
            or InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy
            or StringToNumberMapStrategy
            or StringToUriMapStrategy
            or DateOnlyToDateTimeMapStrategy
            or DateOnlyToLongMapStrategy
            or DateTimeOffsetToDateOnlyMapStrategy
            or DateTimeOffsetToDateTimeMapStrategy
            or DateTimeOffsetToLongMapStrategy
            or DateTimeOffsetToTimeOnlyMapStrategy
            or DateTimeToDateOnlyMapStrategy
            or DateTimeToTimeOnlyMapStrategy
            or DateTimeToLongMapStrategy
            or DoubleToTimeSpanMapStrategy
            or LongToDateTimeMapStrategy
            or LongToDateTimeOffsetMapStrategy
            or TimeSpanToDoubleMapStrategy;

    private static bool TryAnalyzeUnsupported(
        MapStrategy strategy,
        out AnalysisFailureKind? failureKind,
        out string? failureMember)
    {
        failureKind = AnalysisFailureKind.UnsupportedConstruct;
        failureMember = strategy.GetType().Name;
        return false;
    }

    private static bool TryAnalyzeConstructor(
        InvokeConstructorMapStrategy invokeConstructorMapStrategy,
        ProjectionCapabilityAnalysisContext? analysisContext,
        out MapStrategy normalizedStrategy,
        out AnalysisFailureKind? failureKind,
        out string? failureMember)
    {
        normalizedStrategy = invokeConstructorMapStrategy;
        failureKind = null;
        failureMember = null;

        if (invokeConstructorMapStrategy.AssignToContextEntries.Length > 0
            || invokeConstructorMapStrategy.ContextParameterName is not null)
        {
            failureKind = AnalysisFailureKind.UnsupportedConstruct;
            failureMember = "MappaAssignToContext";
            return false;
        }

        if (invokeConstructorMapStrategy.InitializerStrategies.Any(propertyMapStrategy => propertyMapStrategy.PostConstructorInitializer))
        {
            failureKind = AnalysisFailureKind.UnsupportedConstruct;
            failureMember = "post-constructor property assignment";
            return false;
        }

        ParameterMapStrategy[] normalizedParameters = new ParameterMapStrategy[invokeConstructorMapStrategy.ParametersMapStrategies.Length];
        for (var index = 0; index < invokeConstructorMapStrategy.ParametersMapStrategies.Length; index++)
        {
            var parameterMapStrategy = invokeConstructorMapStrategy.ParametersMapStrategies[index];
            if (!TryAnalyzeCore(
                    parameterMapStrategy.ParameterStrategy,
                    analysisContext,
                    out var normalizedParameterStrategy,
                    out failureKind,
                    out failureMember))
            {
                return false;
            }

            normalizedParameters[index] = new ParameterMapStrategy(
                parameterMapStrategy.TargetParameter,
                parameterMapStrategy.SourceProperty,
                normalizedParameterStrategy,
                parameterMapStrategy.RequiresUnsafeAccessorOnSource);
        }

        PropertyMapStrategy[] normalizedInitializers = new PropertyMapStrategy[invokeConstructorMapStrategy.InitializerStrategies.Length];
        for (var index = 0; index < invokeConstructorMapStrategy.InitializerStrategies.Length; index++)
        {
            if (!TryAnalyzeProperty(
                    invokeConstructorMapStrategy.InitializerStrategies[index],
                    analysisContext,
                    out var normalizedPropertyStrategy,
                    out failureKind,
                    out failureMember))
            {
                return false;
            }

            normalizedInitializers[index] = (PropertyMapStrategy)normalizedPropertyStrategy;
        }

        normalizedStrategy = new InvokeConstructorMapStrategy(
            invokeConstructorMapStrategy.TargetType,
            invokeConstructorMapStrategy.SourceType,
            invokeConstructorMapStrategy.Constructor,
            normalizedParameters,
            normalizedInitializers,
            invokeConstructorMapStrategy.AssignToContextEntries,
            invokeConstructorMapStrategy.ContextParameterName,
            invokeConstructorMapStrategy.RequiresUnsafeAccessorOnConstructor);
        return true;
    }

    private static bool TryAnalyzeProperty(
        PropertyMapStrategy propertyMapStrategy,
        ProjectionCapabilityAnalysisContext? analysisContext,
        out MapStrategy normalizedStrategy,
        out AnalysisFailureKind? failureKind,
        out string? failureMember)
    {
        normalizedStrategy = propertyMapStrategy;
        failureKind = null;
        failureMember = null;

        if (propertyMapStrategy.PostConstructorInitializer)
        {
            failureKind = AnalysisFailureKind.UnsupportedConstruct;
            failureMember = propertyMapStrategy.TargetProperty.Name;
            return false;
        }

        if (analysisContext is not null
            && (IsNestedQueryableType(propertyMapStrategy.TargetProperty.Type, analysisContext.Compilation)
                || (propertyMapStrategy.SourceProperty is not null
                    && IsNestedQueryableType(propertyMapStrategy.SourceProperty.Type, analysisContext.Compilation))))
        {
            failureKind = AnalysisFailureKind.NestedQueryable;
            failureMember = propertyMapStrategy.TargetProperty.Name;
            return false;
        }

        if (!TryAnalyzeCore(
                propertyMapStrategy.PropertyStrategy,
                analysisContext,
                out var normalizedPropertyStrategy,
                out failureKind,
                out failureMember))
        {
            return false;
        }

        normalizedStrategy = new PropertyMapStrategy(
            propertyMapStrategy.TargetProperty,
            propertyMapStrategy.SourceProperty,
            normalizedPropertyStrategy,
            propertyMapStrategy.PostConstructorInitializer,
            propertyMapStrategy.ChainedSourcePropertyPath,
            propertyMapStrategy.RequiresUnsafeAccessorOnSource,
            propertyMapStrategy.RequiresUnsafeAccessorOnTarget);
        return true;
    }

    private static bool TryAnalyzeMethodMap(
        MethodMapStrategy methodMapStrategy,
        ProjectionCapabilityAnalysisContext? analysisContext,
        out MapStrategy normalizedStrategy,
        out AnalysisFailureKind? failureKind,
        out string? failureMember)
    {
        normalizedStrategy = methodMapStrategy;
        failureKind = null;
        failureMember = null;

        if (methodMapStrategy.MapMethod.RequireMappaContextWhenInvoked()
            || methodMapStrategy.ContextParameterName is not null)
        {
            failureKind = AnalysisFailureKind.InvokeMethodNotInlinable;
            failureMember = methodMapStrategy.MapMethod.MethodName;
            return false;
        }

        if (!TryResolveInlinableStrategy(
                methodMapStrategy.TargetType,
                methodMapStrategy.SourceType,
                analysisContext,
                out var inlinableStrategy))
        {
            failureKind = AnalysisFailureKind.InvokeMethodNotInlinable;
            failureMember = methodMapStrategy.MapMethod.MethodName;
            return false;
        }

        return TryAnalyzeCore(inlinableStrategy, analysisContext, out normalizedStrategy, out failureKind, out failureMember);
    }

    private static bool TryAnalyzeInvokeMethodAttribute(
        MappaInvokeMethodAttributeStrategy mappaInvokeMethodAttributeStrategy,
        ProjectionCapabilityAnalysisContext? analysisContext,
        out MapStrategy normalizedStrategy,
        out AnalysisFailureKind? failureKind,
        out string? failureMember)
    {
        normalizedStrategy = mappaInvokeMethodAttributeStrategy;
        failureKind = null;
        failureMember = null;

        if (mappaInvokeMethodAttributeStrategy.ContextParameterName is not null)
        {
            failureKind = AnalysisFailureKind.InvokeMethodNotInlinable;
            failureMember = mappaInvokeMethodAttributeStrategy.Method.Name;
            return false;
        }

        if (analysisContext is null
            || !analysisContext.AlgorithmContext.TryGetMethod(
                mappaInvokeMethodAttributeStrategy.TargetType,
                mappaInvokeMethodAttributeStrategy.SourceType,
                out var mapMethod)
            || mapMethod.RequireMappaContextWhenInvoked())
        {
            failureKind = AnalysisFailureKind.InvokeMethodNotInlinable;
            failureMember = mappaInvokeMethodAttributeStrategy.Method.Name;
            return false;
        }

        return TryAnalyzeMethodMap(
            new MethodMapStrategy(mapMethod, null),
            analysisContext,
            out normalizedStrategy,
            out failureKind,
            out failureMember);
    }

    private static bool TryResolveInlinableStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        ProjectionCapabilityAnalysisContext? analysisContext,
        out MapStrategy strategy)
    {
        strategy = new NoMapStrategy(targetType, sourceType);
        if (analysisContext is null)
        {
            return false;
        }

        var derivedContext = new DerivedMappaMapAlgorithmContext(
            analysisContext.AlgorithmContext,
            targetType,
            sourceType);
        strategy = new TypeMapIdentifierAlgorithm(
            derivedContext,
            analysisContext.Compilation,
            analysisContext.CancellationToken).GetStrategy();

        return strategy is not NoMapStrategy
               and not MethodMapStrategy
               and not PolymorphicMethodMapStrategy
               and not MappaInvokeMethodAttributeStrategy;
    }

    private static bool IsIdentitySupported(IdentityMapStrategy identityMapStrategy)
        => identityMapStrategy.IdentityMapDeepCopySetting is IdentityMapDeepCopySetting.ShallowCopy
           && !identityMapStrategy.RequiresMemberwiseClone
           && identityMapStrategy.NestedFieldStrategies.Count == 0;

    private static bool IsNestedQueryableType(ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsOrImplementIQueryable(compilation);

    private static void TryReportEnumWarning(
        EnumToEnumMapSetting enumToEnumMapSetting,
        BooleanSetting caseInsensitiveEnumMap,
        ProjectionCapabilityAnalysisContext? analysisContext)
    {
        var effectiveSetting = enumToEnumMapSetting is EnumToEnumMapSetting.Undefined
            ? EnumToEnumMapSetting.MemberName
            : enumToEnumMapSetting;
        if (effectiveSetting is not EnumToEnumMapSetting.MemberName
            || caseInsensitiveEnumMap is not BooleanSetting.Enable
            || analysisContext is null)
        {
            return;
        }

        analysisContext.AlgorithmContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionEnumStrategyNotSupported(analysisContext.Location, analysisContext.MethodName));
    }

    private static void TryReportEnumStringWarning(
        EnumStringMapSetting enumStringMapSetting,
        BooleanSetting caseInsensitiveEnumMap,
        ProjectionCapabilityAnalysisContext? analysisContext)
    {
        var effectiveSetting = enumStringMapSetting is EnumStringMapSetting.Undefined
            ? EnumStringMapSetting.MemberName
            : enumStringMapSetting;
        if (effectiveSetting is not EnumStringMapSetting.MemberName
            || caseInsensitiveEnumMap is not BooleanSetting.Enable
            || analysisContext is null)
        {
            return;
        }

        analysisContext.AlgorithmContext.ReportDiagnostic(
            MappaDiagnostics.ProjectionEnumStrategyNotSupported(analysisContext.Location, analysisContext.MethodName));
    }

    private static void ReportFailure(
        ProjectionCapabilityAnalysisContext analysisContext,
        AnalysisFailureKind failureKind,
        string? failureMember)
    {
        var diagnostic = failureKind switch
        {
            AnalysisFailureKind.InvokeMethodNotInlinable => MappaDiagnostics.ProjectionInvokeMethodNotInlinable(
                analysisContext.Location,
                analysisContext.MethodName,
                failureMember ?? string.Empty),
            AnalysisFailureKind.NestedQueryable => MappaDiagnostics.ProjectionNestedQueryableNotSupported(
                analysisContext.Location,
                analysisContext.MethodName,
                failureMember ?? string.Empty),
            _ => MappaDiagnostics.ProjectionMappingNotSupported(
                analysisContext.Location,
                analysisContext.MethodName,
                failureMember ?? string.Empty),
        };

        analysisContext.AlgorithmContext.ReportDiagnostic(diagnostic);
    }
}