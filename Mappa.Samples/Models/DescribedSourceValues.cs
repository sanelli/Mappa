// <copyright file="DescribedSourceValues.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Mappa.Samples.Models;

/// <summary>
/// Source enum for Description-based enum-to-enum mapping samples.
/// </summary>
public enum DescribedSourceValues
{
    /// <summary>
    /// Alpha value.
    /// </summary>
    [Description("Alpha")]
    Alpha,

    /// <summary>
    /// Beta value.
    /// </summary>
    [Description("Beta")]
    Beta,
}