// <copyright file="CustomDictionaryWithGeneric.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Custom dictionary implementation with generics.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class CustomDictionaryWithGeneric<TKey, TValue> : Dictionary<TKey, TValue>
    where TKey : notnull;