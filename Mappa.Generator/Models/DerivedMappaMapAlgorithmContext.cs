// <copyright file="DerivedMappaMapAlgorithmContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Generic mappa method generator context with a parent.
/// </summary>
/// <param name="parentContext">The parent context.</param>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="propertyPathContext">The active nested property path context.</param>
internal sealed class DerivedMappaMapAlgorithmContext(
    MappaMapAlgorithmContext parentContext,
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    PropertyPathContext? propertyPathContext = null)
    : MappaMapAlgorithmContext
{
    /// <summary>
    /// Gets the parent context.
    /// </summary>
    internal MappaMapAlgorithmContext ParentContext { get; } = parentContext;

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType { get; } = sourceType;

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType { get; } = targetType;

    /// <inheritdoc/>
    internal override ISymbol ParentSymbol
        => this.ParentContext.ParentSymbol;

    /// <inheritdoc/>
    internal override MappaMapAlgorithmContextSettings AlgorithmSettings
        => this.ParentContext.AlgorithmSettings;

    /// <inheritdoc/>
    internal override MappaUserSettings MappaUserSettings => this.ParentContext.MappaUserSettings;

    /// <inheritdoc/>
    internal override bool HasErrorDiagnostics => this.ParentContext.HasErrorDiagnostics;

    /// <inheritdoc/>
    internal override MapMethod? MapMethod => null;

    /// <inheritdoc/>
    internal override PropertyPathContext? PropertyPathContext => propertyPathContext;

    /// <inheritdoc/>
    internal override bool IsNullableEnabled()
        => this.ParentContext.IsNullableEnabled();

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.ParentContext.TryGetMethod(targetType, sourceType, out mapMethod);

    /// <inheritdoc/>
    internal override bool TryGetPolymorphicMethod(ITypeSymbol targetType, ITypeSymbol sourceType, IMappaUserSettings mappaUserSettings, out MapMethod mapMethod)
        => this.ParentContext.TryGetPolymorphicMethod(targetType, sourceType, mappaUserSettings, out mapMethod);

    /// <inheritdoc/>
    internal override void ReportDiagnostic(Diagnostic diagnostic)
        => this.ParentContext.ReportDiagnostic(diagnostic);

    /// <inheritdoc/>
    internal override Location? GetLocation()
        => this.ParentContext.GetLocation();
}