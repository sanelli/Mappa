// <copyright file="InaccessibleMembersMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type. Multiple sample mappers share this file by design.

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaAllowInaccessibleSourceMembersAttribute"/> and
/// <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> for all eligible inaccessible
/// members and a private target constructor.
/// </summary>
[Mappa]
public sealed partial class InaccessibleMembersMapper
{
    /// <summary>
    /// Map from <see cref="InaccessibleMembersSourceModel"/> to
    /// <see cref="InaccessibleMembersTargetModel"/> using all inaccessible members.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAllowInaccessibleSourceMembers]
    [MappaAllowInaccessibleTargetMembers]
    public partial InaccessibleMembersTargetModel Map(InaccessibleMembersSourceModel source);
}

/// <summary>
/// Mapper demonstrating named inaccessible properties together with a private target constructor.
/// </summary>
[Mappa]
public sealed partial class InaccessibleMembersNamedPropertiesAndConstructorMapper
{
    /// <summary>
    /// Map using an explicit inaccessible property whitelist and the private target constructor.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAllowInaccessibleSourceMembers("Name")]
    [MappaAllowInaccessibleTargetMembers("Name", "Age")]
    public partial InaccessibleMembersTargetModel Map(InaccessibleMembersSourceModel source);
}

/// <summary>
/// Mapper demonstrating inaccessible target constructor access only (public property setters).
/// </summary>
[Mappa]
public sealed partial class InaccessibleMembersConstructorOnlyMapper
{
    /// <summary>
    /// Map using only the private target constructor; properties use public setters.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAllowInaccessibleSourceMembers("Name")]
    [MappaAllowInaccessibleTargetMembers(AllowProperties = false)]
    public partial InaccessibleMembersPublicSettersTargetModel Map(InaccessibleMembersSourceModel source);
}

/// <summary>
/// Mapper demonstrating inaccessible target property access with an explicit whitelist
/// that excludes <c>Age</c>.
/// </summary>
[Mappa]
public sealed partial class InaccessibleMembersNamedPropertiesOnlyMapper
{
    /// <summary>
    /// Map only the whitelisted inaccessible target property <c>Name</c>
    /// ( <c>Age</c> is excluded and remains at its default).
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAllowInaccessibleSourceMembers("Name")]
    [MappaAllowInaccessibleTargetMembers("Name", AllowConstructors = false)]
    public partial InaccessibleMembersPublicCtorTargetModel Map(InaccessibleMembersSourceModel source);
}

/// <summary>
/// Mapper demonstrating a private target constructor with parameters.
/// </summary>
[Mappa]
public sealed partial class InaccessibleMembersParameterizedConstructorMapper
{
    /// <summary>
    /// Map using a private target constructor that accepts <c>name</c> and <c>age</c>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAllowInaccessibleTargetMembers(AllowProperties = false)]
    public partial InaccessibleMembersParameterizedCtorTargetModel Map(InaccessibleMembersParameterizedCtorSourceModel source);
}