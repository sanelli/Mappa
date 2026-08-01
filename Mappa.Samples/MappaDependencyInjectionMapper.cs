// <copyright file="MappaDependencyInjectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Sample mapper registered via <see cref="MappaDependencyInjectionRegistrar"/>.
/// </summary>
[Mappa]
public sealed partial class MappaDependencyInjectionMapper
    : IMappaDependencyInjectionMapper
{
    /// <inheritdoc cref="IMappaDependencyInjectionMapper.Map"/>
    public partial string Map(int input);
}