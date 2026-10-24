// <copyright file="MappaAllowInaccessibleSourceMembersAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Opt-in attribute that allows the Mappa source generator to read inaccessible
/// (private or protected) source properties and, when <see cref="MappaSettingsAttribute.MapSourceFields"/>
/// is enabled, inaccessible source fields when mapping.
/// <para>
/// When applied with no member names, every eligible inaccessible source property (and field, when
/// source fields are enabled) may be used. When applied with one or more member names, only those
/// listed properties or fields may be accessed via generated <c>UnsafeAccessor</c> methods
/// (flat names only; nested paths are not supported).
/// </para>
/// <para>
/// Set-only source properties are ignored. Requires a compilation that supports
/// <c>System.Runtime.CompilerServices.UnsafeAccessorAttribute</c> (typically .NET 8 or later);
/// otherwise the generator reports an error.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class MappaAllowInaccessibleSourceMembersAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAllowInaccessibleSourceMembersAttribute"/> class
    /// allowing all eligible inaccessible source properties (and fields when mapping source fields) to be read.
    /// </summary>
    public MappaAllowInaccessibleSourceMembersAttribute()
    {
        this.MemberNames = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAllowInaccessibleSourceMembersAttribute"/> class
    /// allowing only the listed inaccessible source properties or fields to be read.
    /// </summary>
    /// <param name="memberNames">
    /// The names of the source properties or fields that may be accessed when inaccessible.
    /// An empty or <c>null</c> list has the same meaning as the parameterless constructor.
    /// </param>
    public MappaAllowInaccessibleSourceMembersAttribute(params string[]? memberNames)
    {
        this.MemberNames = memberNames ?? [];
    }

    /// <summary>
    /// Gets the names of the source properties or fields that may be accessed when inaccessible.
    /// An empty array means all eligible inaccessible source properties (and fields when mapping
    /// source fields) may be used.
    /// </summary>
    public string[] MemberNames { get; }
}