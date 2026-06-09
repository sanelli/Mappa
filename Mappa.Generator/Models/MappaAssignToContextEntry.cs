// <copyright file="MappaAssignToContextEntry.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a value to assign to a <see cref="MappaContext"/> entry after target construction.
/// </summary>
internal sealed class MappaAssignToContextEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignToContextEntry"/> class.
    /// </summary>
    /// <param name="contextKey">The context dictionary key.</param>
    /// <param name="memberName">The target property or field name to read the value from.</param>
    internal MappaAssignToContextEntry(string contextKey, string memberName)
    {
        this.ContextKey = contextKey;
        this.MemberName = memberName;
    }

    /// <summary>
    /// Gets the context dictionary key.
    /// </summary>
    internal string ContextKey { get; }

    /// <summary>
    /// Gets the target property or field name to read the value from.
    /// </summary>
    internal string MemberName { get; }
}