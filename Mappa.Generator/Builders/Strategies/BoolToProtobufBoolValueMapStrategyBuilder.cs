// <copyright file="BoolToProtobufBoolValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="BoolToProtobufBoolValueMapStrategy"/> strategy.
/// </summary>
internal sealed class BoolToProtobufBoolValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.BoolValue {temporary} = new global::Google.Protobuf.WellKnownTypes.BoolValue() {{ Value = {source} }};";
        return (temporary, code);
    }
}