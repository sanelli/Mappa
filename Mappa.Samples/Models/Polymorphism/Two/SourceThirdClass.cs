// <copyright file="SourceThirdClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.Two;

/// <summary>
/// A base class.
/// </summary>
public class SourceThirdClass : SourceSecondClass
{
    /// <summary>
    /// Gets or sets an array of numbers mapped as string.
    /// </summary>
#pragma warning disable CA1819
    public required string[] Numbers { get; set; }
#pragma warning restore CA1819
}