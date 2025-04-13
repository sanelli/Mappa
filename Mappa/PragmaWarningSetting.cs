// <copyright file="PragmaWarningSetting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Define if the methods should be surrounded by
/// <c>#pragma warning</c> blocks.
/// </summary>
public enum PragmaWarningSetting
{
    /// <summary>
    /// Undefined, use the default or what has been set on the parent.
    /// </summary>
    Undefined,

    /// <summary>
    /// Do not use a <c>#pragma warning</c>.
    /// </summary>
    NoBlock,

    /// <summary>
    /// Surround the method <c>#pragma warning disable</c>.
    /// </summary>
    Disable,
}