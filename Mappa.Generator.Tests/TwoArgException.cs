// <copyright file="TwoArgException.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests;

/// <summary>
/// Fixture type with only a two-argument constructor so polymorphism throw generation rejects it.
/// </summary>
#pragma warning disable CA1812
internal sealed class TwoArgException
#pragma warning restore CA1812
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TwoArgException"/> class.
    /// </summary>
    /// <param name="first">First argument.</param>
    /// <param name="second">Second argument.</param>
    public TwoArgException(int first, int second)
    {
        _ = first;
        _ = second;
    }
}