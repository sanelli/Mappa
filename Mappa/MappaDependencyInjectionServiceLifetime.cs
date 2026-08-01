// <copyright file="MappaDependencyInjectionServiceLifetime.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Describes the service lifetime used when registering mapper types
/// discovered by <see cref="MappaDependencyInjectionAttribute"/>.
/// </summary>
public enum MappaDependencyInjectionServiceLifetime
{
    /// <summary>
    /// Register mappers as singletons (for example <c>AddSingleton</c>).
    /// This is the default behaviour.
    /// </summary>
    Singleton,

    /// <summary>
    /// Register mappers with scoped lifetime (for example <c>AddScoped</c>).
    /// </summary>
    Scoped,

    /// <summary>
    /// Register mappers with transient lifetime (for example <c>AddTransient</c>).
    /// </summary>
    Transient,
}