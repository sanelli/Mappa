// <copyright file="EnumerableConcreteTypeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type

using Mappa;
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating default <see cref="EnumerableConcreteTypeSetting.List"/> for <see cref="IEnumerable{T}"/> targets.
/// </summary>
[Mappa]
public sealed partial class EnumerableConcreteTypeListMapper
{
    /// <summary>
    /// Map an enumerable of <see cref="CountingValues"/> to an enumerable of <see cref="int"/> using the default list buffer.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The mapped collection.</returns>
    public partial IEnumerable<int> Map(IEnumerable<CountingValues> input);
}

/// <summary>
/// Mapper demonstrating <see cref="EnumerableConcreteTypeSetting.Array"/> for <see cref="IEnumerable{T}"/> targets.
/// </summary>
[Mappa]
[MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
public sealed partial class EnumerableConcreteTypeArrayMapper
{
    /// <summary>
    /// Map an enumerable of <see cref="CountingValues"/> to an enumerable of <see cref="int"/> using an array buffer.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The mapped collection.</returns>
    public partial IEnumerable<int> Map(IEnumerable<CountingValues> input);
}

/// <summary>
/// Mapper demonstrating that a concrete <see cref="List{T}"/> target remains a list even when
/// <see cref="EnumerableConcreteTypeSetting.Array"/> is enabled.
/// </summary>
[Mappa]
[MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
public sealed partial class EnumerableConcreteTypeExplicitListMapper
{
    /// <summary>
    /// Map an enumerable of <see cref="CountingValues"/> to a <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The mapped list.</returns>
    public partial List<int> Map(IEnumerable<CountingValues> input);
}

/// <summary>
/// Mapper demonstrating <see cref="EnumerableConcreteTypeSetting.Array"/> for <see cref="ICollection{T}"/> targets.
/// </summary>
[Mappa]
[MappaSettings(EnumerableConcreteType = EnumerableConcreteTypeSetting.Array)]
public sealed partial class EnumerableConcreteTypeArrayInterfaceMapper
{
    /// <summary>
    /// Map an enumerable of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/> using an array buffer.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The mapped collection.</returns>
    public partial ICollection<int> Map(IEnumerable<CountingValues> input);
}