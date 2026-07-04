// <copyright file="EnumToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumToEnumMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumToEnumMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly EnumToEnumMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToEnumMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumToEnumMapStrategyBuilder(EnumToEnumMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        static EnumToEnumMapSetting GetEffectiveEnumToEnumMapSetting(EnumToEnumMapSetting enumToEnumMapSetting)
            => enumToEnumMapSetting is EnumToEnumMapSetting.Undefined
                ? EnumToEnumMapSetting.MemberName
                : enumToEnumMapSetting;

        var builder = new PrettyCode.StringBuilder();

        var sourceEnumFullType = this.strategy.SourceType.ToDisplayString();
        var targetEnumFullType = this.strategy.TargetType.ToDisplayString();

        var temporary = context.NextTemporary();
        builder.AppendLine($"{targetEnumFullType} {temporary};");
        builder.AppendLine($"switch ({source})");
        using (builder.CurlyBracesBlock())
        {
            if (GetEffectiveEnumToEnumMapSetting(this.strategy.EnumToEnumMapSetting) is EnumToEnumMapSetting.NumericValue)
            {
                var sharedEnumMemberMappings = this.strategy.SourceType.GetSharedEnumMemberMappingsByValue(this.strategy.TargetType);

                foreach (var (sourceMemberName, targetMemberName) in sharedEnumMemberMappings)
                {
                    builder.AppendLine($"case {sourceEnumFullType}.{sourceMemberName}:");
                    using (builder.CurlyBracesBlock())
                    {
                        builder.AppendLine($"{temporary} = {targetEnumFullType}.{targetMemberName};");
                        builder.AppendLine("break;");
                    }
                }
            }
            else
            {
                var sharedEnumNames = this.strategy.SourceType.GetSharedEnumMemberNamesByName(this.strategy.TargetType);

                foreach (var enumName in sharedEnumNames)
                {
                    builder.AppendLine($"case {sourceEnumFullType}.{enumName}:");
                    using (builder.CurlyBracesBlock())
                    {
                        builder.AppendLine($"{temporary} = {targetEnumFullType}.{enumName};");
                        builder.AppendLine("break;");
                    }
                }
            }

            builder.AppendLine($"default:");
            using (builder.CurlyBracesBlock())
            {
                builder.AppendLine($"throw new System.ArgumentOutOfRangeException(\"{source}\");");
            }
        }

        return (temporary, builder.ToString());
    }
}