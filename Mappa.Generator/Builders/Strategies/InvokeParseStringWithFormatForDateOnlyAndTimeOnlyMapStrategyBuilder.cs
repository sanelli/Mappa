// <copyright file="InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder"/>.
/// </summary>
internal sealed class InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    internal InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder(InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var (parseMethod, parameters) = ParseDateTimeStylesCodeHelper.BuildParseInvocation(
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