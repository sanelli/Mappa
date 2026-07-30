// <copyright file="EnumModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1515, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Enums.Models;

/// <summary>
/// Source enum for enum-to-enum benchmarks.
/// </summary>
public enum SourceStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Active status.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inactive status.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Archived status.
    /// </summary>
    Archived = 3,
}

/// <summary>
/// Target enum for enum-to-enum benchmarks.
/// </summary>
public enum TargetStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Active status.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inactive status.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Archived status.
    /// </summary>
    Archived = 3,
}