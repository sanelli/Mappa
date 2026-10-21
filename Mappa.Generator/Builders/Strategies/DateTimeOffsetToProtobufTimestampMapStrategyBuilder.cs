// <copyright file="DateTimeOffsetToProtobufTimestampMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateTimeOffsetToProtobufTimestampMapStrategy"/> strategy.
/// </summary>
internal sealed class DateTimeOffsetToProtobufTimestampMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.Timestamp {temporary} = global::Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset({source}.ToUniversalTime());";
        return (temporary, code);
    }
}