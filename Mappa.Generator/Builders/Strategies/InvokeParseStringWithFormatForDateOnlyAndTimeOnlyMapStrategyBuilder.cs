// <copyright file="InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
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
        var hasFormat = !string.IsNullOrWhiteSpace(this.strategy.Format);
        var parameters = hasFormat ? $"{source}, \"{this.strategy.Format}\"" : source;
        var parseMethod = hasFormat ? "ParseExact" : "Parse";

        if (this.strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
            this.strategy.CultureInfoSetting is not CultureInfoSetting.None)
        {
            parameters = $"{parameters}, {GetCultureParameter(this.strategy)}";
        }

        var temporary = context.NextTemporary();
        var code = $"{this.strategy.TargetType.ToDisplayString()} {temporary} = {this.strategy.TargetType.ToDisplayString()}.{parseMethod}({parameters});";

        return (temporary, code);

        static string GetCultureParameter(InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy strategy)
        {
            switch (strategy.CultureInfoSetting)
            {
                case CultureInfoSetting.CurrentCulture:
                    return "System.Globalization.CultureInfo.CurrentCulture";
                case CultureInfoSetting.InvariantCulture:
                    return "System.Globalization.CultureInfo.InvariantCulture";
                case CultureInfoSetting.UserDefined:
                    if (!string.IsNullOrWhiteSpace(strategy.CultureName))
                    {
                        return $"System.Globalization.CultureInfo.GetCultureInfo(\"{strategy.CultureName}\")";
                    }

                    throw new MappaGeneratorException("Unexpected scenario when building GeyCultureInfo without culture name");
            }

            throw new MappaGeneratorException($"Unexpected culture info setting '{strategy.CultureInfoSetting}'.");
        }
    }
}