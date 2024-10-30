// <copyright file="InvokeToStringMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeToStringMapStrategy"/> strategy.
/// </summary>
internal sealed class InvokeToStringMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeToStringMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeToStringMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeToStringMapStrategyBuilder(InvokeToStringMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        string parameters;
        if (this.strategy.CultureInfoSetting is not null && this.strategy.CultureInfoSetting is not MappaSettingsAttribute.CultureInfoSettings.None
            && !string.IsNullOrWhiteSpace(this.strategy.Format))
        {
            parameters = $"\"{this.strategy.Format}\", {GetCulture(this.strategy.CultureInfoSetting.Value, this.strategy.CultureName)}";
        }
        else if (this.strategy.CultureInfoSetting is not null && this.strategy.CultureInfoSetting is not MappaSettingsAttribute.CultureInfoSettings.None)
        {
            parameters = $"\"{GetCulture(this.strategy.CultureInfoSetting.Value, this.strategy.CultureName)}";
        }
        else if (!string.IsNullOrWhiteSpace(this.strategy.Format))
        {
            parameters = $"\"{this.strategy.Format}\"";
        }
        else
        {
            parameters = string.Empty;
        }

        var code = $"string {temporary} = {source}.ToString({parameters});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }

    private static string GetCulture(MappaSettingsAttribute.CultureInfoSettings cultureInfoSettings, string? cultureName)
        => cultureInfoSettings switch
        {
            MappaSettingsAttribute.CultureInfoSettings.None => string.Empty,
            MappaSettingsAttribute.CultureInfoSettings.CurrentCulture => "System.Globalization.CultureInfo.CurrentCulture",
            MappaSettingsAttribute.CultureInfoSettings.InvariantCulture => "System.Globalization.CultureInfo.InvariantCulture",
            MappaSettingsAttribute.CultureInfoSettings.UserDefined => $"System.Globalization.CultureInfo.GetCultureInfo(\"{cultureName ?? string.Empty}\")",
            _ => throw new ArgumentOutOfRangeException(nameof(cultureInfoSettings), cultureInfoSettings, null),
        };
}