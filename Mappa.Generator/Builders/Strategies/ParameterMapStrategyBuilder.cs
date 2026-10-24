// <copyright file="ParameterMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for strategy <see cref="ParameterMapStrategy"/>.
/// </summary>
internal sealed class ParameterMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ParameterMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ParameterMapStrategyBuilder(ParameterMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();

        var sourceMemberTemporary = string.Empty;
        if (this.strategy.SourceProperty is not null)
        {
            sourceMemberTemporary = context.NextTemporary();
            var sourceReadExpression = InaccessibleMemberAccessHelper.BuildPropertyReadExpression(
                source,
                this.strategy.SourceProperty,
                this.strategy.RequiresUnsafeAccessorOnSource,
                this.strategy.RequiresInterfaceCastOnSource,
                context);
            builder.AppendLine($"{this.strategy.SourceProperty.Type.ToDisplayString()} {sourceMemberTemporary} = {sourceReadExpression};");
        }
        else if (this.strategy.SourceField is not null)
        {
            sourceMemberTemporary = context.NextTemporary();
            var sourceReadExpression = InaccessibleMemberAccessHelper.BuildFieldReadExpression(
                source,
                this.strategy.SourceField,
                this.strategy.RequiresUnsafeAccessorOnSource,
                context);
            builder.AppendLine($"{this.strategy.SourceField.Type.ToDisplayString()} {sourceMemberTemporary} = {sourceReadExpression};");
        }

        (string targetTemporary, string code) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            this.strategy.ParameterStrategy,
            sourceMemberTemporary,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(code))
        {
            builder.AppendLine(code);
            builder.AppendEmptyLine();
        }

        return (targetTemporary, builder.ToString());
    }
}