// <copyright file="PreferredInterfaceMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Demonstrates deterministic preferred-interface selection for multi-interface collections.
/// </summary>
[Mappa]
public sealed partial class PreferredInterfaceMapper
{
    /// <summary>
    /// Maps <see cref="DualEnumerable"/> to <see cref="List{T}"/> of <see cref="int"/> using the unique
    /// usable implicit <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The dual enumerable source.</param>
    /// <returns>The mapped list of integers.</returns>
    public partial List<int> MapImplicitInt(DualEnumerable input);

    /// <summary>
    /// Forces the explicit <see cref="IEnumerable{T}"/> of <see cref="string"/> via a source preference attribute.
    /// </summary>
    /// <param name="input">The dual enumerable source.</param>
    /// <returns>The mapped list of strings.</returns>
    [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<string>))]
    public partial List<string> MapExplicitString(DualEnumerable input);

    /// <summary>
    /// Nested DTO mapping that uses the implicit <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The source DTO.</param>
    /// <returns>The target DTO with integer items.</returns>
    public partial PreferredInterfaceIntTargetDto MapNestedImplicit(PreferredInterfaceSourceDto input);

    /// <summary>
    /// Nested DTO mapping that forces the explicit string interface on the nested <see cref="DualEnumerable"/>.
    /// </summary>
    /// <param name="input">The source DTO.</param>
    /// <returns>The target DTO with string items.</returns>
    [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<string>))]
    public partial PreferredInterfaceStringTargetDto MapNestedExplicitString(PreferredInterfaceSourceDto input);

    /// <summary>
    /// Maps integers into <see cref="DualCollection"/> forcing the explicit string enumerable interface
    /// (without the attribute, the unique usable implicit interface would be <see cref="IEnumerable{T}"/> of <see cref="int"/>).
    /// </summary>
    /// <param name="input">The source integers.</param>
    /// <returns>The dual collection populated on the string side via <c>ToString</c>.</returns>
    [MappaTargetPreferredInterfaceImplementation(typeof(DualCollection), typeof(IEnumerable<string>))]
    public partial DualCollection MapToDualCollectionStrings(List<int> input);

    /// <summary>
    /// Identity list mapping that intentionally carries an unused preferred-interface attribute.
    /// </summary>
    /// <param name="input">The input list.</param>
    /// <returns>The same list.</returns>
#pragma warning disable MP00082 // Intentional: attribute targets DualEnumerable but the map never consults that type
    [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<int>))]
#pragma warning restore MP00082
    public partial List<int> MapWithUnusedPreferredInterfaceAttribute(List<int> input);
}