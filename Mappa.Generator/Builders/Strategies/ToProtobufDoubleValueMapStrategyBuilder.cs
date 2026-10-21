// <copyright file="ToProtobufDoubleValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ToProtobufDoubleValueMapStrategy"/> strategy.
/// </summary>
internal sealed class ToProtobufDoubleValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.DoubleValue {temporary} = new global::Google.Protobuf.WellKnownTypes.DoubleValue() {{ Value = {source} }};";
        return (temporary, code);
    }
}