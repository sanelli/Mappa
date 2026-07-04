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
        static EnumStringMapSetting GetEffectiveEnumStringMapSetting(EnumStringMapSetting enumStringMapSetting)
            => enumStringMapSetting is EnumStringMapSetting.Undefined
                ? EnumStringMapSetting.MemberName
                : enumStringMapSetting;

        var builder = new PrettyCode.StringBuilder();

        var enumFullName = this.strategy.TargetType.ToDisplayString();
        var temporary = context.NextTemporary();
        var caseInsensitive = this.strategy.CaseInsensitiveEnumMap is BooleanSetting.Enable;
        var switchExpression = caseInsensitive ? $"{source}.ToUpperInvariant()" : source;
        builder.AppendLine($"{enumFullName} {temporary};");
        builder.AppendLine($"switch ({switchExpression})");
        using (builder.CurlyBracesBlock())
        {
            if (GetEffectiveEnumStringMapSetting(this.strategy.EnumStringMapSetting) is EnumStringMapSetting.Description)
            {
                var membersWithDescriptions = this.strategy.TargetType.GetEnumMembersWithDescriptions(context.Compilation);
                foreach (var (memberName, description) in membersWithDescriptions)
                {
                    var enumValueFullName = $"{enumFullName}.{memberName}";
                    var caseLabel = caseInsensitive
                        ? TypeSymbolExtensions.ToCSharpStringLiteral(description.ToUpperInvariant())
                        : TypeSymbolExtensions.ToCSharpStringLiteral(description);
                    builder.AppendLine($"case {caseLabel}:");
                    using (builder.CurlyBracesBlock())
                    {
                        builder.AppendLine($"{temporary} = {enumValueFullName};");
                        builder.AppendLine("break;");
                    }
                }
            }
            else
            {
                var enumValues = this.strategy.TargetType.GetEnumValues();
                foreach (var enumName in enumValues.Select(enumValue => enumValue.Name))
                {
                    var enumValueFullName = $"{enumFullName}.{enumName}";
                    var caseLabel = caseInsensitive
                        ? $"\"{enumName.ToUpperInvariant()}\""
                        : $"nameof({enumValueFullName})";
                    builder.AppendLine($"case {caseLabel}:");
                    using (builder.CurlyBracesBlock())
                    {
                        builder.AppendLine($"{temporary} = {enumValueFullName};");
                        builder.AppendLine("break;");
                    }
                }
            }

            builder.AppendLine("default:");
            using (builder.CurlyBracesBlock())
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(\"{source}\");");
            }
        }

        return (temporary, builder.ToString());
    }
}