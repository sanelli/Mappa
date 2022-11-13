// <copyright file="MappaDiagnosticsKindExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

namespace Mappa.Generator.Diagnostics;

/// <summary>
/// Extension methods for <see cref="MappaDiagnosticsKind"/>.
/// </summary>
internal static class MappaDiagnosticsKindExtensions
{
    /// <summary>
    /// Returns a string representing the diagnostic ID.
    /// </summary>
    /// <param name="mappaDiagnosticsKind">The mappa diagnostic kind.</param>
    /// <returns>A string representing the diagnostic ID.</returns>
    internal static string ToDiagnosticId(this MappaDiagnosticsKind mappaDiagnosticsKind)
        => $"MP{(int)mappaDiagnosticsKind:00000}";
}