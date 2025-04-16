// <copyright file="IdentityStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Sample mapper to demonstrate the identity strategy
/// across various scenarios.
/// </summary>
[Mappa]
public sealed partial class IdentityStrategyMapper
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="string"/>
    /// when nullable is enabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial string? MapStringToString(string? input);

    /// <summary>
    /// Map <see cref="int"/> to <see cref="int"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial int MapIntToIntWhenNullableIsDisabled(int input);

    /// <summary>
    /// Map <see cref="int"/> to nullable <see cref="int"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial int? MapIntToNullableIntWhenNullableIsDisabled(int input);

    /// <summary>
    /// Map <see cref="int"/> to <see cref="object"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial object MapIntToObjectWhenNullableIsDisabled(int input);

    /// <summary>
    /// Map <see cref="int"/> to nullable <see cref="object"/>
    /// when nullable is enabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial object? MapIntToNullableObjectWhenNullableIsEnabled(int input);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="object"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial object MapStringToObjectWhenNullableIsDisabled(string input);
}