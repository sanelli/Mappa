// <copyright file="MappaInterceptorExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Fluent helpers for configuring interceptor-based <c>MappaMap</c> call sites at compile time.
/// </summary>
public static class MappaInterceptorExtensions
{
    /// <summary>
    /// Identity helper used after <c>MappaMap</c> to supply mapping settings, static dependencies,
    /// and method attributes to the Mappa source generator.
    /// <para>
    /// Arguments are inspected only at generation time and are not used at runtime. Prefer
    /// <c>new</c> object-creation expressions (and arrays/collections of <c>new</c>); other
    /// argument forms are rejected with a diagnostic. Multiple contiguous <c>With</c> calls are
    /// collated in source order.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The mapped value type (typically the <c>MappaMap</c> target).</typeparam>
    /// <param name="input">The mapped value.</param>
    /// <param name="mappaSettings">
    /// Optional settings instance; merged after editorconfig and class-level
    /// <see cref="MappaSettingsAttribute"/> when multiple <c>With</c> settings appear.
    /// </param>
    /// <param name="mappaStaticDependencies">
    /// Optional static dependency declarations merged with class-level
    /// <see cref="MappaStaticDependencyAttribute"/> values.
    /// </param>
    /// <param name="mappaMethodAttributes">
    /// Optional method-level mapper attributes implementing <see cref="IMappaMethodAttribute"/>.
    /// </param>
    /// <returns><paramref name="input"/> unchanged.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T With<T>(
        this T input,
        MappaSettingsAttribute? mappaSettings = null,
        MappaStaticDependencyAttribute[]? mappaStaticDependencies = null,
        IMappaMethodAttribute[]? mappaMethodAttributes = null)
    {
        _ = mappaSettings;
        _ = mappaStaticDependencies;
        _ = mappaMethodAttributes;
        return input;
    }
}