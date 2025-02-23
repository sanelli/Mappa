// <copyright file="ReferenceNullableToReferenceNullableMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ReferenceNullableToReferenceNullableMapStrategy"/> strategy.
/// </summary>
internal sealed class ReferenceNullableToReferenceNullableMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ReferenceNullableToReferenceNullableMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceNullableToReferenceNullableMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ReferenceNullableToReferenceNullableMapStrategyBuilder(ReferenceNullableToReferenceNullableMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var returnValue = context.NextTemporary();
        var nonNullTemporary = context.NextTemporary();

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {returnValue};");
        builder.AppendLine($"if ({source} is not null)");
        using (builder.CurlyBracesBlock())
        {
            builder.AppendLine($"{this.strategy.SourceType.ToDisplayNameWithoutNullableAnnotation()} {nonNullTemporary} = {source};");
            var (innerVariable, innerStrategyCode) = this.strategy.InnerStrategy.GetBuilder().BuildSource(nonNullTemporary, context, mappaGlobalOptions);

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

        return (returnValue, builder.ToString());
    }
}