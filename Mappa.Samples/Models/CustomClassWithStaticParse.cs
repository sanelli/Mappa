// <copyright file="CustomClassWithStaticParse.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Class with a custom static Parse method.
/// </summary>
public sealed class CustomClassWithStaticParse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomClassWithStaticParse"/> class.
    /// </summary>
    /// <param name="theString">A string.</param>
    /// <param name="gibberish">Just some nonsense.</param>
    public CustomClassWithStaticParse(string theString, int gibberish)
    {
        this.TheString = theString;
        this.Gibberish = gibberish;
    }

    /// <summary>
    /// Gets the string.
    /// </summary>
    public string TheString { get; }

    /// <summary>
    /// Gets some gibberish so the constructor is not used to map from string to this class.
    /// </summary>
    public int Gibberish { get; }

    /// <summary>
    /// Convert a string into a <see cref="CustomClassWithStaticParse"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed string as <see cref="CustomClassWithStaticParse"/>.</returns>
    public static CustomClassWithStaticParse Parse(string s) => new(s, 123);
}