// <copyright file="ProtobufDoubleValueToDoubleMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ProtobufDoubleValueToDoubleMapStrategy"/> strategy.
/// </summary>
internal sealed class ProtobufDoubleValueToDoubleMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"double {temporary} = {source}.Value;";
        return (temporary, code);
    }
}