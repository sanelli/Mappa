// <copyright file="MappaContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Describe a context that can be forwarded
/// across mapper methods.
/// </summary>
public sealed class MappaContext
{
    private readonly Dictionary<string, object> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaContext"/> class.
    /// </summary>
    /// <param name="items">The items that can be store in this context.</param>
    public MappaContext(IDictionary<string, object> items)
    {
        this.items = new Dictionary<string, object>(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaContext"/> class.
    /// </summary>
    /// <param name="items">The items that can be store in this context.</param>
    public MappaContext(KeyValuePair<string, object>[] items)
    {
        this.items = items.ToDictionary(item => item.Key, item => item.Value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaContext"/> class.
    /// </summary>
    public MappaContext()
    {
        this.items = new Dictionary<string, object>();
    }

    /// <summary>
    /// Get the value for <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to be accessed.</param>
    public object this[string key]
    {
        get => this.items[key];
        set => this.items[key] = value;
    }

    /// <summary>
    /// Build a <see cref="MappaContext"/> from the input <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="items">The list of items to store in the context.</param>
    /// <returns>A new <see cref="MappaContext"/> containing the specified items.</returns>
    public static implicit operator MappaContext(Dictionary<string, object> items) => ToMappaContext(items);

    /// <summary>
    /// Build a <see cref="MappaContext"/> from the input <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="items">The list of items to store in the context.</param>
    /// <returns>A new <see cref="MappaContext"/> containing the specified items.</returns>
    public static implicit operator MappaContext(KeyValuePair<string, object>[] items) => ToMappaContext(items);

    /// <summary>
    /// Build a <see cref="MappaContext"/> from the input <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="items">The list of items to store in the context.</param>
    /// <returns>A new <see cref="MappaContext"/> containing the specified items.</returns>
    public static MappaContext ToMappaContext(IDictionary<string, object> items)
    {
        return new(items);
    }

    /// <summary>
    /// Build a <see cref="MappaContext"/> from the input <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="items">The list of items to store in the context.</param>
    /// <returns>A new <see cref="MappaContext"/> containing the specified items.</returns>
    public static MappaContext ToMappaContext(KeyValuePair<string, object>[] items)
    {
        return new(items);
    }

    /// <summary>
    /// Add an item to the context.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value of the item.</param>
    public void Add(string key, object value) => this.items.Add(key, value);
}