// <copyright file="InvokeParseStringWithFormatMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeParseStringWithFormatMapStrategy"/>.
/// </summary>
internal sealed class InvokeParseStringWithFormatMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeParseStringWithFormatMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeParseStringWithFormatMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeParseStringWithFormatMapStrategyBuilder(InvokeParseStringWithFormatMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var (parseMethod, parameters) = ParseDateTimeStylesCodeHelper.BuildDateTimeOrDateTimeOffsetParseInvocation(
            source,
            this.strategy.Format,
            this.strategy.CultureInfoSetting,
            this.strategy.CultureName,
            this.strategy.DateTimeStyle);

        var temporary = context.NextTemporary();
        var code = $"{this.strategy.TargetType.ToDisplayString()} {temporary} = {this.strategy.TargetType.ToDisplayString()}.{parseMethod}({parameters});";

        return (temporary, code);
    }
}