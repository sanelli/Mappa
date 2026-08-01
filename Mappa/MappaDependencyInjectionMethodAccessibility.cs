// <copyright file="MappaDependencyInjectionMethodAccessibility.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Describes the accessibility of the registration method generated for
/// <see cref="MappaDependencyInjectionAttribute"/>.
/// </summary>
public enum MappaDependencyInjectionMethodAccessibility
{
    /// <summary>
    /// Generate a <c>public</c> method. This is the default behaviour.
    /// </summary>
    Public,

    /// <summary>
    /// Generate a <c>private</c> method.
    /// </summary>
    Private,

    /// <summary>
    /// Generate a <c>protected</c> method.
    /// </summary>
    Protected,

    /// <summary>
    /// Generate an <c>internal</c> method.
    /// </summary>
    Internal,

    /// <summary>
    /// Generate a <c>protected internal</c> method.
    /// </summary>
    ProtectedInternal,
}