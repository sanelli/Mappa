// <copyright file="IdentityStrategyMapperDup.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Sample mapper to demonstrate the identity strategy
/// across various scenarios.
/// </summary>
[Mappa]
public sealed partial class IdentityStrategyMapperDup
{
#nullable enable
    /// <summary>
    /// Map <see cref="int"/> to nullable <see cref="int"/>
    /// when nullable is enabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial int? MapIntToNullableIntWhenNullableIsEnabled(int input);
#nullable restore
}