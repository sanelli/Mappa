// <copyright file="StringToNumberMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToNumberMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToNumberMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToNumberMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToNumberMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToNumberMapStrategyBuilder(StringToNumberMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetType = this.strategy.TargetType.ToDisplayString();
        var parameters = source;

        if (this.strategy.CultureInfoSetting is not CultureInfoSetting.Undefined &&
            this.strategy.CultureInfoSetting is not CultureInfoSetting.None)
        {
            switch (this.strategy.CultureInfoSetting)
            {
                case CultureInfoSetting.Undefined:
                case CultureInfoSetting.None:
                    break;
                case CultureInfoSetting.CurrentCulture:
                    parameters = $"{parameters}, System.Globalization.CultureInfo.CurrentCulture";
                    break;
                case CultureInfoSetting.InvariantCulture:
                    parameters = $"{parameters}, System.Globalization.CultureInfo.InvariantCulture";
                    break;
                case CultureInfoSetting.UserDefined:
                    if (!string.IsNullOrWhiteSpace(this.strategy.CultureName))
                    {
                        parameters = $"{parameters}, System.Globalization.CultureInfo.GetCultureInfo(\"{this.strategy.CultureName}\")";
                    }
                    else
                    {
                        throw new MappaGeneratorException("Reached the scenario where we are trying to build using user defined custom culture without culture name.");
                    }

                    break;
                default:
                    throw new MappaGeneratorException($"Unexpected culture info setting '{this.strategy.CultureInfoSetting}'.");
            }
        }

        var temporary = context.NextTemporary();
        var code = $"{targetType} {temporary} = {targetType}.Parse({parameters});";

        return (temporary, code);
    }
}