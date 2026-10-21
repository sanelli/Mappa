// <copyright file="TimeSpanToProtobufDurationMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="TimeSpanToProtobufDurationMapStrategy"/> strategy.
/// </summary>
internal sealed class TimeSpanToProtobufDurationMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::Google.Protobuf.WellKnownTypes.Duration {temporary} = global::Google.Protobuf.WellKnownTypes.Duration.FromTimeSpan({source});";
        return (temporary, code);
    }
}