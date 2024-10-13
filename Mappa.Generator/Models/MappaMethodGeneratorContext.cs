// <copyright file="MappaMethodGeneratorContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Context describing the mapping of a single method.
/// </summary>
internal sealed class MappaMethodGeneratorContext
    : MappaMapAlgorithmContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMethodGeneratorContext"/> class.
    /// </summary>
    /// <param name="classContext">The context of the parent class.</param>
    /// <param name="mapMethod">The method to be mapped.</param>
    public MappaMethodGeneratorContext(
        MappaClassGeneratorContext classContext,
        MapMethod mapMethod)
    {
        this.ClassContext = classContext;
        this.MapMethod = mapMethod;
    }

    /// <summary>
    /// Gets the parent class context.
    /// </summary>
    internal MappaClassGeneratorContext ClassContext { get; }

    /// <inheritdoc/>
    internal override MapMethod? MapMethod { get; }

    /// <inheritdoc/>
    internal override ISymbol ParentSymbol => this.GetMapMethod().MethodSymbol.ContainingSymbol;

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType => this.GetMapMethod().SourceType;

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType => this.GetMapMethod().TargetType;

    /// <inheritdoc/>
    internal override MappaMapAlgorithmContextSettings Settings { get; } = new();

    /// <inheritdoc/>
    internal override bool IsNullableEnabled()
        => this.GetMapMethod().NullableEnabled;

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.ClassContext.TryGetMethod(targetType, sourceType, this.IsNullableEnabled(), out mapMethod);

    /// <inheritdoc/>
    internal override void ReportDiagnostic(Diagnostic diagnostic)
        => this.ClassContext.ReportDiagnostic(diagnostic);

    /// <inheritdoc/>
    internal override Location? GetLocation()
        => this.GetMapMethod().Location;
}