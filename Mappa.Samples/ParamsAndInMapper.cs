// <copyright file="ParamsAndInMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper to test usage of <c>in</c> and <c>params</c>.
/// </summary>
[Mappa]
public sealed partial class ParamsAndInMapper
{
    /// <summary>
    /// Map fom <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModel MapWithIn(in SourceClassModel source);

    /// <summary>
    /// Map from <see cref="Array"/> of <see cref="SourceClassModel"/>
    /// to <see cref="Array"/> of <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModel[] MapWithParams(params SourceClassModel[] source);

    /// <summary>
    /// Map fom <see cref="SourceClassModel"/> to <see cref="TargetClassModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <param name="context">The context.</param>
    /// <returns>The target model.</returns>
    [MappaAssignFromContext(nameof(TargetRecordModel.ParamB), "paramB")]
    public partial TargetRecordModel MapWithInOnContext(SourceRecordModel source, in MappaContext context);
}