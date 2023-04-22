// <copyright file="GenericMappaMethodGeneratorContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Generic mappa method generator context with a parent.
/// </summary>
internal sealed class GenericMappaMethodGeneratorContext
    : MappaMapAlgorithmContext
{
    private readonly MappaMapAlgorithmContext parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMappaMethodGeneratorContext"/> class.
    /// </summary>
    /// <param name="parent">The parent context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetPropertyName">The name of the target property.</param>
    /// <param name="sourcePropertyName">The source property name.</param>
    public GenericMappaMethodGeneratorContext(
        MappaMapAlgorithmContext parent,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string targetPropertyName,
        string sourcePropertyName)
    {
        this.parent = parent;
        this.SourceType = sourceType;
        this.TargetType = targetType;
        this.SourcePropertyName = sourcePropertyName;
        this.TargetPropertyName = targetPropertyName;
    }

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    internal override string SourcePropertyName { get; }

    /// <inheritdoc/>
    internal override string TargetPropertyName { get; }

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