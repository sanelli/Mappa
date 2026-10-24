// <copyright file="MappaAssignFromConstantAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MappaAssignFromConstantAttributeStrategy"/>.
/// </summary>
internal sealed class MappaAssignFromConstantAttributeStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly MappaAssignFromConstantAttributeStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignFromConstantAttributeStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public MappaAssignFromConstantAttributeStrategyBuilder(MappaAssignFromConstantAttributeStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetType = this.strategy.TargetType.ToDisplayString();
        var value = AttributeConstantValueHelper.ToCode(this.strategy.Attribute.Value);
        var code = $"{targetType} {temporary} = {value};";
        return (temporary, code);
    }
}