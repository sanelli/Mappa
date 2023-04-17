// <copyright file="EnumToIntegralMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumToIntegralMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumToIntegralMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly EnumToIntegralMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToIntegralMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumToIntegralMapStrategyBuilder(EnumToIntegralMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string StrategySource, string Header) BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new IndentStringBuilder();

        if (mappaGlobalOptions.MappaDebugComments)
        {
            builder.AppendLine($"/* Mappa Rule: {this.strategy.Rule} */ ");
        }

        var enumFullName = this.strategy.SourceType.ToDisplayString();
        var temporary = context.NextTemporary();
        builder.AppendLine($"var {temporary} = default({this.strategy.TargetType.ToDisplayString()});");
        builder.AppendLine($"switch ({this.strategy.Source})");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var enumValues = this.strategy.SourceType.GetEnumValues();
            foreach (var enumValue in enumValues)
            {
                var enumValueFullName = $"{enumFullName}.{enumValue.Name}";
                builder.AppendLine($"case {enumValueFullName}:");
                using (builder.CodeBlock())
                using (builder.Indent())
                {
                    builder.AppendLine($"{temporary} = {enumValue.Value};");
                    builder.AppendLine("break;");
                }
            }

            builder.AppendLine($"default:");
            using (builder.CodeBlock())
            using (builder.Indent())
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(nameof({this.strategy.Source}));");
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (source-enum is \"{enumFullName}\") */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", builder.ToString());
    }
}