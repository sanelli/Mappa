// <copyright file="IMappaBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

namespace Mappa.Generator.Builders;

/// <summary>
/// Describe a mappa builder.
/// </summary>
internal interface IMappaBuilder
{
    /// <summary>
    /// Generate the source code required by this builder.
    /// </summary>
    /// <returns>The source code.</returns>
    string BuildSource();
}