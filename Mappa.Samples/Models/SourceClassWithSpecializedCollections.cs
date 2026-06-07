// <copyright file="SourceClassWithSpecializedCollections.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source class for mapping to readonly specialized collection target properties.
/// </summary>
public sealed class SourceClassWithSpecializedCollections
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceClassWithSpecializedCollections"/> class.
    /// </summary>
    /// <param name="propertyA">The stack source property.</param>
    /// <param name="propertyB">The queue source property.</param>
    /// <param name="propertyC">The concurrent stack source property.</param>
    /// <param name="propertyD">The concurrent queue source property.</param>
    /// <param name="propertyE">The concurrent bag source property.</param>
    /// <param name="propertyF">The blocking collection source property.</param>
    public SourceClassWithSpecializedCollections(
        int[] propertyA,
        int[] propertyB,
        int[] propertyC,
        int[] propertyD,
        int[] propertyE,
        IEnumerable<int> propertyF)
    {
        this.PropertyA = propertyA;
        this.PropertyB = propertyB;
        this.PropertyC = propertyC;
        this.PropertyD = propertyD;
        this.PropertyE = propertyE;
        this.PropertyF = propertyF;
    }

    /// <summary>
    /// Gets the stack source property.
    /// </summary>
#pragma warning disable CA1819
    public int[] PropertyA { get; }
#pragma warning restore CA1819

    /// <summary>
    /// Gets the queue source property.
    /// </summary>
#pragma warning disable CA1819
    public int[] PropertyB { get; }
#pragma warning restore CA1819

    /// <summary>
    /// Gets the concurrent stack source property.
    /// </summary>
#pragma warning disable CA1819
    public int[] PropertyC { get; }
#pragma warning restore CA1819

    /// <summary>
    /// Gets the concurrent queue source property.
    /// </summary>
#pragma warning disable CA1819
    public int[] PropertyD { get; }
#pragma warning restore CA1819

    /// <summary>
    /// Gets the concurrent bag source property.
    /// </summary>
#pragma warning disable CA1819
    public int[] PropertyE { get; }
#pragma warning restore CA1819

    /// <summary>
    /// Gets the blocking collection source property.
    /// </summary>
    public IEnumerable<int> PropertyF { get; }
}