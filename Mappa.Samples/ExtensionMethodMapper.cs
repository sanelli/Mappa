// <copyright file="ExtensionMethodMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper that uses extension methods.
/// </summary>
[Mappa]
public static partial class ExtensionMethodMapper
{
    /// <summary>
    /// Map from integer to long using an extension method.
    /// </summary>
    /// <param name="input">The integer input.</param>
    /// <returns>The integer output.</returns>
    public static partial long MapToLong(this int input);
}