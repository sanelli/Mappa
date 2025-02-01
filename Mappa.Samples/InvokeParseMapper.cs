// <copyright file="InvokeParseMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the string-to-number strategy.
/// </summary>
[Mappa]
public sealed partial class ParseNumericMapper
{
    /// <summary>
    /// Map a string to integer.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial byte MapToByte(string input);

    /// <summary>
    /// Map a string to integer.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial short MapToShort(string input);

    /// <summary>
    /// Map a string to integer.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial int MapToInteger(string input);

    /// <summary>
    /// Map a string to long.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial long MapToLong(string input);

    /// <summary>
    /// Map a string to decimal.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial decimal MapToDecimal(string input);

    /// <summary>
    /// Map a string to float.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial float MapToFloat(string input);

    /// <summary>
    /// Map a string to double.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial double MapToDouble(string input);
}

// TODO String -> unsigned numeric types
// TODO String -> Uri
// TODO String -> * : On Method : Format
// TODO String -> * : On Method : Format + Invariant culture
// TODO String -> * : On Method : Invariant culture
// TODO String -> * : On Method : Current culture
// TODO String -> * : On Method : Custom culture
// TODO String -> * : On Class : Format
// TODO String -> * : On Class : Format + Invariant culture
// TODO String -> * : On Class : Invariant culture
// TODO String -> * : On Class : Current culture
// TODO String -> * : On Class : Custom culture
// TODO String -> * : On Method override On Class