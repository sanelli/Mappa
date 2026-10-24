// <copyright file="MappaNullSubstituteMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaNullSubstituteAttribute"/> AssignValue and InvokeMethod modes.
/// </summary>
[Mappa]
public sealed partial class MappaNullSubstituteMapper
{
    /// <summary>
    /// Map using a constant substitute when <see cref="NullSubstituteSourceModel.Name"/> is null.
    /// </summary>
    /// <param name="input">The source model.</param>
    /// <returns>The mapped model.</returns>
    [MappaNullSubstitute(nameof(NullSubstituteTargetModel.Name), AssignValue = "Anonymous")]
    [MappaNullSubstitute(nameof(NullSubstituteTargetModel.Alias), InvokeMethod = nameof(WhenAliasNull))]
    public partial NullSubstituteTargetModel Map(NullSubstituteSourceModel input);

    [MappaIgnore]
    private static string WhenAliasNull(string? value) => value ?? "NoAlias";
}