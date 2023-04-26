// <copyright file="DictionaryToDictionaryMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DictionaryToDictionaryMapStrategy"/>.
/// </summary>
internal sealed class DictionaryToDictionaryMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly DictionaryToDictionaryMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryToDictionaryMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DictionaryToDictionaryMapStrategyBuilder(DictionaryToDictionaryMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (key strategy: {this.strategy.KeyStrategy.Rule}, value strategy: {this.strategy.ValueStrategy.Rule}) */ "
            : string.Empty;

        var (targetKeyType, targetValueType) = this.strategy.TargetType.GetKeyAndValueTypes();
        var (sourceKeyType, sourceValueType) = this.strategy.SourceType.GetKeyAndValueTypes();

        var dictionaryTemporary = context.NextTemporary();
        var loopTemporary = context.NextTemporary();

        var builder = new IndentStringBuilder();
        builder.AppendLine($"System.Collections.Generic.Dictionary<{targetKeyType.ToDisplayString()}, {targetValueType.ToDisplayString()}> {dictionaryTemporary} = new System.Collections.Generic.Dictionary<{targetKeyType.ToDisplayString()}, {targetValueType.ToDisplayString()}>();");
        builder.AppendLine($"foreach (System.Collections.Generic.KeyValuePair<{sourceKeyType.ToDisplayString()}, {sourceValueType.ToDisplayString()}> {loopTemporary} in {source})");

        using (builder.CodeBlock())
        using (builder.Indent())
        {
            // Process the source
            var sourceKeyTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceKeyType.ToDisplayString()} {sourceKeyTemporary} = {loopTemporary}.Key;");

            var (targetKeyTemporary, targetKeyStrategyCode) = this.strategy.KeyStrategy.GetBuilder().BuildSource(sourceKeyTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(targetKeyStrategyCode))
            {
                builder.AppendLine(targetKeyStrategyCode);
                builder.AppendEmptyLine();
            }

            // Process the target.
            var valueTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceValueType.ToDisplayString()} {valueTemporary} = {loopTemporary}.Value;");
            var (targetValueTemporary, targetValueStrategyCode) = this.strategy.ValueStrategy.GetBuilder().BuildSource(valueTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(targetValueStrategyCode))
            {
                builder.AppendLine(targetValueStrategyCode);
                builder.AppendEmptyLine();
            }

            // Assign using the indexer.
            builder.AppendEmptyLine();
            builder.AppendLine($"{dictionaryTemporary}[{targetKeyTemporary}] = {targetValueTemporary};");
        }

        return ($"{ruleComment}{dictionaryTemporary}", builder.ToString());
    }
}