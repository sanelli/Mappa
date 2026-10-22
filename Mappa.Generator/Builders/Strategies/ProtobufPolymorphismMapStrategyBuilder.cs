// <copyright file="ProtobufPolymorphismMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ProtobufPolymorphismMapStrategy"/> strategy.
/// </summary>
/// <param name="strategy">The strategy.</param>
internal sealed class ProtobufPolymorphismMapStrategyBuilder(ProtobufPolymorphismMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly ProtobufPolymorphismMapStrategy strategy = strategy;

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var targetTemporary = context.NextTemporary();
        var targetTypeName = this.strategy.TargetType.ToDisplayNameWithoutNullableAnnotation();

        if (this.strategy.TargetIsProtobuf)
        {
            builder.AppendLine($"{targetTypeName} {targetTemporary} = new {targetTypeName}();");
        }
        else
        {
            builder.AppendLine($"{targetTypeName} {targetTemporary};");
        }

        builder.AppendLine($"switch ({this.BuildSwitchExpression(source)})");
        using (builder.CurlyBracesBlock())
        {
            foreach (var arm in this.strategy.Arms)
            {
                this.AppendArmSection(arm, source, targetTemporary, builder, context, mappaGlobalOptions);
                builder.AppendEmptyLine();
            }

            builder.AppendLine("default:");
            using (builder.CurlyBracesBlock())
            {
                this.AppendDefaultSection(source, targetTemporary, builder, context, mappaGlobalOptions);
            }
        }

        return (targetTemporary, builder.ToString());
    }

    private static void AppendArmAssignment(
        ProtobufPolymorphismArm arm,
        string targetTemporary,
        string armTargetVariable,
        PrettyCode.StringBuilder builder)
    {
        if (arm.TargetArmPropertyName is { } targetArmPropertyName)
        {
            builder.AppendLine($"{targetTemporary}.{targetArmPropertyName} = {armTargetVariable};");
        }
        else
        {
            builder.AppendLine($"{targetTemporary} = {armTargetVariable};");
        }
    }

    private string BuildSwitchExpression(string source)
        => this.strategy.SwitchesOnClrType
            ? source
            : $"{source}.{this.strategy.SourceCaseProperty?.Name ?? throw new MappaGeneratorException("The source oneof case property is missing.")}";

    private void AppendArmSection(
        ProtobufPolymorphismArm arm,
        string source,
        string targetTemporary,
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        var armSourceTemporary = context.NextTemporary();
        if (this.strategy.SwitchesOnClrType)
        {
            builder.AppendLine($"case {arm.Strategy.SourceType.ToDisplayString()} {armSourceTemporary}:");
        }
        else
        {
            var caseEnumTypeName = this.strategy.SourceCaseEnumType?.ToDisplayString()
                                   ?? throw new MappaGeneratorException("The source oneof case enum is missing.");
            builder.AppendLine($"case {caseEnumTypeName}.{arm.SourceCaseMemberName}:");
        }

        using (builder.CurlyBracesBlock())
        {
            if (!this.strategy.SwitchesOnClrType)
            {
                builder.AppendLine($"{arm.Strategy.SourceType.ToDisplayString()} {armSourceTemporary} = {source}.{arm.SourceArmPropertyName};");
            }

            var (armTargetVariable, armCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                arm.Strategy,
                armSourceTemporary,
                context,
                mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(armCode))
            {
                builder.AppendLine(armCode);
            }

            AppendArmAssignment(arm, targetTemporary, armTargetVariable, builder);
            builder.AppendLine("break;");
        }
    }

    private void AppendDefaultSection(
        string source,
        string targetTemporary,
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        switch (this.strategy.DefaultBehavior.Behavior)
        {
            case MappaProtobufTypeMappingDefaultBehavior.Throw:
                PolymorphismMapStrategyBuilder.AppendThrowStatement(
                    this.strategy.DefaultBehavior.ExceptionType,
                    source,
                    context,
                    builder);
                break;

            case MappaProtobufTypeMappingDefaultBehavior.NoMap:
                builder.AppendLine("break;");
                break;

            case MappaProtobufTypeMappingDefaultBehavior.MapTo:
                this.AppendMapToDefaultSection(source, targetTemporary, builder, context, mappaGlobalOptions);
                break;

            default:
                throw new MappaGeneratorException("Unexpected undefined behavior while generating default branch for protobuf type mapping.");
        }
    }

    private void AppendMapToDefaultSection(
        string source,
        string targetTemporary,
        PrettyCode.StringBuilder builder,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        var mapToArm = this.strategy.MapToArm
                       ?? throw new MappaGeneratorException("The MapTo arm has not been identified.");
        var (mapToVariable, mapToCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
            mapToArm.Strategy,
            source,
            context,
            mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(mapToCode))
        {
            builder.AppendLine(mapToCode);
        }

        AppendArmAssignment(mapToArm, targetTemporary, mapToVariable, builder);
        builder.AppendLine("break;");
    }
}