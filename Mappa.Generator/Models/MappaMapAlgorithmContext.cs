// <copyright file="MappaMapAlgorithmContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describe the minimal properties needed to perform
/// a mapping.
/// </summary>
internal abstract class MappaMapAlgorithmContext
{
    /// <summary>
    /// Gets the parent symbol.
    /// </summary>
    internal abstract ISymbol ParentSymbol { get;  }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal abstract ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal abstract ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the method declaration syntax.
    /// </summary>
    internal abstract MapMethod? MapMethod { get; }

    /// <summary>
    /// Gets the active nested property path context, if any.
    /// </summary>
    internal virtual PropertyPathContext? PropertyPathContext => null;

    /// <summary>
    /// Gets the context settings.
    /// </summary>
    internal abstract MappaMapAlgorithmContextSettings AlgorithmSettings { get; }

    /// <summary>
    /// Gets the user settings built up to this point.
    /// </summary>
    internal abstract MappaUserSettings MappaUserSettings { get; }

    /// <summary>
    /// Gets a value indicating whether a diagnostic with severity error has been reported.
    /// </summary>
    internal abstract bool HasErrorDiagnostics { get; }

    /// <summary>
    /// Gets a value indicating whether the nullable flag
    /// is enabled in the current context.
    /// </summary>
    /// <returns><c>true</c> if nullable is enabled, <c>false</c> otherwise.</returns>
    internal abstract bool IsNullableEnabled();

    /// <summary>
    /// Try to obtain a method with the given <paramref name="targetType"/> and <paramref name="sourceType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="mapMethod">The map method (if it exists).</param>
    /// <returns><c>true</c> if map method exists, <c>false</c> otherwise.</returns>
    internal abstract bool TryGetMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        out MapMethod mapMethod);

    /// <summary>
    /// Try to obtain a polymorphic method with the given <paramref name="targetType"/> and <paramref name="sourceType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="mappaUserSettings">The user settings applied to the method being mapped.</param>
    /// <param name="mapMethod">The map method (if it exists).</param>
    /// <returns><c>true</c> if map method exists, <c>false</c> otherwise.</returns>
    internal abstract bool TryGetPolymorphicMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMappaUserSettings mappaUserSettings,
        out MapMethod mapMethod);

    /// <summary>
    /// Report a diagnostic.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to report.</param>
    internal abstract void ReportDiagnostic(Diagnostic diagnostic);

    /// <summary>
    /// Get the location being mapped.
    /// </summary>
    /// <returns>The location being mapped.</returns>
    internal abstract Location? GetLocation();

    /// <summary>
    /// Gets the map method.
    /// </summary>
    /// <returns>The map method <see cref="MapMethod"/>.</returns>
    /// <exception cref="MappaGeneratorException">When <see cref="MapMethod"/> is <c>null</c>.</exception>
    internal MapMethod GetMapMethod()
    {
        if (this.MapMethod is null)
        {
            throw new MappaGeneratorException("Map method is not defined.");
        }

        return this.MapMethod;
    }

    /// <summary>
    /// Gets the root map method which is actually being mapped.
    /// </summary>
    /// <returns>The map method from the root chain of calls.</returns>
    /// <exception cref="MappaGeneratorException">When the map method cannot be obtained.</exception>
    internal MapMethod GetRootMapMethod()
    {
        MappaMapAlgorithmContext context = this;
        while (context is DerivedMappaMapAlgorithmContext algorithmContext)
        {
            context = algorithmContext.ParentContext;
        }

        return context.GetMapMethod();
    }

    /// <summary>
    /// Gets the root source type for the map method being generated.
    /// </summary>
    /// <returns>The root source type.</returns>
    internal ITypeSymbol GetRootSourceType()
    {
        MappaMapAlgorithmContext context = this;
        while (context is DerivedMappaMapAlgorithmContext algorithmContext)
        {
            context = algorithmContext.ParentContext;
        }

        return context.SourceType;
    }
}