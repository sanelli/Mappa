// <copyright file="IMappaMethodAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Marker interface for Mappa attributes that can be applied to mapping methods
/// and passed to <see cref="MappaInterceptorExtensions.With{T}"/> for interceptor-based maps.
/// <para>
/// <see cref="Attributes.MappaSettingsAttribute"/> does not implement this interface; settings
/// are supplied via the dedicated <c>mappaSettings</c> parameter of
/// <see cref="MappaInterceptorExtensions.With{T}"/>.
/// </para>
/// </summary>
#pragma warning disable CA1040
#pragma warning disable CA1711
public interface IMappaMethodAttribute
#pragma warning restore CA1711
#pragma warning restore CA1040
{
}