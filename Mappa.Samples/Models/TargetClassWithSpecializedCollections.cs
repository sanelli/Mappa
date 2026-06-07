// <copyright file="TargetClassWithSpecializedCollections.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

namespace Mappa.Samples.Models;

/// <summary>
/// Target class for <see cref="SourceClassWithSpecializedCollections"/>.
/// </summary>
public sealed class TargetClassWithSpecializedCollections
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetClassWithSpecializedCollections"/> class.
    /// </summary>
    public TargetClassWithSpecializedCollections()
    {
        this.PropertyA = new Stack<string>();
        this.PropertyB = new Queue<string>();
        this.PropertyC = new ConcurrentStack<string>();
        this.PropertyD = new ConcurrentQueue<string>();
        this.PropertyE = new ConcurrentBag<string>();
        this.PropertyF = new BlockingCollection<string>();
    }

    /// <summary>
    /// Gets a stack property.
    /// </summary>
    public Stack<string> PropertyA { get; }

    /// <summary>
    /// Gets a queue property.
    /// </summary>
    public Queue<string> PropertyB { get; }

    /// <summary>
    /// Gets a concurrent stack property.
    /// </summary>
    public ConcurrentStack<string> PropertyC { get; }

    /// <summary>
    /// Gets a concurrent queue property.
    /// </summary>
    public ConcurrentQueue<string> PropertyD { get; }

    /// <summary>
    /// Gets a concurrent bag property.
    /// </summary>
    public ConcurrentBag<string> PropertyE { get; }

    /// <summary>
    /// Gets a blocking collection property.
    /// </summary>
    public BlockingCollection<string> PropertyF { get; }
}