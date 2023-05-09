// <copyright file="ToReferenceNullableMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ToReferenceNullableMapStrategy"/> strategy.
/// </summary>
internal sealed class ToReferenceNullableMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ToReferenceNullableMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToReferenceNullableMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ToReferenceNullableMapStrategyBuilder(ToReferenceNullableMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (inner strategy: {this.strategy.InnerStrategy.Rule}) */ "
            : string.Empty;

        var (innerVariable, innerStrategyCode) = this.strategy.InnerStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);

        return ($"{ruleComment}{innerVariable}", innerStrategyCode);
    }
}