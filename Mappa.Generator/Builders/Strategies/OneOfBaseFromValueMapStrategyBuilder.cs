// <copyright file="OneOfBaseFromValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="OneOfBaseFromValueMapStrategy"/>.
/// </summary>
internal sealed class OneOfBaseFromValueMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly OneOfBaseFromValueMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfBaseFromValueMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public OneOfBaseFromValueMapStrategyBuilder(OneOfBaseFromValueMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetTypeName = this.strategy.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var mappingExpression = this.strategy.ConstructionKind switch
        {
            OneOfBaseConstructionKind.ImplicitFromArm => $"({targetTypeName}){source}",
            OneOfBaseConstructionKind.ConstructorFromOneOf => this.BuildConstructorFromOneOfExpression(source, targetTypeName),
            _ => throw new MappaGeneratorException($"Unexpected OneOfBase construction kind '{this.strategy.ConstructionKind}'."),
        };

        var code = $"{targetTypeName} {temporary} = {mappingExpression};";
        return (temporary, code);
    }

    private string BuildConstructorFromOneOfExpression(string source, string targetTypeName)
    {
        if (this.strategy.OptionStructType is null)
        {
            throw new MappaGeneratorException("ConstructorFromOneOf construction requires an OptionStructType.");
        }

        var optionStructTypeName = this.strategy.OptionStructType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var fromArm = $"{optionStructTypeName}.FromT{this.strategy.ArmIndex}({source})";
        return $"new {targetTypeName}({fromArm})";
    }
}