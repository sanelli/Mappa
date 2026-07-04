// <copyright file="IdentityMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="IdentityMapStrategy"/> strategy.
/// </summary>
internal sealed class IdentityMapStrategyBuilder(IdentityMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly IdentityMapStrategy strategy = strategy;

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        if (this.strategy.IdentityMapDeepCopySetting is IdentityMapDeepCopySetting.ShallowCopy
            && !this.strategy.RequiresMemberwiseClone
            && this.strategy.NestedFieldStrategies.Count == 0)
        {
            return (source, string.Empty);
        }

        if (this.strategy.IdentityMapDeepCopySetting is IdentityMapDeepCopySetting.DeepCopy
            && this.strategy.RequiresMemberwiseClone
            && this.strategy.NestedFieldStrategies.Count == 0)
        {
            return this.BuildMemberwiseClone(source, context);
        }

        if (this.strategy.IdentityMapDeepCopySetting is IdentityMapDeepCopySetting.NestedDeepCopy)
        {
            return this.BuildNestedDeepCopy(source, context, mappaGlobalOptions);
        }

        return (source, string.Empty);
    }

    private (string VariableName, string Code) BuildMemberwiseClone(string source, MappaBuilderContext context)
    {
        var cloneTemporary = context.NextTemporary();
        var typeDisplayString = this.strategy.TargetType.ToDisplayString();
        var code = $"{typeDisplayString} {cloneTemporary} = ({typeDisplayString}){source}.MemberwiseClone();";
        return (cloneTemporary, code);
    }

    private (string VariableName, string Code) BuildNestedDeepCopy(
        string source,
        MappaBuilderContext context,
        MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var cloneTemporary = context.NextTemporary();
        var typeDisplayString = this.strategy.TargetType.ToDisplayString();

        if (this.strategy.IsStructRoot)
        {
            builder.AppendLine($"{typeDisplayString} {cloneTemporary} = {source};");
        }
        else
        {
            builder.AppendLine($"{typeDisplayString} {cloneTemporary} = ({typeDisplayString}){source}.MemberwiseClone();");
        }

        foreach (var nestedFieldStrategy in this.strategy.NestedFieldStrategies)
        {
            var fieldSourceTemporary = context.NextTemporary();
            builder.AppendLine($"{nestedFieldStrategy.Field.Type.ToDisplayString()} {fieldSourceTemporary} = {source}.{nestedFieldStrategy.Field.Name};");
            var (mappedTemporary, mappedCode) = nestedFieldStrategy.FieldStrategy.GetBuilder()
                .BuildSource(fieldSourceTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(mappedCode))
            {
                builder.AppendLine(mappedCode);
            }

            builder.AppendLine($"{cloneTemporary}.{nestedFieldStrategy.Field.Name} = {mappedTemporary};");
        }

        return (cloneTemporary, builder.ToString());
    }
}