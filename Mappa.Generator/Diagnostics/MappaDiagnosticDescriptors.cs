// <copyright file="MappaDiagnosticDescriptors.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Diagnostics;

/// <summary>
/// Mappa diagnostic descriptors.
/// </summary>
internal static class MappaDiagnosticDescriptors
{
    private const string Title = "Mappa";
    private const string Category = "Mappa.Generator";

    private static DiagnosticDescriptor? methodHasInvalidNumberOfParameters;
    private static DiagnosticDescriptor? methodIsVoid;
    private static DiagnosticDescriptor? methodReturnsTaskType;
    private static DiagnosticDescriptor? duplicateMapping;
    private static DiagnosticDescriptor? cannotIdentifyStrategy;

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
        => methodIsVoid ??= BuildError(
            MappaDiagnosticsKind.MethodIsVoid,
            "Method '{0}' cannot be used for mapping because it returns void.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodReturnsTaskType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodReturnsTaskType
        => methodReturnsTaskType ??= BuildError(
            MappaDiagnosticsKind.MethodReturnsTaskType,
            "Method '{0}' cannot be used for mapping because it returns either Task, Task<T>, ValueTask or ValueTask<T>.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicatedMapping"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicatedMapping
        => duplicateMapping ??= BuildError(
            MappaDiagnosticsKind.DuplicatedMapping,
            "Method '{0}' cannot be used for mapping from '{1}' to '{2}' already exists in the current class.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotIdentifyStrategy"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotIdentifyStrategy
        => cannotIdentifyStrategy ??= BuildError(
            MappaDiagnosticsKind.CannotIdentifyStrategy,
            "Cannot identify a mapping strategy between from '{0}' (type: '{1}') to '{2}' (type: '{3}').");

    private static DiagnosticDescriptor BuildError(MappaDiagnosticsKind kind, string message)
        => new DiagnosticDescriptor(
            kind.ToDiagnosticId(),
            Title,
            message,
            Category,
            DiagnosticSeverity.Error,
            true);
}