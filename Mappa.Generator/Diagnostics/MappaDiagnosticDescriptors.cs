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
    private static DiagnosticDescriptor? methodHasInvalidMappaContextParameter;
    private static DiagnosticDescriptor? methodIsVoid;
    private static DiagnosticDescriptor? methodReturnsTaskType;
    private static DiagnosticDescriptor? duplicateMapping;
    private static DiagnosticDescriptor? cannotIdentifyStrategy;
    private static DiagnosticDescriptor? multipleAttributesTargetTheSamePropertyOrParameter;
    private static DiagnosticDescriptor? cannotDetectSuitableMethodToInvoke;
    private static DiagnosticDescriptor? cannotDetectType;
    private static DiagnosticDescriptor? cannotFindFieldOrProperty;
    private static DiagnosticDescriptor? cannotUseMappaAssignFromContextAttributeWithoutContextParameter;
    private static DiagnosticDescriptor? userDefinedCultureIsMissingCultureName;

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodHasInvalidNumberOfParameters"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodHasInvalidNumberOfParameters
        => methodHasInvalidNumberOfParameters ??= BuildError(
            MappaDiagnosticsKind.MethodHasInvalidNumberOfParameters,
            "Method '{0}' cannot be used for mapping because it has an unsupported number of parameters.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodHasInvalidMappaContextParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodHasInvalidMappaContextParameter
        => methodHasInvalidMappaContextParameter ??= BuildError(
            MappaDiagnosticsKind.MethodHasInvalidMappaContextParameter,
            "Method '{0}' cannot be used for mapping because the second parameter is not of type MappaContext.");

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
            "Method '{0}' cannot be generated because mapping from '{1}' to '{2}' already exists in the current class.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotIdentifyStrategy"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotIdentifyStrategy
        => cannotIdentifyStrategy ??= BuildError(
            MappaDiagnosticsKind.CannotIdentifyStrategy,
            "Cannot identify a mapping strategy from type '{0}' to type: '{1}'.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MultipleAttributesTargetTheSamePropertyOrParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor MultipleAttributesTargetTheSamePropertyOrParameter
        => multipleAttributesTargetTheSamePropertyOrParameter ??= BuildError(
            MappaDiagnosticsKind.MultipleAttributesTargetTheSamePropertyOrParameter,
            "Multiple mapping attributes on method '{0}' target property or constructor parameter '{1}'.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotDetectSuitableMethodToInvoke"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotDetectSuitableMethodToInvoke
        => cannotDetectSuitableMethodToInvoke ??= BuildError(
            MappaDiagnosticsKind.CannotDetectSuitableMethodToInvoke,
            "Cannot identify a method with name '{0}' in class '{1}' for target property or constructor parameter '{2}'.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotDetectType"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotDetectType
        => cannotDetectType ??= BuildError(
            MappaDiagnosticsKind.CannotDetectType,
            "Cannot identify type '{0}'.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotFindFieldOrProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotFindFieldOrProperty
        => cannotFindFieldOrProperty ??= BuildError(
            MappaDiagnosticsKind.CannotFindFieldOrProperty,
            "Cannot identify field or property '{0}'.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotUseMappaAssignFromContextAttributeWithoutContextParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotUseMappaAssignFromContextAttributeWithoutContextParameter
        => cannotUseMappaAssignFromContextAttributeWithoutContextParameter ??= BuildError(
            MappaDiagnosticsKind.CannotUseMappaAssignFromContextAttributeWithoutContextParameter,
            "Cannot use attribute MappaAssignFromContextAttribute for field, property or parameter '{0}': the method does not provide a MappaContext parameter.");

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.UserDefinedCultureIsMissingCultureName"/>.
    /// </summary>
    internal static DiagnosticDescriptor UserDefinedCultureIsMissingCultureName
        => userDefinedCultureIsMissingCultureName ??= BuildWarning(
            MappaDiagnosticsKind.UserDefinedCultureIsMissingCultureName,
            "The user defined culture does not define a culture name while mapping method '{0}': no culture will be used.");

    private static DiagnosticDescriptor BuildError(MappaDiagnosticsKind kind, string message)
        => new(
            kind.ToDiagnosticId(),
            Title,
            message,
            Category,
            DiagnosticSeverity.Error,
            true);

    private static DiagnosticDescriptor BuildWarning(MappaDiagnosticsKind kind, string message)
        => new(
            kind.ToDiagnosticId(),
            Title,
            message,
            Category,
            DiagnosticSeverity.Error,
            true);
}