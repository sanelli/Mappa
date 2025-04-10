// <copyright file="NullableSetup.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Describe the <c>#nullable</c> setup.
/// </summary>
internal enum NullableSetup
{
    /// <summary>
    /// Unknown <c>#nullable</c> setup.
    /// </summary>
    None,

    /// <summary>
    /// Require <c>#nullable enable</c>.
    /// </summary>
    Enable,

    /// <summary>
    /// Require <c>#nullable disable</c>.
    /// </summary>
    Disable,
}