// <copyright file="ByteArrayToObjectIdMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ByteArrayToObjectIdMapStrategy"/> strategy.
/// </summary>
internal sealed class ByteArrayToObjectIdMapStrategyBuilder
   : IMappaStrategyBuilder
{
    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"global::MongoDB.Bson.ObjectId {temporary} = new global::MongoDB.Bson.ObjectId({source});";
        return (temporary, code);
    }
}