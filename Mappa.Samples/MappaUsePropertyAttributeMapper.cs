// <copyright file="MappaUsePropertyAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper used to demonstrate <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaUsePropertyAttributeMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>
    /// using multiple <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaUseProperty(nameof(TargetClassModel.ParamA), nameof(SourceClassModel.ParamB))]
    [MappaUseProperty(nameof(TargetClassModel.ParamB), nameof(SourceClassModel.ParamA))]
    public partial TargetClassModel MapWithEmptyConstructor(SourceClassModel source);

    /// <summary>
    /// Map from <see cref="SourceRecordModel"/> to <see cref="TargetRecordModel"/>
    /// using multiple <see cref="MappaUsePropertyAttribute"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaUseProperty(nameof(TargetRecordModel.ParamA), nameof(SourceRecordModel.ParamB))]
    [MappaUseProperty(nameof(TargetRecordModel.ParamB), nameof(SourceRecordModel.ParamA))]
    public partial TargetRecordModel MapWithConstructorWithParameters(SourceRecordModel source);
}