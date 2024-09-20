// <copyright file="InvokeConstructorStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Unit tests for the invoke-constructor strategy without empty constructor.
/// </summary>
[Mappa]
public sealed partial class InvokeConstructorStrategyMapper
{
    /// <summary>
    /// Map from <see cref="SourceRecordModel"/>
    /// to <see cref="TargetRecordModel"/> using the
    /// constructor strategy using the fields.
    /// </summary>
    /// <param name="sourceRecordModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetRecordModel Map(SourceRecordModel sourceRecordModel);
}