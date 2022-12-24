// <copyright file="MappaMapAlgorithmContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describe the minimal properties needed to perform
/// a mapping.
/// </summary>
internal abstract class MappaMapAlgorithmContext
{
    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal abstract ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal abstract ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the name of the property used to generate the source.
    /// </summary>
    internal abstract string PropertyName { get; }

    /// <summary>
    /// Gets a value indicating weather the nullable flag
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
    internal abstract bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod);
}