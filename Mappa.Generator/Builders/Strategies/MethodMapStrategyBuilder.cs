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
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;
        var temporary = context.NextTemporary();

        var methodName = this.strategy.MapMethod.MethodName;
        if (!string.IsNullOrWhiteSpace(this.strategy.MapMethod.AccessFieldName))
        {
            methodName = $"{this.strategy.MapMethod.AccessFieldName}.{methodName}";
        }

        var code = $"{this.strategy.TargetType.ToDisplayString()} {temporary} = {methodName}({source});";

        return ($"{ruleComment}{temporary}", code);
    }
}