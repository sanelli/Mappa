// <copyright file="MappaAssignFromContextAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MappaAssignFromContextAttributeStrategy"/>.
/// </summary>
internal sealed class MappaAssignFromContextAttributeStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly MappaAssignFromContextAttributeStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignFromContextAttributeStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public MappaAssignFromContextAttributeStrategyBuilder(MappaAssignFromContextAttributeStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc />
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetType = this.strategy.TargetType.ToDisplayString();
        var code = $"{targetType} {temporary} = ({targetType}) {this.strategy.ContextParameterName}[\"{this.strategy.Attribute.ItemName}\"];";
        return (temporary, code);
    }
}