// <copyright file="NullableToNullableMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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

        var temporary = context.NextTemporary();

        var builder = new IndentStringBuilder();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {temporary};");
        builder.AppendLine($"if ({source}.HasValue)");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var (innerStrategySource, innerHeader) = this.strategy.ChildStrategy.GetBuilder().BuildSource($"{source}.Value", context, mappaGlobalOptions);
            builder.AppendLine(innerHeader);
            builder.AppendLine($"{temporary} = {innerStrategySource};");
        }

        builder.AppendLine("else");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            builder.AppendLine($"{temporary} = ({this.strategy.TargetType.ToDisplayString()}) null;");
        }

        return ($"{ruleComment}{temporary}", builder.ToString());
    }
}