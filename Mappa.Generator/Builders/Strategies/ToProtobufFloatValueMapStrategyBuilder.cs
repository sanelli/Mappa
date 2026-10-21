// <copyright file="ToProtobufFloatValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ToProtobufFloatValueMapStrategy"/> strategy.
/// </summary>
internal sealed class ToProtobufFloatValueMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.FloatValue {temporary} = new global::Google.Protobuf.WellKnownTypes.FloatValue() {{ Value = {source} }};";
        return (temporary, code);
    }
}