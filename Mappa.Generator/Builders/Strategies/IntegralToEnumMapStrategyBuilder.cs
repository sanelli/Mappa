// <copyright file="IntegralToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
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
        var builder = new PrettyCode.StringBuilder();

        var enumFullName = this.strategy.TargetType.ToDisplayString();
        var enumUnderlyingType = ((INamedTypeSymbol)this.strategy.TargetType).EnumUnderlyingType
                                 ?? throw new MappaGeneratorException($"The enum \"{enumFullName}\" does not have an underlying type");
        var temporary = context.NextTemporary();
        builder.AppendLine($"{enumFullName} {temporary};");
        builder.AppendLine($"switch (({enumUnderlyingType.ToDisplayString()}) {source})");
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