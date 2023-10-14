// <copyright file="NullableToNullableMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
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
            ? $"/* Mappa Rule: {this.strategy.Rule} (type argument strategy: {this.strategy.TypeArgumentStrategy.Rule}) */ "
            : string.Empty;

        var returnValue = context.NextTemporary();

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {returnValue};");
        builder.AppendLine($"if ({source}.HasValue)");
        using (builder.CurlyBracesBlock())
        {
            var temporary = context.NextTemporary();
            var sourceUnderlyingType = this.strategy.SourceType.GetElementType();
            builder.AppendLine($"{sourceUnderlyingType} {temporary} = {source}.Value;");

            var (innerVariable, innerStrategyCode) = this.strategy.TypeArgumentStrategy.GetBuilder().BuildSource(temporary, context, mappaGlobalOptions);

            if (!string.IsNullOrWhiteSpace(innerStrategyCode))
            {
                builder.AppendLine(innerStrategyCode);
                builder.AppendEmptyLine();
            }

            builder.AppendLine($"{returnValue} = {innerVariable};");
        }

        builder.AppendLine("else");
        using (builder.CurlyBracesBlock())
        {
            builder.AppendLine($"{returnValue} = ({this.strategy.TargetType.ToDisplayString()}) null;");
        }

        return ($"{ruleComment}{returnValue}", builder.ToString());
    }
}