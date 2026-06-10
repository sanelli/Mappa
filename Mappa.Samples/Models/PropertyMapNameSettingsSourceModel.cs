// <copyright file="PropertyMapNameSettingsSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Mappa.Samples.Models;

/// <summary>
/// Source model for <see cref="PropertyMapNameSettingsMapper"/> samples.
/// </summary>
public sealed class PropertyMapNameSettingsSourceModel
{
    /// <summary>
    /// Gets or sets a value whose name differs from the target by casing and underscores.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Sample property name for property map name settings.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Sample property name for property map name settings.")]
    public int user_name { get; set; } = 42;

    /// <summary>
    /// Gets or sets a value whose name matches the target exactly.
    /// </summary>
    public int PropertyB { get; set; } = 7;
}