// <copyright file="InvokeConstructorMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

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

        // Handle arguments mappings
        var parametersVariableNames = new List<string>();
        foreach (var parameterMapStrategy in this.strategy.ParametersMapStrategies)
        {
            var parameterTemporary = context.NextTemporary();
            builder.AppendLine($"{parameterMapStrategy.SourceType.ToDisplayString()} {parameterTemporary} = {source}.{parameterMapStrategy.SourceProperty.Name};");
            var (parameterTargetTemporary, parameterCode) = parameterMapStrategy.GetBuilder().BuildSource(parameterTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(parameterCode))
            {
                builder.AppendLine(parameterCode);
                builder.AppendEmptyLine();
            }

            parametersVariableNames.Add(parameterTargetTemporary);
        }

        // Handle initializer properties
        var propertyInitializersMappings = new List<(string TargetPropertyName, string TemporaryName)>();
        foreach (var propertyMapStrategy in this.strategy.InitializerStrategies)
        {
            var initializerPropertyTemporary = context.NextTemporary();
            builder.AppendLine($"{propertyMapStrategy.SourceType.ToDisplayString()} {initializerPropertyTemporary} = {source}.{propertyMapStrategy.SourceProperty.Name};");
            var (initializerPropertyTargetTemporary, initializerPropertyCode) = propertyMapStrategy.GetBuilder().BuildSource(initializerPropertyTemporary, context, mappaGlobalOptions);
            propertyInitializersMappings.Add((propertyMapStrategy.TargetProperty.Name, initializerPropertyTargetTemporary));
            if (!string.IsNullOrWhiteSpace(initializerPropertyCode))
            {
                builder.AppendLine(initializerPropertyCode);
                builder.AppendEmptyLine();
            }
        }

        var hasPropertyInitializers = propertyInitializersMappings.Count > 0;

        var resultTemporary = context.NextTemporary();
        var initializerCodeLine = $"{this.strategy.TargetType.ToDisplayString()} {resultTemporary} = new {this.strategy.TargetType.ToDisplayNameWithoutNullableAnnotation()}({string.Join(", ", parametersVariableNames)}){(hasPropertyInitializers ? string.Empty : ";")}";
        builder.AppendLine(initializerCodeLine);
        if (hasPropertyInitializers)
        {
            using (builder.CurlyBracesBlock(trailingSemicolon: true, indent: false))
            {
                foreach (var propertyInitializersMapping in propertyInitializersMappings)
                {
                    builder.AppendLine($"{propertyInitializersMapping.TargetPropertyName} = {propertyInitializersMapping.TemporaryName},");
                }
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (source-type is \"{this.strategy.SourceType.ToDisplayString()}\", target-type is \"{this.strategy.TargetType.ToDisplayString()}\") */ "
            : string.Empty;

        return ($"{ruleComment}{resultTemporary}", builder.ToString());
    }
}