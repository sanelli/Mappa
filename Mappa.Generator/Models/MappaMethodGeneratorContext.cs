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
    /// <param name="mappaUserSettings">The user settings.</param>
    /// <param name="mapMethod">The method to be mapped.</param>
    public MappaMethodGeneratorContext(
        MappaClassGeneratorContext classContext,
        MappaUserSettings mappaUserSettings,
        MapMethod mapMethod)
    {
        this.ClassContext = classContext;
        this.MapMethod = mapMethod;
        this.MappaUserSettings = mappaUserSettings;
    }

    /// <summary>
    /// Gets the parent class context.
    /// </summary>
    internal MappaClassGeneratorContext ClassContext { get; }

    /// <inheritdoc/>
    internal override MapMethod? MapMethod { get; }

    /// <inheritdoc/>
    internal override ISymbol ParentSymbol => this.GetMapMethod().ContainingType;

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType => this.GetMapMethod().SourceType;

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType => this.GetMapMethod().TargetType;

    /// <inheritdoc/>
    internal override MappaMapAlgorithmContextSettings AlgorithmSettings { get; } = new();

    /// <inheritdoc/>
    internal override MappaUserSettings MappaUserSettings { get; }

    /// <inheritdoc/>
    internal override bool HasErrorDiagnostics => this.ClassContext.HasErrorDiagnostics;

    /// <inheritdoc/>
    internal override bool IsNullableEnabled()
        => this.GetMapMethod().NullableEnabled;

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.ClassContext.TryGetMethod(targetType, sourceType, this.IsNullableEnabled(), this.GetRootMapMethod().CanBeUsedByStaticMethod, out mapMethod);

    /// <inheritdoc/>
    internal override bool TryGetPolymorphicMethod(ITypeSymbol targetType, ITypeSymbol sourceType, IMappaUserSettings mappaUserSettings, out MapMethod mapMethod)
        => this.ClassContext.TryGetPolymorphicMethod(targetType, sourceType, this.IsNullableEnabled(), this.GetRootMapMethod().CanBeUsedByStaticMethod, mappaUserSettings, out mapMethod);

    /// <inheritdoc/>
    internal override bool TryGetCompatibleMethod(ITypeSymbol targetType, ITypeSymbol sourceType, Compilation compilation, out MapMethod mapMethod)
        => this.ClassContext.TryGetCompatibleMethod(targetType, sourceType, this.GetRootMapMethod().CanBeUsedByStaticMethod, compilation, out mapMethod);

    /// <inheritdoc/>
    internal override void ReportDiagnostic(Diagnostic diagnostic)
        => this.ClassContext.ReportDiagnostic(diagnostic);

    /// <inheritdoc/>
    internal override Location? GetLocation()
        => this.GetMapMethod().Location;
}