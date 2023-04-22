// <copyright file="NullableToNullableMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="NullableToNullableMapStrategy"/> strategy.
/// </summary>
internal sealed class NullableToNullableMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly NullableToNullableMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableToNullableMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public NullableToNullableMapStrategyBuilder(NullableToNullableMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (inner strategy: {this.strategy.ChildStrategy.Rule}) */ "
            : string.Empty;

        var returnValue = context.NextTemporary();

        var builder = new IndentStringBuilder();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {returnValue};");
        builder.AppendLine($"if ({source}.HasValue)");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var temporary = context.NextTemporary();
            var sourceUnderlyingType = this.strategy.SourceType.GetFirstGenericType();
            builder.AppendLine($"{sourceUnderlyingType} {temporary} = {source}.Value;");

            var (innerVariable, innerStrategyCode) = this.strategy.ChildStrategy.GetBuilder().BuildSource(temporary, context, mappaGlobalOptions);

            if (!string.IsNullOrWhiteSpace(innerStrategyCode))
            {
                builder.AppendLine(innerStrategyCode);
                builder.AppendEmptyLine();
            }

            builder.AppendLine($"{returnValue} = {innerVariable};");
        }

        builder.AppendLine("else");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            builder.AppendLine($"{returnValue} = ({this.strategy.TargetType.ToDisplayString()}) null;");
        }

        return ($"{ruleComment}{returnValue}", builder.ToString());
    }
}