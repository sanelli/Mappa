// <copyright file="DictionaryAssignmentSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Defines how entries are inserted when mapping between dictionaries.
/// </summary>
public enum DictionaryAssignmentSetting
{
    /// <summary>
    /// Ignore the setting from the application of this attribute
    /// and use the value from a parent scope or global configuration.
    /// </summary>
    Undefined,

    /// <summary>
    /// Insert mapped entries using the dictionary indexer (<c>target[key] = value</c>).
    /// </summary>
    Indexer,

    /// <summary>
    /// Insert mapped entries using <see cref="System.Collections.Generic.IDictionary{TKey,TValue}.Add(TKey, TValue)"/>.
    /// </summary>
    Add,
}