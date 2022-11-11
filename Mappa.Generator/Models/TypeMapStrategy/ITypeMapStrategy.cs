// <copyright file="ITypeMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.TypeMapStrategy;

/// <summary>
/// Describe a strategy to map from a type to another.
/// </summary>
internal interface ITypeMapStrategy
{
    /// <summary>
    /// Gets the target type.
    /// </summary>
    ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    ITypeSymbol SourceType { get; }
}