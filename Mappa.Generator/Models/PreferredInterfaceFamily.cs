// <copyright file="PreferredInterfaceFamily.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Open-generic collection/dictionary interface families used for preferred-interface selection.
/// </summary>
internal enum PreferredInterfaceFamily
{
    /// <summary>
    /// <c>IEnumerable&lt;T&gt;</c>.
    /// </summary>
    IEnumerable,

    /// <summary>
    /// <c>ICollection&lt;T&gt;</c>.
    /// </summary>
    ICollection,

    /// <summary>
    /// <c>IList&lt;T&gt;</c>.
    /// </summary>
    IList,

    /// <summary>
    /// <c>IReadOnlyCollection&lt;T&gt;</c>.
    /// </summary>
    IReadOnlyCollection,

    /// <summary>
    /// <c>IReadOnlyList&lt;T&gt;</c>.
    /// </summary>
    IReadOnlyList,

    /// <summary>
    /// <c>ISet&lt;T&gt;</c>.
    /// </summary>
    ISet,

    /// <summary>
    /// <c>IReadOnlySet&lt;T&gt;</c>.
    /// </summary>
    IReadOnlySet,

    /// <summary>
    /// <c>IDictionary&lt;TKey,TValue&gt;</c>.
    /// </summary>
    IDictionary,

    /// <summary>
    /// <c>IReadOnlyDictionary&lt;TKey,TValue&gt;</c>.
    /// </summary>
    IReadOnlyDictionary,

    /// <summary>
    /// <c>IQueryable&lt;T&gt;</c>.
    /// </summary>
    IQueryable,

    /// <summary>
    /// <c>IProducerConsumerCollection&lt;T&gt;</c>.
    /// </summary>
    IProducerConsumerCollection,

    /// <summary>
    /// <c>IImmutableList&lt;T&gt;</c>.
    /// </summary>
    IImmutableList,

    /// <summary>
    /// <c>IImmutableSet&lt;T&gt;</c>.
    /// </summary>
    IImmutableSet,

    /// <summary>
    /// <c>IImmutableDictionary&lt;TKey,TValue&gt;</c>.
    /// </summary>
    IImmutableDictionary,

    /// <summary>
    /// <c>IImmutableQueue&lt;T&gt;</c>.
    /// </summary>
    IImmutableQueue,

    /// <summary>
    /// <c>IImmutableStack&lt;T&gt;</c>.
    /// </summary>
    IImmutableStack,

    /// <summary>
    /// <c>IEnumerable&lt;KeyValuePair&lt;TKey,TValue&gt;&gt;</c> used as a dictionary-like source/target.
    /// </summary>
    IEnumerableOfKeyValuePair,
}