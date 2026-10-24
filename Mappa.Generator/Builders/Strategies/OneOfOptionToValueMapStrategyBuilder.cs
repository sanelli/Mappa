// <copyright file="OneOfOptionToValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="OneOfOptionToValueMapStrategy"/>.
/// </summary>
internal sealed class OneOfOptionToValueMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly OneOfOptionToValueMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfOptionToValueMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public OneOfOptionToValueMapStrategyBuilder(OneOfOptionToValueMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetTypeName = this.strategy.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var whenSome = $"{source}.AsT0";
        var whenNone = this.BuildAbsentExpression();
        var code = $"{targetTypeName} {temporary} = {source}.IsT0 ? {whenSome} : {whenNone};";
        return (temporary, code);
    }

    private string BuildAbsentExpression()
        => this.strategy.AbsentBehavior switch
        {
            OneOfOptionAbsentBehavior.Throw => $"throw new global::Mappa.MappaException({this.BuildExceptionMessageLiteral()})",
            OneOfOptionAbsentBehavior.ReturnNull => "null",
            OneOfOptionAbsentBehavior.ReturnDefault => this.strategy.TargetType.IsReferenceType
                ? "null"
                : "default",
            _ => throw new MappaGeneratorException($"Unexpected OneOf option absent behavior '{this.strategy.AbsentBehavior}'."),
        };

    private string BuildExceptionMessageLiteral()
    {
        var sourceDisplay = this.strategy.SourceType.ToDisplayString();
        var targetDisplay = this.strategy.TargetType.ToDisplayString();
        var message = $"Cannot complete mapping from '{sourceDisplay}' to '{targetDisplay}': the OneOf option value is None.";
        return CSharpLiteralHelper.ToStringLiteral(message);
    }
}