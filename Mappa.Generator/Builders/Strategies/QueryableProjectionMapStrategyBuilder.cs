// <copyright file="QueryableProjectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Expressions;
using Mappa.Generator.Exceptions;
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
        var expressionContext = new ExpressionBuildContext(context, mappaGlobalOptions);
        if (!ProjectionExpressionBuilder.TryBuildExpression(
                this.strategy.ElementStrategy,
                lambdaParameter,
                expressionContext,
                out var elementExpression))
        {
            throw new MappaGeneratorException("Queryable projection element strategy is not supported.");
        }

        var mapMethod = context.GetMapMethod();
        var elementMethodName = $"{this.strategy.MethodSymbol.Name}Element";
        context.ProjectionElementMethods.Add(new ProjectionElementMethodDefinition(
            elementMethodName,
            this.strategy.SourceElementType,
            this.strategy.TargetElementType,
            lambdaParameter,
            elementExpression,
            this.strategy.MethodSymbol.IsStatic,
            mapMethod.NullableEnabled));

        var selectExpression =
            $"global::System.Linq.Queryable.Select({source}, {lambdaParameter} => {elementExpression})";
        return (selectExpression, string.Empty);
    }
}