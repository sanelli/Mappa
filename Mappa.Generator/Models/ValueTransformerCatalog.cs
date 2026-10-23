// <copyright file="ValueTransformerCatalog.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// The value transformers resolved for a single root map method.
/// </summary>
internal sealed class ValueTransformerCatalog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueTransformerCatalog"/> class.
    /// </summary>
    /// <param name="applyTransformers">The transformers introduced by <c>[MappaApplyTransformer]</c>, in attribute order.</param>
    /// <param name="classTransformers">The <c>[MappaTransformer]</c> transformers: local, base hierarchy, then dependencies.</param>
    internal ValueTransformerCatalog(
        IReadOnlyList<ValueTransformer> applyTransformers,
        IReadOnlyList<ValueTransformer> classTransformers)
    {
        this.ApplyTransformers = applyTransformers;
        this.ClassTransformers = classTransformers;
    }

    /// <summary>
    /// Gets a catalog without any transformer.
    /// </summary>
    internal static ValueTransformerCatalog Empty { get; } = new([], []);

    /// <summary>
    /// Gets the transformers introduced by <c>[MappaApplyTransformer]</c>, in attribute order.
    /// </summary>
    internal IReadOnlyList<ValueTransformer> ApplyTransformers { get; }

    /// <summary>
    /// Gets the <c>[MappaTransformer]</c> transformers: local, base hierarchy, then dependencies.
    /// </summary>
    internal IReadOnlyList<ValueTransformer> ClassTransformers { get; }

    /// <summary>
    /// Gets a value indicating whether the catalog does not contain any transformer.
    /// </summary>
    internal bool IsEmpty => this.ApplyTransformers.Count == 0 && this.ClassTransformers.Count == 0;
}