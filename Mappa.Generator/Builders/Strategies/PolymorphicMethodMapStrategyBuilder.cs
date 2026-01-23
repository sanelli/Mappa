// <copyright file="PolymorphicMethodMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="PolymorphicMethodMapStrategy"/>.
/// </summary>
internal sealed class PolymorphicMethodMapStrategyBuilder(PolymorphicMethodMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly PolymorphicMethodMapStrategy strategy = strategy;

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

        var code = $"{this.strategy.TargetType.ToDisplayString()} {temporary} = ({this.strategy.TargetType.ToDisplayString()}) {methodName}({source}{contextParameter});";

        return (temporary, code);
    }
}