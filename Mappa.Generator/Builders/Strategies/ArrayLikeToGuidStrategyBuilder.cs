// <copyright file="ArrayLikeToGuidStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="GuidToArrayLikeStrategy"/>.
/// </summary>
internal sealed class ArrayLikeToGuidStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ArrayLikeToGuidStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayLikeToGuidStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ArrayLikeToGuidStrategyBuilder(ArrayLikeToGuidStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();
        var targetTemporary = context.NextTemporary();
        string getSpanFromMemory = this.strategy.SourceType.IsMemory(context.Compilation)
                             || this.strategy.SourceType.IsReadOnlyMemory(context.Compilation)
            ? ".Span"
            : string.Empty;

        stringBuilder.AppendLine($"global::System.Guid {targetTemporary} = new global::System.Guid({source}{getSpanFromMemory});");

        return (targetTemporary, stringBuilder.ToString());
    }
}