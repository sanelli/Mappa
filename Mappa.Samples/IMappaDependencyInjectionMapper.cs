// <copyright file="IMappaDependencyInjectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples;

/// <summary>
/// Contract for <see cref="MappaDependencyInjectionMapper"/> used by the
/// <see cref="MappaDependencyInjectionRegistrar"/> sample.
/// </summary>
public interface IMappaDependencyInjectionMapper
{
    /// <summary>
    /// Map an <see cref="int"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The mapped string.</returns>
    string Map(int input);
}