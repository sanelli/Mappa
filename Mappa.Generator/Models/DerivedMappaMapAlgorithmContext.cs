// <copyright file="DerivedMappaMapAlgorithmContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Generic mappa method generator context with a parent.
/// </summary>
internal sealed class DerivedMappaMapAlgorithmContext
    : MappaMapAlgorithmContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DerivedMappaMapAlgorithmContext"/> class.
    /// </summary>
    /// <param name="parentContext">The parent context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public DerivedMappaMapAlgorithmContext(
        MappaMapAlgorithmContext parentContext,
        ITypeSymbol targetType,
        ITypeSymbol sourceType)
    {
        this.ParentContext = parentContext;
        this.SourceType = sourceType;
        this.TargetType = targetType;
    }

    /// <summary>
    /// Gets the parent context.
    /// </summary>
    internal MappaMapAlgorithmContext ParentContext { get; }

    /// <inheritdoc/>
    internal override ISymbol ParentSymbol
        => this.ParentContext.ParentSymbol;

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    internal override MappaMapAlgorithmContextSettings AlgorithmSettings
        => this.ParentContext.AlgorithmSettings;

    /// <inheritdoc/>
    internal override MapMethod? MapMethod => null;

    /// <inheritdoc/>
    internal override bool IsNullableEnabled()
        => this.ParentContext.IsNullableEnabled();

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.ParentContext.TryGetMethod(targetType, sourceType, out mapMethod);

    /// <inheritdoc/>
    internal override void ReportDiagnostic(Diagnostic diagnostic)
        => this.ParentContext.ReportDiagnostic(diagnostic);

    /// <inheritdoc/>
    internal override Location? GetLocation()
        => this.ParentContext.GetLocation();
}