// <copyright file="DateOnlyToLongMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateOnlyToLongMapStrategy"/> strategy.
/// </summary>
internal sealed class DateOnlyToLongMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"long {temporary} = (long)new System.DateTime({source}, System.TimeOnly.MinValue, System.DateTimeKind.Utc).ToUniversalTime().Subtract(System.DateTime.UnixEpoch).TotalSeconds;";

        return (temporary, code);
    }
}