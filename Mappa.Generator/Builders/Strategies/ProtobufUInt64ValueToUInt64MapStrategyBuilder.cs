// <copyright file="ProtobufUInt64ValueToUInt64MapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ProtobufUInt64ValueToUInt64MapStrategy"/> strategy.
/// </summary>
internal sealed class ProtobufUInt64ValueToUInt64MapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"ulong {temporary} = {source}.Value;";
        return (temporary, code);
    }
}