// <copyright file="TargetThirdClass.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism.One;

/// <summary>
/// A base class.
/// </summary>
public class TargetThirdClass : TargetSecondClass
{
    /// <summary>
    /// Gets or sets an array of numbers mapped as string.
    /// </summary>
#pragma warning disable CA1819
    public required long[] Numbers { get; set; }
#pragma warning restore CA1819
}