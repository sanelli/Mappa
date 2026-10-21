// <copyright file="StringToProtobufStringValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToProtobufStringValueMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToProtobufStringValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.StringValue {temporary} = new global::Google.Protobuf.WellKnownTypes.StringValue() {{ Value = {source} }};";
        return (temporary, code);
    }
}