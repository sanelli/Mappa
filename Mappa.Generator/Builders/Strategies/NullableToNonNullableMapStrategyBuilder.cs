// <copyright file="NullableToNonNullableMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="NullableToNonNullableMapStrategy"/> strategy.
/// </summary>
internal sealed class NullableToNonNullableMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly NullableToNonNullableMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableToNonNullableMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public NullableToNonNullableMapStrategyBuilder(NullableToNonNullableMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (inner strategy: {this.strategy.InnerStrategy.Rule}) */ "
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

            var (innerVariable, innerStrategyCode) = this.strategy.InnerStrategy.GetBuilder().BuildSource(temporary, context, mappaGlobalOptions);

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
            builder.AppendLine($"throw new System.NullReferenceException(\"\\\"{source}\\\" is null.\");");
        }

        return ($"{ruleComment}{returnValue}", builder.ToString());
    }
}