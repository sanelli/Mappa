// <copyright file="InvokeEmptyConstructorStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Unit tests for the invoke-empty-constructor strategy.
/// </summary>
[Mappa]
public sealed partial class InvokeEmptyConstructorStrategyMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/>
    /// to <see cref="TargetClassModel"/> using the
    /// empty constructor strategy.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModel Map(SourceClassModel sourceClassModel);

    /// <summary>
    /// Map from <see cref="SourceRecordModelWithEmptyConstructor"/>
    /// to <see cref="TargetRecordModelWithEmptyConstructor"/> using the
    /// empty constructor strategy.
    /// </summary>
    /// <param name="sourceRecordModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetRecordModelWithEmptyConstructor Map(SourceRecordModelWithEmptyConstructor sourceRecordModel);

    /// <summary>
    /// Map from <see cref="SourceClassModel"/> to <see cref="TargetClassModelWithOnePrivateSetterProperty"/>
    /// using the constructor empty strategy and ignoring properties with private setter.
    /// </summary>
    /// <returns>The target model.</returns>
    public partial TargetClassModelWithOnePrivateSetterProperty MapWithPrivateSetter(SourceClassModel sourceClassModel);
}