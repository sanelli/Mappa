// <copyright file="InvokeConstructorMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
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
        var builder = new IndentStringBuilder();

        // Handle arguments mappings
        // TODO: Implement me
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

        var resultTemporary = context.NextTemporary();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {resultTemporary} = new {this.strategy.TargetType.ToDisplayString()}()");
        using (builder.CodeBlock(addSemicolonAfterClose: true))
        using (builder.Indent())
        {
            foreach (var propertyInitializersMapping in propertyInitializersMappings)
            {
                builder.AppendLine($"{propertyInitializersMapping.TargetPropertyName} = {propertyInitializersMapping.TemporaryName},");
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (source-type is \"{this.strategy.SourceType.ToDisplayString()}\", target-enum is \"{this.strategy.TargetType.ToDisplayString()}\") */ "
            : string.Empty;

        return ($"{ruleComment}{resultTemporary}", builder.ToString());
    }
}