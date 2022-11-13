// <copyright file="MappaDiagnosticDescriptors.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Diagnostics;

/// <summary>
/// Mappa diagnostic descriptors.
/// </summary>
internal static class MappaDiagnosticDescriptors
{
    private const string Title = "Mappa";
    private const string Category = "Mappa.Generator";

    private static DiagnosticDescriptor? methodHasInvalidNumberOfParameters = null;

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodHasInvalidNumberOfParameters"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodHasInvalidNumberOfParameters
        => methodHasInvalidNumberOfParameters ??= BuildError(
            MappaDiagnosticsKind.MethodHasInvalidNumberOfParameters,
            "Method '{0}' cannot be used for mapping because it has an unsupported number of parameters.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodIsVoid"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodIsVoid
        => methodHasInvalidNumberOfParameters ??= BuildError(
            MappaDiagnosticsKind.MethodIsVoid,
            "Method '{0}' cannot be used for mapping because it returns void.");

    private static DiagnosticDescriptor BuildError(MappaDiagnosticsKind kind, string message)
        => new DiagnosticDescriptor(
            kind.ToDiagnosticId(),
            Title,
            message,
            Category,
            DiagnosticSeverity.Error,
            true);
}