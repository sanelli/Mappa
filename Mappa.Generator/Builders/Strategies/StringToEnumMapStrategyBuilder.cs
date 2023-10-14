// <copyright file="StringToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
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

        if (mappaGlobalOptions.MappaDebugComments)
        {
            builder.AppendLine($"/* Mappa Rule: {this.strategy.Rule} */ ");
        }

        var enumFullName = this.strategy.TargetType.ToDisplayString();
        var temporary = context.NextTemporary();
        builder.AppendLine($"{enumFullName} {temporary};");
        builder.AppendLine($"switch ({source})");
        using (builder.CurlyBracesBlock())
        {
            var enumValues = this.strategy.TargetType.GetEnumValues();
            foreach (var enumValue in enumValues)
            {
                var enumValueFullName = $"{enumFullName}.{enumValue.Name}";
                builder.AppendLine($"case nameof({enumValueFullName}):");
                using (builder.CurlyBracesBlock())
                {
                    builder.AppendLine($"{temporary} = {enumValueFullName};");
                    builder.AppendLine("break;");
                }
            }

            builder.AppendLine("default:");
            using (builder.CurlyBracesBlock())
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(\"{source}\");");
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (target-enum is \"{enumFullName}\") */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", builder.ToString());
    }
}