// <copyright file="DecimalToDecimal128MapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DecimalToDecimal128MapStrategy"/> strategy.
/// </summary>
internal sealed class DecimalToDecimal128MapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::MongoDB.Bson.Decimal128 {temporary} = new global::MongoDB.Bson.Decimal128({source});";
        return (temporary, code);
    }
}