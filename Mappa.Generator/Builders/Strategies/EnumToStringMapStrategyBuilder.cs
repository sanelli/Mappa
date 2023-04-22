// <copyright file="EnumToStringMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumToStringMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumToStringMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly EnumToStringMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToStringMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumToStringMapStrategyBuilder(EnumToStringMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new IndentStringBuilder();

        if (mappaGlobalOptions.MappaDebugComments)
        {
            builder.AppendLine($"/* Mappa Rule: {this.strategy.Rule} */ ");
        }

        var enumFullName = this.strategy.SourceType.ToDisplayString();
        var temporary = context.NextTemporary();
        builder.AppendLine($"string {temporary};");
        builder.AppendLine($"switch ({source})");
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
                    builder.AppendLine($"{temporary} = nameof({enumValueFullName});");
                    builder.AppendLine("break;");
                }
            }

            builder.AppendLine($"default:");
            using (builder.CodeBlock())
            using (builder.Indent())
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(\"{source}\");");
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (source-enum is \"{enumFullName}\") */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", builder.ToString());
    }
}