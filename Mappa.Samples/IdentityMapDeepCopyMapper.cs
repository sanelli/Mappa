// <copyright file="IdentityMapDeepCopyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type

using Mappa;
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating default shallow same-type identity mapping.
/// </summary>
[Mappa]
public sealed partial class IdentityMapDeepCopyShallowMapper
{
    /// <summary>
    /// Map a person to itself using the default shallow identity copy.
    /// </summary>
    /// <param name="input">The input person.</param>
    /// <returns>The mapped person.</returns>
    public partial IdentityMapDeepCopyPerson Map(IdentityMapDeepCopyPerson input);
}

/// <summary>
/// Mapper demonstrating same-type identity mapping with <see cref="IdentityMapDeepCopySetting.DeepCopy"/>.
/// </summary>
[Mappa]
[MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.DeepCopy)]
public sealed partial class IdentityMapDeepCopyDeepMapper
{
    /// <summary>
    /// Map a person to itself using <see cref="IdentityMapDeepCopySetting.DeepCopy"/>.
    /// </summary>
    /// <param name="input">The input person.</param>
    /// <returns>The mapped person.</returns>
    public partial IdentityMapDeepCopyPerson Map(IdentityMapDeepCopyPerson input);
}

/// <summary>
/// Mapper demonstrating same-type identity mapping with <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/>.
/// </summary>
[Mappa]
[MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy)]
public sealed partial class IdentityMapDeepCopyNestedMapper
{
    /// <summary>
    /// Map a person to itself using <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/>.
    /// </summary>
    /// <param name="input">The input person.</param>
    /// <returns>The mapped person.</returns>
    public partial IdentityMapDeepCopyPerson Map(IdentityMapDeepCopyPerson input);
}

/// <summary>
/// Mapper demonstrating struct same-type identity mapping with <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/>.
/// </summary>
[Mappa]
[MappaSettings(IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy)]
public sealed partial class IdentityMapDeepCopyNestedStructMapper
{
    /// <summary>
    /// Map a struct to itself using <see cref="IdentityMapDeepCopySetting.NestedDeepCopy"/>.
    /// </summary>
    /// <param name="input">The input struct.</param>
    /// <returns>The mapped struct.</returns>
    public partial IdentityMapDeepCopyStruct Map(IdentityMapDeepCopyStruct input);
}