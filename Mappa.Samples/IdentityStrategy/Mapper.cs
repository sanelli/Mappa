// <copyright file="Mapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.IdentityStrategy;

/// <summary>
/// Sample mapper to demonstrate the identity strategy.
/// </summary>
[Mappa]
public sealed partial class Mapper
{
#nullable disable
    /// <summary>
    /// Map <see cref="string"/> to <see cref="object"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial object MapStringToObjectWithNullableDisabled(string input);
#nullable restore
}