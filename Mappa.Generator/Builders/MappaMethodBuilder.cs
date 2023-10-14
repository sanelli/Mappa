// <copyright file="MappaMethodBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

namespace Mappa.Generator.Builders;

/// <summary>
/// Build a method.
/// </summary>
internal sealed class MappaMethodBuilder
    : IMappaBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMethodBuilder"/> class.
    /// </summary>
    /// <param name="classContext">The class generator context.</param>
    /// <param name="mapMethod">THe method to be generated.</param>
    public MappaMethodBuilder(MappaClassGeneratorContext classContext, MapMethod mapMethod)
    {
        this.ClassContext = classContext;
        this.MapMethod = mapMethod;
    }

    /// <summary>
    /// Gets the class generator context.
    /// </summary>
    private MappaClassGeneratorContext ClassContext { get; }

    /// <summary>
    /// Gets the map method.
    /// </summary>
    private MapMethod MapMethod { get; }

    /// <inheritdoc/>
    public string BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var isNullableEnabled = this.MapMethod.NullableEnabled;
        using (builder.NullableDirective(isNullableEnabled))
        {
            builder
                .AppendLine(new MappaGeneratedCodeAttributeBuilder().BuildSource(context, mappaGlobalOptions))
                .AppendLine(this.GetSignature());
            using (builder.CurlyBracesBlock())
            {
                var (strategySource, header) = this.MapMethod.Strategy.GetBuilder().BuildSource(this.MapMethod.SourceParameterName, context, mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(header))
                {
                    builder.AppendLine(header);
                    builder.AppendEmptyLine();
                }

                builder.AppendLine(strategySource);
            }
        }

        return builder.ToString();
    }

    private string GetSignature()
    {
        var modifiersWithReturnType = string.Join(
            " ",
            this.MapMethod.MethodSymbol.GetSymbolModifiers(),
            "partial",
            this.MapMethod.TargetType.ToDisplayString())
            .Trim();

        var parameters = $"{this.MapMethod.SourceType.ToDisplayString()} {this.MapMethod.SourceParameterName}";

        var signature = $"{modifiersWithReturnType} {this.MapMethod.MethodName}({parameters})";

        return signature;
    }
}