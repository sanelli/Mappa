// <copyright file="QueryableProjectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="QueryableProjectionMapStrategy"/>.
/// </summary>
internal sealed class QueryableProjectionMapStrategyBuilder(QueryableProjectionMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly QueryableProjectionMapStrategy strategy = strategy;

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var lambdaParameter = context.NextTemporary();
        var (elementResult, elementCode) = this.strategy.ElementStrategy.GetBuilder()
            .BuildSource(lambdaParameter, context, mappaGlobalOptions);

        var builder = new PrettyCode.StringBuilder();
        if (!string.IsNullOrWhiteSpace(elementCode))
        {
            builder.AppendLine(elementCode);
        }

        var selectExpression =
            $"global::System.Linq.Queryable.Select({source}, {lambdaParameter} => {elementResult})";
        return (selectExpression, builder.ToString());
    }
}