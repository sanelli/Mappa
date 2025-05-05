// <copyright file="GuidToArrayLikeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="GuidToArrayLikeStrategy"/>.
/// </summary>
internal sealed class GuidToArrayLikeStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly GuidToArrayLikeStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuidToArrayLikeStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public GuidToArrayLikeStrategyBuilder(GuidToArrayLikeStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var targetTemporary = context.NextTemporary();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {targetTemporary} = {source}.ToByteArray();");
        return (targetTemporary, builder.ToString());
    }
}