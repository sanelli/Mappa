// <copyright file="InvokeConstructorMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeConstructorMapStrategy"/>.
/// </summary>
internal sealed class InvokeConstructorMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeConstructorMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeConstructorMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeConstructorMapStrategyBuilder(InvokeConstructorMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        string resultTemporary;

        using (context.PushCurrentCompositeTypeSourceName(source))
        {
            var parametersVariableNames = BuildParameterMappings(source, context, mappaGlobalOptions, builder, this.strategy.ParametersMapStrategies);
            var (objectInitializerStrategies, postConstructorStrategies) = this.PartitionConstructorInitializers(context, source);
            var propertyInitializersMappings = BuildObjectInitializerMappings(
                source,
                context,
                mappaGlobalOptions,
                builder,
                objectInitializerStrategies);

            var hasPropertyInitializers = propertyInitializersMappings.Count > 0;
            var constructionExpression = InaccessibleMemberAccessHelper.BuildConstructorInvocationExpression(
                this.strategy.Constructor,
                parametersVariableNames,
                this.strategy.RequiresUnsafeAccessorOnConstructor,
                context);

            resultTemporary = context.NextTemporary();
            this.AppendConstructionWithOptionalObjectInitializer(
                builder,
                resultTemporary,
                constructionExpression,
                propertyInitializersMappings,
                hasPropertyInitializers);

            AppendEarlyReferencePair(context, builder, resultTemporary, source, this.strategy.TargetType, this.strategy.SourceType);
            AppendPostConstructorInitializers(
                source,
                context,
                mappaGlobalOptions,
                builder,
                resultTemporary,
                postConstructorStrategies);
            AppendAssignToContextEntries(builder, resultTemporary, this.strategy.AssignToContextEntries, this.strategy.ContextParameterName);
        }

        return (resultTemporary, builder.ToString());
    }

    private static List<string> BuildParameterMappings(
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        PrettyCode.StringBuilder builder,
        IReadOnlyList<ParameterMapStrategy> parametersMapStrategies)
    {
        var parametersVariableNames = new List<string>();
        foreach (var parameterMapStrategy in parametersMapStrategies)
        {
            var (parameterTargetTemporary, parameterCode) = parameterMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(parameterCode))
            {
                builder.AppendLine(parameterCode);
            }

            parametersVariableNames.Add(parameterTargetTemporary);
        }

        return parametersVariableNames;
    }

    private static List<(IPropertySymbol TargetProperty, string TemporaryName, bool RequiresUnsafeAccessorOnTarget)> BuildObjectInitializerMappings(
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        PrettyCode.StringBuilder builder,
        PropertyMapStrategy[] objectInitializerStrategies)
    {
        var propertyInitializersMappings = new List<(IPropertySymbol TargetProperty, string TemporaryName, bool RequiresUnsafeAccessorOnTarget)>();
        foreach (var propertyMapStrategy in objectInitializerStrategies)
        {
            var (initializerPropertyTargetTemporary, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
            propertyInitializersMappings.Add((
                propertyMapStrategy.TargetProperty,
                initializerPropertyTargetTemporary,
                propertyMapStrategy.RequiresUnsafeAccessorOnTarget));
            if (!string.IsNullOrWhiteSpace(initializerPropertyCode))
            {
                builder.AppendLine(initializerPropertyCode);
            }
        }

        return propertyInitializersMappings;
    }

    private static void AppendConstructionWithOptionalObjectInitializer(
        PrettyCode.StringBuilder builder,
        string resultTemporary,
        string constructionExpression,
        List<(IPropertySymbol TargetProperty, string TemporaryName, bool RequiresUnsafeAccessorOnTarget)> propertyInitializersMappings,
        bool hasPropertyInitializers,
        ITypeSymbol targetType)
    {
        if (hasPropertyInitializers)
        {
            builder.AppendLine($"{targetType.ToDisplayString()} {resultTemporary} = {constructionExpression}");
            using (builder.CurlyBracesBlock(trailingSemicolon: true))
            {
                foreach (var propertyInitializersMapping in propertyInitializersMappings)
                {
                    builder.AppendLine($"{propertyInitializersMapping.TargetProperty.Name} = {propertyInitializersMapping.TemporaryName},");
                }
            }

            return;
        }

        builder.AppendLine($"{targetType.ToDisplayString()} {resultTemporary} = {constructionExpression};");
    }

    private static void AppendEarlyReferencePair(
        MappaBuilderContext context,
        PrettyCode.StringBuilder builder,
        string resultTemporary,
        string source,
        ITypeSymbol targetType,
        ITypeSymbol sourceType)
    {
        var earlyAddReferencePair = ReferenceHandlingCodeGenerator.BuildEarlyAddReferencePairStatement(
            context,
            resultTemporary,
            source,
            targetType,
            sourceType);
        if (earlyAddReferencePair is not null)
        {
            builder.AppendLine(earlyAddReferencePair);
        }
    }

    private static void AppendPostConstructorInitializers(
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions,
        PrettyCode.StringBuilder builder,
        string resultTemporary,
        PropertyMapStrategy[] postConstructorStrategies)
    {
        using (context.PushCurrentCompositeTypeTargetName(resultTemporary))
        {
            foreach (var propertyMapStrategy in postConstructorStrategies)
            {
                var (initializerPropertyTargetTemporary, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(initializerPropertyCode))
                {
                    builder.AppendLine(initializerPropertyCode);
                }

                if (!propertyMapStrategy.PostConstructorInitializer)
                {
                    builder.AppendLine(InaccessibleMemberAccessHelper.BuildPropertyAssignmentStatement(
                        resultTemporary,
                        propertyMapStrategy.TargetProperty,
                        initializerPropertyTargetTemporary,
                        propertyMapStrategy.RequiresUnsafeAccessorOnTarget,
                        context));
                }
            }
        }
    }

    private static void AppendAssignToContextEntries(
        PrettyCode.StringBuilder builder,
        string resultTemporary,
        MappaAssignToContextEntry[] assignToContextEntries,
        string? contextParameterName)
    {
        if (assignToContextEntries.Length == 0 || contextParameterName is null)
        {
            return;
        }

        foreach (var assignToContextEntry in assignToContextEntries)
        {
            builder.AppendLine($"{contextParameterName}[{CSharpLiteralHelper.ToStringLiteral(assignToContextEntry.ContextKey)}] = {PropertyPathExpressionBuilder.BuildTargetMemberAccessExpression(resultTemporary, assignToContextEntry.MemberName)};");
        }
    }

    private (PropertyMapStrategy[] ObjectInitializers, PropertyMapStrategy[] PostConstructor) PartitionConstructorInitializers(
        MappaBuilderContext context,
        string source)
    {
        var deferInitializersForReferenceReusing = ReferenceHandlingCodeGenerator.ShouldRegisterReferencePairEarly(
            context,
            source,
            this.strategy.TargetType,
            this.strategy.SourceType);
        if (this.strategy.RequiresUnsafeAccessorOnConstructor || deferInitializersForReferenceReusing)
        {
            return ([], [.. this.strategy.InitializerStrategies]);
        }

        var objectInitializerStrategies = this.strategy.InitializerStrategies
            .Where(propertyMapStrategy => !propertyMapStrategy.PostConstructorInitializer
                                          && !propertyMapStrategy.RequiresUnsafeAccessorOnTarget)
            .ToArray();
        var postConstructorStrategies = this.strategy.InitializerStrategies
            .Where(propertyMapStrategy => propertyMapStrategy.PostConstructorInitializer
                                          || propertyMapStrategy.RequiresUnsafeAccessorOnTarget)
            .ToArray();
        return (objectInitializerStrategies, postConstructorStrategies);
    }

    private void AppendConstructionWithOptionalObjectInitializer(
        PrettyCode.StringBuilder builder,
        string resultTemporary,
        string constructionExpression,
        List<(IPropertySymbol TargetProperty, string TemporaryName, bool RequiresUnsafeAccessorOnTarget)> propertyInitializersMappings,
        bool hasPropertyInitializers)
        => AppendConstructionWithOptionalObjectInitializer(
            builder,
            resultTemporary,
            constructionExpression,
            propertyInitializersMappings,
            hasPropertyInitializers,
            this.strategy.TargetType);
}