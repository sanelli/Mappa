// <copyright file="MappaUseConstructorMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Demonstrates <see cref="MappaUseConstructorAttribute"/> constructor selection.
/// </summary>
[Mappa]
public sealed partial class MappaUseConstructorMapper
{
    /// <summary>
    /// Forces the empty constructor when a mapping constructor would otherwise win.
    /// </summary>
    /// <param name="input">The source.</param>
    /// <returns>The mapped target.</returns>
    [MappaUseConstructor(ConstructorType.NoParameters)]
    public partial UseConstructorEmptyPreferredTarget MapForceEmpty(UseConstructorSource input);

    /// <summary>
    /// Forces a parameterized constructor when an empty constructor would otherwise win.
    /// </summary>
    /// <param name="input">The source.</param>
    /// <returns>The mapped target.</returns>
    [MappaUseConstructor(ConstructorType.WithParameters, typeof(string))]
    public partial UseConstructorParameterizedPreferredTarget MapForceParameterized(UseConstructorSource input);

    /// <summary>
    /// Forces an exact constructor signature via the generic attribute form.
    /// </summary>
    /// <param name="input">The source.</param>
    /// <returns>The mapped target.</returns>
    [MappaUseConstructor<int, string>]
    public partial UseConstructorExactSignatureTarget MapExactSignature(UseConstructorSource input);

    /// <summary>
    /// Forces a parameterized constructor on a nested target type
    /// (equivalent generic form: <c>[MappaUseConstructor&lt;string&gt;(typeof(UseConstructorNestedInnerTarget))]</c>).
    /// </summary>
    /// <param name="input">The outer source.</param>
    /// <returns>The mapped outer target.</returns>
    [MappaUseConstructor(typeof(UseConstructorNestedInnerTarget), ConstructorType.WithParameters, typeof(string))]
    public partial UseConstructorNestedOuterTarget MapNestedTyped(UseConstructorNestedOuterSource input);
}