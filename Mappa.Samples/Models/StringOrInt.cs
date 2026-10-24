// <copyright file="StringOrInt.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using OneOf;

namespace Mappa.Samples.Models;

/// <summary>
/// Sample <c>OneOfBase</c> union of <see cref="string"/> and <see cref="int"/>.
/// </summary>
public sealed class StringOrInt
    : OneOfBase<string, int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringOrInt"/> class.
    /// </summary>
    /// <param name="input">The underlying <c>OneOf</c> value.</param>
    public StringOrInt(OneOf<string, int> input)
        : base(input)
    {
    }

    /// <summary>
    /// Implicitly wraps a <see cref="string"/> into <see cref="StringOrInt"/>.
    /// </summary>
    /// <param name="value">The string value.</param>
    public static implicit operator StringOrInt(string value)
        => FromString(value);

    /// <summary>
    /// Wraps a <see cref="string"/> into <see cref="StringOrInt"/>.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The wrapped value.</returns>
    public static StringOrInt FromString(string value)
        => new(value);
}