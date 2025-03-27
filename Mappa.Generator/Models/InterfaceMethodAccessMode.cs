// <copyright file="InterfaceMethodAccessMode.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describe how a method from interface can be accessed.
/// </summary>
public enum InterfaceMethodAccessMode
{
    /// <summary>
    /// The method cannot be accessed.
    /// </summary>
    None,

    /// <summary>
    /// The method can be accessed directly.
    /// </summary>
    Direct,

    /// <summary>
    /// The method can be accessed via interface only.
    /// </summary>
    InterfaceExplicit,
}