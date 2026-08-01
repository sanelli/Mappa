// <copyright file="InaccessibleMembersMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaAllowInaccessibleSourceMembersAttribute"/> and
/// <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> for private members
/// and a private target constructor.
/// </summary>
[Mappa]
public sealed partial class InaccessibleMembersMapper
{
    /// <summary>
    /// Map from <see cref="InaccessibleMembersSourceModel"/> to
    /// <see cref="InaccessibleMembersTargetModel"/> using inaccessible members.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaAllowInaccessibleSourceMembers]
    [MappaAllowInaccessibleTargetMembers]
    public partial InaccessibleMembersTargetModel Map(InaccessibleMembersSourceModel source);
}