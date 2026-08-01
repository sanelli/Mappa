// <copyright file="InaccessibleSourceMemberOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Generator.Models;

/// <summary>
/// Parsed options for allowing inaccessible source members on a map method.
/// </summary>
/// <param name="memberNames">The whitelisted member names; empty means all eligible members.</param>
internal sealed class InaccessibleSourceMemberOptions(string[] memberNames)
{
    /// <summary>
    /// Gets the whitelisted source member names. An empty array means all eligible members.
    /// </summary>
    internal string[] MemberNames { get; } = memberNames;

    /// <summary>
    /// Gets a value indicating whether every eligible inaccessible source member is allowed.
    /// </summary>
    internal bool AllowAll => this.MemberNames.Length == 0;

    /// <summary>
    /// Creates options from the attribute, or <c>null</c> when the attribute is absent.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <returns>The options, or <c>null</c>.</returns>
    internal static InaccessibleSourceMemberOptions? FromAttribute(MappaAllowInaccessibleSourceMembersAttribute? attribute)
        => attribute is null ? null : new InaccessibleSourceMemberOptions(attribute.MemberNames);

    /// <summary>
    /// Checks whether <paramref name="memberName"/> is allowed by this options instance.
    /// </summary>
    /// <param name="memberName">The member name.</param>
    /// <returns><c>true</c> when allowed; otherwise <c>false</c>.</returns>
    internal bool IsMemberAllowed(string memberName)
        => this.AllowAll
           || this.MemberNames.Any(name => name.Equals(memberName, StringComparison.Ordinal));
}