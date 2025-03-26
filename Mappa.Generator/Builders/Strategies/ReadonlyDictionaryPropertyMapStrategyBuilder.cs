// <copyright file="ReadonlyDictionaryPropertyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ReadonlyDictionaryPropertyMapStrategy"/>.
/// </summary>
internal sealed class ReadonlyDictionaryPropertyMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ReadonlyDictionaryPropertyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadonlyDictionaryPropertyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    internal ReadonlyDictionaryPropertyMapStrategyBuilder(ReadonlyDictionaryPropertyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc />
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var (sourceKeyType, sourceValueType) = this.strategy.SourceType.GetKeyAndValueTypes(context.Compilation);
        var loopTemporary = context.NextTemporary();

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"foreach (System.Collections.Generic.KeyValuePair<{sourceKeyType.ToDisplayString()}, {sourceValueType.ToDisplayString()}> {loopTemporary} in {source})");
        using (builder.CurlyBracesBlock())
        {
            // Process the source
            var sourceKeyTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceKeyType.ToDisplayString()} {sourceKeyTemporary} = {loopTemporary}.Key;");

            var (targetKeyTemporary, targetKeyStrategyCode) = this.strategy.KeyStrategy.GetBuilder().BuildSource(sourceKeyTemporary, context, mappaGlobalOptions);
            builder.AppendLine(targetKeyStrategyCode);
            builder.AppendEmptyLine();

            // Process the target.
            var valueTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceValueType.ToDisplayString()} {valueTemporary} = {loopTemporary}.Value;");
            var (targetValueTemporary, targetValueStrategyCode) = this.strategy.ValueStrategy.GetBuilder().BuildSource(valueTemporary, context, mappaGlobalOptions);
            builder.AppendLine(targetValueStrategyCode);
            builder.AppendEmptyLine();

            // Assign using the indexer.
            builder.AppendEmptyLine();

            // TODO [#111] The indexer setter property could be implemented explicitly so a conversion might be needed.
            builder.AppendLine($"{context.GetCompositeTypeTargetName()}.{this.strategy.TargetProperty.Name}[{targetKeyTemporary}] = {targetValueTemporary};");
        }

        return (string.Empty, builder.ToString());
    }
}