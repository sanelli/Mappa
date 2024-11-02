// <copyright file="InvokeParseStringWithFormatMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
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
        // Default: TTarget.Parse(string)
        var parameters = source;
        var parseMethod = "Parse";

        if (this.strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
            this.strategy.CultureInfoSetting is not CultureInfoSetting.None)
        {
            // Use: TTarget.ParsExact(string, string, IFormatProvider)
            if (!string.IsNullOrWhiteSpace(this.strategy.Format))
            {
                parseMethod = "ParseExact";
                parameters = $"{source}, \"{this.strategy.Format}\", {GetCultureParameter(this.strategy)}";
            }
            else
            {
                // Use: TTarget.Parse(string, IFormatProvider)
                parameters = $"{source}, {GetCultureParameter(this.strategy)}";
            }
        }

        var temporary = context.NextTemporary();
        var code = $"{this.strategy.TargetType.ToDisplayString()} {temporary} = {this.strategy.TargetType.ToDisplayString()}.{parseMethod}({parameters});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);

        static string GetCultureParameter(InvokeParseStringWithFormatMapStrategy strategy)
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

                    // TODO [#56] This case should generate a warning and we should returning an empty string that will be ignored by the caller.
                    return "System.Globalization.CultureInfo.CurrentCulture";
            }

            throw new MappaGeneratorException($"Unexpected culture info setting '{strategy.CultureInfoSetting}'.");
        }
    }
}