// <copyright file="MethodMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MethodMapStrategy"/>.
/// </summary>
internal sealed class MethodMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly MethodMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public MethodMapStrategyBuilder(MethodMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();

        var methodName = this.strategy.MapMethod.MethodName;
        if (!string.IsNullOrWhiteSpace(this.strategy.MapMethod.AccessFieldName))
        {
            methodName = $"{this.strategy.MapMethod.AccessFieldName}.{methodName}";
        }

        var contextParameter = string.Empty;
        if (this.strategy.MapMethod.RequireMappaContextWhenInvoked() &&
            this.strategy.ContextParameterName is not null)
        {
            contextParameter = $", {this.strategy.ContextParameterName}";
        }

        var code = $"{this.strategy.TargetType.ToDisplayString()} {temporary} = {methodName}({source}{contextParameter});";

        return (temporary, code);
    }
}