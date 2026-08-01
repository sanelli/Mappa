// <copyright file="InaccessibleTargetMemberOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Generator.Models;

/// <summary>
/// Parsed options for allowing inaccessible target members on a map method.
/// </summary>
/// <param name="memberNames">The whitelisted member names; empty means all eligible members.</param>
/// <param name="allowProperties">Whether inaccessible properties may be written.</param>
/// <param name="allowConstructors">Whether inaccessible constructors may be invoked.</param>
internal sealed class InaccessibleTargetMemberOptions(
    string[] memberNames,
    bool allowProperties,
    bool allowConstructors)
{
    /// <summary>
    /// Gets the whitelisted target member names. An empty array means all eligible members.
    /// </summary>
    internal string[] MemberNames { get; } = memberNames;

    /// <summary>
    /// Gets a value indicating whether inaccessible target properties may be written.
    /// </summary>
    internal bool AllowProperties { get; } = allowProperties;

    /// <summary>
    /// Gets a value indicating whether inaccessible target constructors may be invoked.
    /// </summary>
    internal bool AllowConstructors { get; } = allowConstructors;

    /// <summary>
    /// Gets a value indicating whether every eligible inaccessible target property is allowed.
    /// </summary>
    internal bool AllowAllProperties => this.MemberNames.Length == 0;

    /// <summary>
    /// Creates options from the attribute, or <c>null</c> when the attribute is absent.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <returns>The options, or <c>null</c>.</returns>
    internal static InaccessibleTargetMemberOptions? FromAttribute(MappaAllowInaccessibleTargetMembersAttribute? attribute)
        => attribute is null
            ? null
            : new InaccessibleTargetMemberOptions(
                attribute.MemberNames,
                attribute.AllowProperties,
                attribute.AllowConstructors);

    /// <summary>
    /// Checks whether <paramref name="memberName"/> is allowed by this options instance for properties.
    /// </summary>
    /// <param name="memberName">The member name.</param>
    /// <returns><c>true</c> when allowed; otherwise <c>false</c>.</returns>
    internal bool IsPropertyAllowed(string memberName)
        => this.AllowProperties
           && (this.AllowAllProperties
               || this.MemberNames.Any(name => name.Equals(memberName, StringComparison.Ordinal)));
}