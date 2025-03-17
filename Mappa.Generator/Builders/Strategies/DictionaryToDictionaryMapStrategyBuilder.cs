// <copyright file="DictionaryToDictionaryMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
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
        var (targetKeyType, targetValueType) = this.strategy.TargetType.GetKeyAndValueTypes(context.Compilation);
        var (sourceKeyType, sourceValueType) = this.strategy.SourceType.GetKeyAndValueTypes(context.Compilation);

        var dictionaryTemporary = context.NextTemporary();
        var loopTemporary = context.NextTemporary();

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"{GetTargetType()} {dictionaryTemporary} = new {GetNewType()}();");
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
            builder.AppendLine($"{dictionaryTemporary}[{targetKeyTemporary}] = {targetValueTemporary};");
        }

        if (this.strategy.TargetType.IsReadOnlyDictionary(context.Compilation))
        {
            var readOnlyTemporary = context.NextTemporary();
            builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {readOnlyTemporary} = new System.Collections.ObjectModel.ReadOnlyDictionary<{targetKeyType.ToDisplayString()},{targetValueType.ToDisplayString()}>({dictionaryTemporary});");
            dictionaryTemporary = readOnlyTemporary;
        }
        else if (this.strategy.TargetType.IsImmutableDictionary(context.Compilation))
        {
            var readOnlyTemporary = context.NextTemporary();
            builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {readOnlyTemporary} = System.Collections.Immutable.ToImmutableDictionary<{targetKeyType.ToDisplayString()},{targetValueType.ToDisplayString()}>({dictionaryTemporary});");
            dictionaryTemporary = readOnlyTemporary;
        }
        else if (this.strategy.TargetType.IsFrozenDictionary(context.Compilation))
        {
            var readOnlyTemporary = context.NextTemporary();
            builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {readOnlyTemporary} = System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary<{targetKeyType.ToDisplayString()},{targetValueType.ToDisplayString()}>({dictionaryTemporary});");
            dictionaryTemporary = readOnlyTemporary;
        }

        return (dictionaryTemporary, builder.ToString());

        string GetTargetType()
        {
            // For read-only targeted (i.e. IReadOnly, ReadOnly, Immutable and Frozen)
            // and for targeting IEnumerable<KeyValuePair<TKey, TValue>>.
            if ((this.strategy.TargetType.IsOrImplementIReadOnlyDictionary(context.Compilation)
                 || this.strategy.TargetType.IsIEnumerableOfKeyValuePairs(context.Compilation))
                && !this.strategy.TargetType.IsOrImplementIDictionary(context.Compilation))
            {
                return $"System.Collections.Generic.Dictionary<{targetKeyType.ToDisplayString()}, {targetValueType.ToDisplayString()}>";
            }

            return this.strategy.TargetType.ToDisplayString();
        }

        string GetNewType()
        {
            if (this.strategy.TargetType.IsIDictionary(context.Compilation)
                || this.strategy.TargetType.IsIEnumerableOfKeyValuePairs(context.Compilation)
                || this.strategy.TargetType.IsIReadOnlyDictionary(context.Compilation))
            {
                return $"System.Collections.Generic.Dictionary<{targetKeyType.ToDisplayString()}, {targetValueType.ToDisplayString()}>";
            }

            return this.strategy.TargetType.ToDisplayString();
        }
    }
}