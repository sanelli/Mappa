// <copyright file="ProtobufBytesValueToByteArrayMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ProtobufBytesValueToByteArrayMapStrategy"/> strategy.
/// </summary>
internal sealed class ProtobufBytesValueToByteArrayMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"byte[] {temporary} = {source}.Value.ToByteArray();";
        return (temporary, code);
    }
}