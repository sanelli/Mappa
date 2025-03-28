// <copyright file="SourceClassWithCollections.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A class containing collection properties.
/// </summary>
public sealed class SourceClassWithCollections
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceClassWithCollections"/> class.
    /// </summary>
    /// <param name="propertyA">The A property.</param>
    /// <param name="propertyB">The B property.</param>
    /// <param name="propertyC">The C property.</param>
    /// <param name="propertyD">The D property.</param>
    /// <param name="propertyE">The E property.</param>
    /// <param name="propertyF">The F property.</param>
    /// <param name="propertyG">The G property.</param>
    /// <param name="propertyH">The H property.</param>
    /// <param name="propertyI">The I property.</param>
    /// <param name="propertyJ">The J property.</param>
    /// <param name="propertyK">The K property.</param>
    /// <param name="propertyL">The L property.</param>
    public SourceClassWithCollections(
        int[] propertyA,
        IList<int> propertyB,
        int[] propertyC,
        IList<int> propertyD,
        ICollection<int> propertyE,
        IEnumerable<int> propertyF,
        Dictionary<int, string> propertyG,
        IDictionary<int, string> propertyH,
        ICollection<int> propertyI,
        ICollection<int> propertyJ,
        Dictionary<int, string> propertyK,
        Dictionary<int, string> propertyL)
    {
        this.PropertyA = propertyA;
        this.PropertyB = propertyB;
        this.PropertyC = propertyC;
        this.PropertyD = propertyD;
        this.PropertyE = propertyE;
        this.PropertyF = propertyF;
        this.PropertyG = propertyG;
        this.PropertyH = propertyH;
        this.PropertyI = propertyI;
        this.PropertyJ = propertyJ;
        this.PropertyK = propertyK;
        this.PropertyL = propertyL;
    }

    /// <summary>
    /// Gets an array property.
    /// </summary>
 #pragma warning disable CA1819
    public int[] PropertyA { get; }
 #pragma warning restore CA1819

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public IList<int> PropertyB { get; }

    /// <summary>
    /// Gets an array property.
    /// </summary>
 #pragma warning disable CA1819
    public int[] PropertyC { get; }
 #pragma warning restore CA1819

    /// <summary>
    /// Gets a list property.
    /// </summary>
    public IList<int> PropertyD { get; }

    /// <summary>
    /// Gets a collection property.
    /// </summary>
 #pragma warning disable CA1819
    public ICollection<int> PropertyE { get; }
 #pragma warning restore CA1819

    /// <summary>
    /// Gets an enumerable property.
    /// </summary>
    public IEnumerable<int> PropertyF { get; }

    /// <summary>
    /// Gets a dictionary property.
    /// </summary>
    public Dictionary<int, string> PropertyG { get; }

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey,TValue}"/> property.
    /// </summary>
    public IDictionary<int, string> PropertyH { get; }

    /// <summary>
    /// Gets an enumerable property.
    /// </summary>
    public IEnumerable<int> PropertyI { get; }

    /// <summary>
    /// Gets an enumerable property.
    /// </summary>
    public IEnumerable<int> PropertyJ { get; }

    /// <summary>
    /// Gets a dictionary property.
    /// </summary>
    public Dictionary<int, string> PropertyK { get; }

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey,TValue}"/> property.
    /// </summary>
    public Dictionary<int, string> PropertyL { get; }
}