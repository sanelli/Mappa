// <copyright file="StringToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
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
    public (string StrategySource, string Header) BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new IndentStringBuilder();

        if (mappaGlobalOptions.MappaDebugComments)
        {
            builder.AppendLine($"/* Mappa Rule: {this.strategy.Rule} */ ");
        }

        var enumFullName = this.strategy.TargetType.ToDisplayString();
        var temporary = context.NextTemporary();
        builder.AppendLine($"{enumFullName} {temporary};");
        builder.AppendLine($"switch ({this.strategy.Source})");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var enumValues = this.strategy.TargetType.GetEnumValues();
            foreach (var enumValue in enumValues)
            {
                var enumValueFullName = $"{enumFullName}.{enumValue.Name}";
                builder.AppendLine($"case nameof({enumValueFullName}):");
                using (builder.CodeBlock())
                using (builder.Indent())
                {
                    builder.AppendLine($"{temporary} = {enumValueFullName};");
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