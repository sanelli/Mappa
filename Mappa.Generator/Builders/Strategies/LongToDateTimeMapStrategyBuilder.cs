// <copyright file="LongToDateTimeMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="LongToDateTimeMapStrategy"/> strategy.
/// </summary>
internal sealed class LongToDateTimeMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.DateTime {temporary} = System.DateTime.UnixEpoch.AddSeconds({source});";

        return (temporary, code);
    }
}