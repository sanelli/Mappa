// <copyright file="NullSubstituteMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="NullSubstituteMapStrategy"/>.
/// </summary>
internal sealed class NullSubstituteMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly NullSubstituteMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullSubstituteMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public NullSubstituteMapStrategyBuilder(NullSubstituteMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();
        var substitutedTemporary = context.NextTemporary();
        var sourceTypeName = this.strategy.SourceType.ToDisplayString();
        var literal = AttributeConstantValueHelper.ToCode(this.strategy.AssignValue);
        stringBuilder.AppendLine($"{sourceTypeName} {substitutedTemporary} = {source} ?? {literal};");

        var (elementTemporary, elementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            this.strategy.ElementStrategy,
            substitutedTemporary,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrEmpty(elementCode))
        {
            stringBuilder.AppendEmptyLine();
            stringBuilder.AppendLine(elementCode);
        }

        return (elementTemporary, stringBuilder.ToString());
    }
}