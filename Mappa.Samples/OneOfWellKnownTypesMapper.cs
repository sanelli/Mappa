// <copyright file="OneOfWellKnownTypesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

using OneOf;
using OneOf.Types;

namespace Mappa.Samples;

/// <summary>
/// Mapper showcasing <see cref="MappaSettingsAttribute.OneOfWellKnownTypes"/> for
/// Option (<c>OneOf&lt;T, None&gt;</c>), general <c>OneOf</c> unwrap, identity wrap, and <c>OneOfBase</c>.
/// </summary>
[Mappa]
[MappaSettings(OneOfWellKnownTypes = BooleanSetting.Enable)]
public sealed partial class OneOfWellKnownTypesMapper
{
    /// <summary>
    /// Unwraps Option to <see cref="string"/> (throws <see cref="MappaException"/> when None).
    /// </summary>
    /// <param name="source">The Option source.</param>
    /// <returns>The string value.</returns>
    public partial string MapOptionToString(OneOf<string, None> source);

    /// <summary>
    /// Unwraps Option to nullable <see cref="string"/> (returns <see langword="null"/> when None).
    /// </summary>
    /// <param name="source">The Option source.</param>
    /// <returns>The string value, or <see langword="null"/>.</returns>
    public partial string? MapOptionToNullableString(OneOf<string, None> source);

    /// <summary>
    /// Wraps a nullable <see cref="string"/> into Option (<c>None</c> when null).
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The Option.</returns>
    public partial OneOf<string, None> MapStringToOption(string? source);

    /// <summary>
    /// Unwraps <c>OneOf&lt;string, int&gt;</c> to the <see cref="string"/> arm.
    /// </summary>
    /// <param name="source">The OneOf source.</param>
    /// <returns>The string arm value.</returns>
    public partial string MapStringOrIntToString(OneOf<string, int> source);

    /// <summary>
    /// Wraps <see cref="string"/> into <c>OneOf&lt;string, int&gt;</c> via identity / implicit conversion
    /// (available even when <c>OneOfWellKnownTypes</c> is disabled).
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The OneOf value.</returns>
    public partial OneOf<string, int> MapStringToStringOrInt(string source);

    /// <summary>
    /// Unwraps a <c>OneOfBase</c>-derived source to the <see cref="string"/> arm.
    /// </summary>
    /// <param name="source">The OneOfBase source.</param>
    /// <returns>The string arm value.</returns>
    public partial string MapStringOrIntBaseToString(StringOrInt source);

    /// <summary>
    /// Wraps <see cref="string"/> into a constructible <c>OneOfBase</c> subclass.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The OneOfBase value.</returns>
    public partial StringOrInt MapStringToStringOrIntBase(string source);
}