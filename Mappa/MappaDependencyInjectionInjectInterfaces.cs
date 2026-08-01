// <copyright file="MappaDependencyInjectionInjectInterfaces.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa;

/// <summary>
/// Describes how mapper types discovered by <see cref="MappaDependencyInjectionAttribute"/>
/// are registered in the dependency injection container.
/// </summary>
public enum MappaDependencyInjectionInjectInterfaces
{
    /// <summary>
    /// Register only the mapper class itself (for example <c>AddSingleton&lt;Mapper&gt;()</c>).
    /// This is the default behaviour.
    /// </summary>
    ClassOnly,

    /// <summary>
    /// Register each eligible interface implemented by the mapper
    /// (for example <c>AddSingleton&lt;IMapper, Mapper&gt;()</c>), without registering the concrete class.
    /// </summary>
    InterfaceOnly,

    /// <summary>
    /// Register both the concrete mapper class and each eligible interface it implements.
    /// </summary>
    InterfaceAndClass,
}