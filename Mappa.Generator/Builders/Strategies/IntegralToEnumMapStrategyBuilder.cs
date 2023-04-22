// <copyright file="IntegralToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="IntegralToEnumMapStrategy"/> strategy.
/// </summary>
internal sealed class IntegralToEnumMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly IntegralToEnumMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegralToEnumMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public IntegralToEnumMapStrategyBuilder(IntegralToEnumMapStrategy strategy)
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

        var enumFullName = this.strategy.TargetType.ToDisplayString();
        var enumUnderlyingType = ((INamedTypeSymbol)this.strategy.TargetType).EnumUnderlyingType
                                 ?? throw new MappaGeneratorException($"The enum \"{enumFullName}\" does not have an underlying type");
        var temporary = context.NextTemporary();
        builder.AppendLine($"{enumFullName} {temporary};");
        builder.AppendLine($"switch (({enumUnderlyingType.ToDisplayString()}) {source})");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var enumValues = this.strategy.TargetType.GetEnumValues();
            foreach (var enumValue in enumValues)
            {
                var enumValueFullName = $"{enumFullName}.{enumValue.Name}";
                builder.AppendLine($"case {enumValue.Value}:");
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
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(\"{source}\");");
            }
        }

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (source-type is \"{this.strategy.SourceType.ToDisplayString()}\", target-enum is \"{enumFullName}\") */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", builder.ToString());
    }
}