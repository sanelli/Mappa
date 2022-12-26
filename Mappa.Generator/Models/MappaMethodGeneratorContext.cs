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

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal MapMethod MapMethod { get; }

    /// <inheritdoc/>
    internal override ITypeSymbol SourceType => throw new NotImplementedException();

    /// <inheritdoc/>
    internal override ITypeSymbol TargetType => throw new NotImplementedException();

    /// <inheritdoc/>
    internal override string PropertyName => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether <c>nullable</c> is enabled for the method.
    /// </summary>
    /// <returns><c>true</c> if the nullable context is enabled, <c>false</c> otherwise.</returns>
    internal override bool IsNullableEnabled()
        => this.ClassContext.IsNullableEnabled(this.MapMethod.MethodDeclarationSyntax);

    /// <inheritdoc/>
    internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        => this.ClassContext.TryGetMethod(targetType, sourceType, this.IsNullableEnabled(), out mapMethod);
}