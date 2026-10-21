// <copyright file="DateTimeToProtobufTimestampMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateTimeToProtobufTimestampMapStrategy"/> strategy.
/// </summary>
internal sealed class DateTimeToProtobufTimestampMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.Timestamp {temporary} = global::Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime({source});";
        return (temporary, code);
    }
}