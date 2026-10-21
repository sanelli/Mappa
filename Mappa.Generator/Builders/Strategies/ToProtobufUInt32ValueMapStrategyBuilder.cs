// <copyright file="ToProtobufUInt32ValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ToProtobufUInt32ValueMapStrategy"/> strategy.
/// </summary>
internal sealed class ToProtobufUInt32ValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.UInt32Value {temporary} = new global::Google.Protobuf.WellKnownTypes.UInt32Value() {{ Value = {source} }};";
        return (temporary, code);
    }
}