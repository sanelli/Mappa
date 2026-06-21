// <copyright file="InvokeToStringMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
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
        if (this.strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
            this.strategy.CultureInfoSetting is not CultureInfoSetting.None &&
            !string.IsNullOrWhiteSpace(this.strategy.Format))
        {
            parameters = $"{CSharpLiteralHelper.ToRequiredStringLiteral(this.strategy.Format)}, {GetCulture(this.strategy.CultureInfoSetting, this.strategy.CultureName)}";
        }
        else if (this.strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
                 this.strategy.CultureInfoSetting is not CultureInfoSetting.None)
        {
            parameters = $"{GetCulture(this.strategy.CultureInfoSetting, this.strategy.CultureName)}";
        }
        else if (!string.IsNullOrWhiteSpace(this.strategy.Format))
        {
            parameters = CSharpLiteralHelper.ToRequiredStringLiteral(this.strategy.Format);
        }
        else
        {
            parameters = string.Empty;
        }

        var code = $"string {temporary} = {source}.ToString({parameters});";

        return (temporary, code);
    }

    private static string GetCulture(CultureInfoSetting cultureInfoSettings, string? cultureName)
        => cultureInfoSettings switch
        {
            CultureInfoSetting.CurrentCulture => "System.Globalization.CultureInfo.CurrentCulture",
            CultureInfoSetting.InvariantCulture => "System.Globalization.CultureInfo.InvariantCulture",
            CultureInfoSetting.UserDefined => $"System.Globalization.CultureInfo.GetCultureInfo({CSharpLiteralHelper.ToStringLiteral(cultureName ?? string.Empty)})",
            _ => throw new ArgumentOutOfRangeException(nameof(cultureInfoSettings), cultureInfoSettings, null),
        };
}