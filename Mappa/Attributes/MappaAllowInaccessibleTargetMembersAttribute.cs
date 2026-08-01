// <copyright file="MappaAllowInaccessibleTargetMembersAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Opt-in attribute that allows the Mappa source generator to write inaccessible
/// (private or protected) target properties and/or invoke inaccessible target constructors
/// when mapping.
/// <para>
/// When applied with no member names, every eligible inaccessible target property may be written
/// (subject to <see cref="AllowProperties"/>). When applied with one or more member names, only
/// those listed properties may be accessed via generated <c>UnsafeAccessor</c> methods
/// (flat names only; nested paths are not supported). Constructor access is controlled by
/// <see cref="AllowConstructors"/> and is independent of the member-name list.
/// </para>
/// <para>
/// Get-only non-collection target properties remain unmappable. Get-only collection, dictionary,
/// queue, and stack properties continue to use the existing post-construction fill rules when
/// their getters can be accessed. Requires a compilation that supports
/// <c>System.Runtime.CompilerServices.UnsafeAccessorAttribute</c> (typically .NET 8 or later);
/// otherwise the generator reports an error. Setting both <see cref="AllowProperties"/> and
/// <see cref="AllowConstructors"/> to <c>false</c> is an error.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class MappaAllowInaccessibleTargetMembersAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> class
    /// allowing all eligible inaccessible target properties and constructors to be used.
    /// </summary>
    public MappaAllowInaccessibleTargetMembersAttribute()
    {
        this.MemberNames = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> class
    /// allowing only the listed inaccessible target properties to be written
    /// (when <see cref="AllowProperties"/> is <c>true</c>).
    /// </summary>
    /// <param name="memberNames">
    /// The names of the target properties that may be accessed when inaccessible.
    /// An empty or <c>null</c> list has the same meaning as the parameterless constructor.
    /// </param>
    public MappaAllowInaccessibleTargetMembersAttribute(params string[]? memberNames)
    {
        this.MemberNames = memberNames ?? [];
    }

    /// <summary>
    /// Gets the names of the target properties that may be accessed when inaccessible.
    /// An empty array means all eligible inaccessible target properties may be used
    /// (when <see cref="AllowProperties"/> is <c>true</c>).
    /// </summary>
    public string[] MemberNames { get; }

    /// <summary>
    /// Gets or sets a value indicating whether inaccessible target properties may be written.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool AllowProperties { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether inaccessible target constructors may be invoked.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool AllowConstructors { get; set; } = true;
}