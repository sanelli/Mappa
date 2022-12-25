// <copyright file="MappaMethodBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
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
    public string BuildSource()
    {
        var builder = new IndentStringBuilder();
        builder.AppendLine(this.GetSignature());

        using (builder.BeginCodeBlock())
        using (builder.Indent())
        {
            var (strategySource, header) = this.MapMethod.Strategy.GetBuilder().BuildSource();
            builder.AppendLine(header);
            builder.AppendLine(strategySource);
        }

        return builder.ToString();
    }

    private string GetSignature()
    {
        var modifiersWithReturnType = string.Join(
            " ",
            this.MapMethod.MethodSymbol.GetClassModifiers(),
            this.MapMethod.TargetType.ToDisplayString());

        var signature =
            $"{modifiersWithReturnType} {this.MapMethod.MethodDeclarationSyntax.Identifier}({this.MapMethod.SourceType.ToDisplayString()} {this.MapMethod.SourceParameterName})";

        return signature;
    }
}