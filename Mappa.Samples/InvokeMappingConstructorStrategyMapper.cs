// <copyright file="InvokeMappingConstructorStrategyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Unit tests for the invoke-mapping-constructor strategy.
/// </summary>
[Mappa]
public sealed partial class InvokeMappingConstructorStrategyMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassModel"/>
    /// to <see cref="TargetClassModelWithSingleMapperConstructorFromSourceClassModel"/>.
    /// </summary>
    /// <param name="sourceClassModel">The source model.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModelWithSingleMapperConstructorFromSourceClassModel MapToClassWithSingleMappingConstructor(SourceClassModel sourceClassModel);

    /// <summary>
    /// Map from <see cref="SourceClassModel"/>
    /// to <see cref="TargetClassModelWithMultipleMapperConstructors"/>.
    /// </summary>
    /// <param name="input">The source enum value.</param>
    /// <returns>The target model.</returns>
    public partial TargetClassModelWithMultipleMapperConstructors MapToClassWithMultipleMappingConstructors(CountingValues input);
}