// <copyright file="PragmaWarning.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Describe the <c>#pragma warning</c> setup.
/// </summary>
internal enum PragmaWarning
{
    /// <summary>
    /// Unknown <c>#pragma warning</c>.
    /// </summary>
    None,

    /// <summary>
    /// The <c>#pragma warning</c> is not added to the code.
    /// </summary>
    NoBlock,

    /// <summary>
    /// The code requires <c>#pragma warning disable</c>.
    /// </summary>
    Disable,
}