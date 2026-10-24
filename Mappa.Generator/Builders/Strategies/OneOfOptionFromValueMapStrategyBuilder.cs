// <copyright file="OneOfOptionFromValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="OneOfOptionFromValueMapStrategy"/>.
/// </summary>
internal sealed class OneOfOptionFromValueMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private const string NoneTypeName = "global::OneOf.Types.None";

    private readonly OneOfOptionFromValueMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfOptionFromValueMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public OneOfOptionFromValueMapStrategyBuilder(OneOfOptionFromValueMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetTypeName = this.strategy.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var optionStructTypeName = this.strategy.OptionStructType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var valueTypeName = this.strategy.ValueType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var noneExpression = this.WrapOptionExpression($"{optionStructTypeName}.FromT1(default({NoneTypeName}))");

        var mappingExpression = this.strategy.WrapBehavior switch
        {
            OneOfOptionWrapBehavior.AlwaysSome => this.WrapOptionExpression($"{optionStructTypeName}.FromT0({source})"),
            OneOfOptionWrapBehavior.SomeWhenNotNull => $"{source} is not null ? {this.WrapOptionExpression($"{optionStructTypeName}.FromT0(({valueTypeName}){source})")} : {noneExpression}",
            OneOfOptionWrapBehavior.SomeWhenHasValue => $"{source}.HasValue ? {this.WrapOptionExpression($"{optionStructTypeName}.FromT0({source}.Value)")} : {noneExpression}",
            OneOfOptionWrapBehavior.SomeWhenNotDefault => $"!global::System.Collections.Generic.EqualityComparer<{valueTypeName}>.Default.Equals({source}, default({valueTypeName})) ? {this.WrapOptionExpression($"{optionStructTypeName}.FromT0({source})")} : {noneExpression}",
            _ => throw new MappaGeneratorException($"Unexpected OneOf option wrap behavior '{this.strategy.WrapBehavior}'."),
        };

        var code = $"{targetTypeName} {temporary} = {mappingExpression};";
        return (temporary, code);
    }

    private string WrapOptionExpression(string oneOfExpression)
    {
        if (!this.strategy.IsOneOfBaseTarget)
        {
            return oneOfExpression;
        }

        var targetTypeName = this.strategy.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return $"new {targetTypeName}({oneOfExpression})";
    }
}