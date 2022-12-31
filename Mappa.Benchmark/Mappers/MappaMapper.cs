// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Benchmark.Mappers;

/// <summary>
/// The Mappa mapper.
/// </summary>
[Attributes.Mappa]
public sealed partial class MappaMapper
{
#nullable disable
    /// <summary>
    /// Map <see cref="string"/> to <see cref="string"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial string MapStringToStringWhenNullableIsDisabled(string input);
#nullable restore

#nullable enable
    /// <summary>
    /// Map <see cref="string"/> to <see cref="string"/>
    /// when nullable is enabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial string? MapStringToStringWhenNullableIsEnabled(string? input);
#nullable restore

#nullable enable
    /// <summary>
    /// Map <see cref="string"/> to nullable <see cref="string"/>
    /// when nullable is enabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial string? MapStringToNullableStringWhenNullableIsEnabled(string input);
#nullable restore

#nullable disable
    /// <summary>
    /// Map <see cref="int"/> to <see cref="int"/>
    /// when nullable is disabled.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The output object.</returns>
    public partial int MapIntToIntWhenNullableIsDisabled(int input);
#nullable restore
#nullable disable
    /// <summary>
    /// Map a string to an object.
    /// Nullable is disabled.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The mapped object.</returns>
    public partial object MapStringToObjectWithNullableDisabled(string input);
#nullable restore

#nullable enable
    /// <summary>
    /// Map a string to a nullable object.
    /// Nullable is enabled.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The mapped object.</returns>
    public partial object? MapStringToNullableObjectWithNullableEnabled(string input);
#nullable restore
}