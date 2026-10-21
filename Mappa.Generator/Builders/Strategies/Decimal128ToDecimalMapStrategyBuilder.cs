// <copyright file="Decimal128ToDecimalMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="Decimal128ToDecimalMapStrategy"/> strategy.
/// </summary>
internal sealed class Decimal128ToDecimalMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"decimal {temporary} = global::MongoDB.Bson.Decimal128.ToDecimal({source});";
        return (temporary, code);
    }
}