// <copyright file="ProtobufTimestampToTimeOnlyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ProtobufTimestampToTimeOnlyMapStrategy"/> strategy.
/// </summary>
internal sealed class ProtobufTimestampToTimeOnlyMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.TimeOnly {temporary} = System.TimeOnly.FromDateTime({source}.ToDateTime());";
        return (temporary, code);
    }
}