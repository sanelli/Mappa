// <copyright file="OneOfToValueMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="OneOfToValueMapStrategy"/>.
/// </summary>
internal sealed class OneOfToValueMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly OneOfToValueMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfToValueMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public OneOfToValueMapStrategyBuilder(OneOfToValueMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var targetTypeName = this.strategy.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var armIndex = this.strategy.ArmIndex;
        var code = $"{targetTypeName} {temporary} = {source}.IsT{armIndex} ? {source}.AsT{armIndex} : throw new global::Mappa.MappaException({this.BuildExceptionMessageLiteral()});";
        return (temporary, code);
    }

    private string BuildExceptionMessageLiteral()
    {
        var sourceDisplay = this.strategy.SourceType.ToDisplayString();
        var targetDisplay = this.strategy.TargetType.ToDisplayString();
        var message = $"Cannot complete mapping from '{sourceDisplay}' to '{targetDisplay}': the OneOf value is not T{this.strategy.ArmIndex}.";
        return CSharpLiteralHelper.ToStringLiteral(message);
    }
}