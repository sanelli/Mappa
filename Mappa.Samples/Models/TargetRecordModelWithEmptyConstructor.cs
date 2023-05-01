// <copyright file="TargetRecordModelWithEmptyConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A source record with a few parameters
/// and an empty constructor.
/// </summary>
/// <param name="ParamA">An integer property.</param>
/// <param name="ParamB">An enumeration property.</param>
public sealed record TargetRecordModelWithEmptyConstructor(string ParamA, int ParamB)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetRecordModelWithEmptyConstructor"/> class.
    /// </summary>
    public TargetRecordModelWithEmptyConstructor()
        : this(string.Empty, -1)
    {
    }
}