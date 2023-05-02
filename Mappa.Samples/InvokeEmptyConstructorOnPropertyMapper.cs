// <copyright file="InvokeEmptyConstructorOnPropertyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Unit tests for the invoke-empty-constructor strategy.
/// </summary>
[Mappa]
public sealed partial class InvokeEmptyConstructorOnPropertyMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassWithInnerClassModel"/>
    /// to <see cref="TargetClassWithInnerClassModel"/> using the
    /// empty constructor strategy.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassWithInnerClassModel Map(SourceClassWithInnerClassModel sourceClassModel);
}