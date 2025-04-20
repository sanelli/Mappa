// <copyright file="CustomConcurrentDictionary.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

namespace Mappa.Samples.Models;

/// <summary>
/// Custom implementation of <see cref="ConcurrentDictionary{TKey,TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class CustomConcurrentDictionary<TKey, TValue>
    : ConcurrentDictionary<TKey, TValue>
    where TKey : notnull;