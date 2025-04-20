// <copyright file="CustomBlockingCollection.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

namespace Mappa.Samples.Models;

#pragma warning disable SA1402

/// <summary>
/// Custom blocking collection of <typeparamref name="T"/>s.
/// </summary>
/// <typeparam name="T">The type of the collection.</typeparam>
public sealed class CustomBlockingCollection<T> : BlockingCollection<T>;

/// <summary>
/// Custom blocking collection <see cref="int"/>.
/// </summary>
public sealed class CustomBlockingCollection : BlockingCollection<int>;