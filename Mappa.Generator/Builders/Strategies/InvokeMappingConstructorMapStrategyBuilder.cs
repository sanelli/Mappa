// <copyright file="InvokeMappingConstructorMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeMappingConstructorMapStrategy"/>.
/// </summary>
internal sealed class InvokeMappingConstructorMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeMappingConstructorMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeMappingConstructorMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeMappingConstructorMapStrategyBuilder(InvokeMappingConstructorMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetTypeName = this.strategy.TargetType.ToDisplayString();
        var sourceTypeName = this.strategy.SourceType.ToDisplayString();

        var stringBuilder = new PrettyCode.StringBuilder();

        var sourceTemporary = context.NextTemporary();
        stringBuilder.AppendLine($"{sourceTypeName} {sourceTemporary} = {source};");

        var (parameterTemporary, parameterCode) = this.strategy.ArgumentStrategy.GetBuilder().BuildSource(sourceTemporary, context, mappaGlobalOptions);
        stringBuilder.AppendLine(parameterCode);
        stringBuilder.AppendEmptyLine();

        var targetTemporary = context.NextTemporary();
        var constructionExpression = InaccessibleMemberAccessHelper.BuildConstructorInvocationExpression(
            this.strategy.Constructor,
            [parameterTemporary],
            this.strategy.RequiresUnsafeAccessorOnConstructor,
            context);
        stringBuilder.AppendLine($"{targetTypeName} {targetTemporary} = {constructionExpression};");

        return (targetTemporary, stringBuilder.ToString());
    }
}