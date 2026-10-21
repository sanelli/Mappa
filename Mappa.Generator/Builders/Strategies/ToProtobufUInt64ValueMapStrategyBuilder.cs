// <copyright file="ToProtobufUInt64ValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ToProtobufUInt64ValueMapStrategy"/> strategy.
/// </summary>
internal sealed class ToProtobufUInt64ValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.UInt64Value {temporary} = new global::Google.Protobuf.WellKnownTypes.UInt64Value() {{ Value = {source} }};";
        return (temporary, code);
    }
}