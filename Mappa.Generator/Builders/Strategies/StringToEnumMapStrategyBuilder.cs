// <copyright file="StringToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToEnumMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToEnumMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToEnumMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToEnumMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToEnumMapStrategyBuilder(StringToEnumMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();

        var enumFullName = this.strategy.TargetType.ToDisplayString();
        var temporary = context.NextTemporary();
        var caseInsensitive = this.strategy.CaseInsensitiveEnumMap is BooleanSetting.Enable;
        var switchExpression = caseInsensitive ? $"{source}.ToUpperInvariant()" : source;
        builder.AppendLine($"{enumFullName} {temporary};");
        builder.AppendLine($"switch ({switchExpression})");
        using (builder.CurlyBracesBlock())
        {
            EnumMapSwitchCodeHelper.AppendSwitchArms(
                builder,
                this.strategy.EnumMapConfiguration,
                temporary,
                source);
        }

        return (temporary, builder.ToString());
    }
}