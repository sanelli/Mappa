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
    private readonly MappaMapAlgorithmContext parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="DerivedMappaMapAlgorithmContext"/> class.
    /// </summary>
    /// <param name="parent">The parent context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public DerivedMappaMapAlgorithmContext(
        MappaMapAlgorithmContext parent,
        ITypeSymbol targetType,
        ITypeSymbol sourceType)
    {
        this.parent = parent;
        this.SourceType = sourceType;
        this.TargetType = targetType;
    }

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    internal override bool IsNullableEnabled()
        => this.parent.IsNullableEnabled();

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.parent.TryGetMethod(targetType, sourceType, out mapMethod);

    /// <inheritdoc/>
    internal override void ReportDiagnostic(Diagnostic diagnostic)
        => this.parent.ReportDiagnostic(diagnostic);

    /// <inheritdoc/>
    internal override Location? GetLocation()
        => this.parent.GetLocation();
}