// <copyright file="ByteArrayToProtobufBytesValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ByteArrayToProtobufBytesValueMapStrategy"/> strategy.
/// </summary>
internal sealed class ByteArrayToProtobufBytesValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.BytesValue {temporary} = new global::Google.Protobuf.WellKnownTypes.BytesValue() {{ Value = global::Google.Protobuf.ByteString.CopyFrom({source}) }};";
        return (temporary, code);
    }
}