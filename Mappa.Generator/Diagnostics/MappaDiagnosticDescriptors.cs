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
    private static DiagnosticDescriptor? parseExactDoesNotAcceptOnlyFormat;
    private static DiagnosticDescriptor? propertySetterIsNotAccessible;
    private static DiagnosticDescriptor? tooManyUsePropertyAttributesForTheSameTargetProperty;

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodHasInvalidNumberOfParameters"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodHasInvalidNumberOfParameters
        => methodHasInvalidNumberOfParameters ??= BuildError(
            MappaDiagnosticsKind.MethodHasInvalidNumberOfParameters,
            DiagnosticsResources.MethodHasInvalidNumberOfParameters);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodHasInvalidMappaContextParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodHasInvalidMappaContextParameter
        => methodHasInvalidMappaContextParameter ??= BuildError(
            MappaDiagnosticsKind.MethodHasInvalidMappaContextParameter,
            DiagnosticsResources.MethodHasInvalidMappaContextParameter);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodIsVoid"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodIsVoid
        => methodIsVoid ??= BuildError(
            MappaDiagnosticsKind.MethodIsVoid,
            DiagnosticsResources.MethodIsVoid);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodReturnsTaskType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodReturnsTaskType
        => methodReturnsTaskType ??= BuildError(
            MappaDiagnosticsKind.MethodReturnsTaskType,
            DiagnosticsResources.MethodReturnsTaskType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicatedMapping"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicatedMapping
        => duplicateMapping ??= BuildError(
            MappaDiagnosticsKind.DuplicatedMapping,
            DiagnosticsResources.DuplicatedMapping);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotIdentifyStrategy"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotIdentifyStrategy
        => cannotIdentifyStrategy ??= BuildError(
            MappaDiagnosticsKind.CannotIdentifyStrategy,
            DiagnosticsResources.CannotIdentifyStrategy);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MultipleAttributesTargetTheSamePropertyOrParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor MultipleAttributesTargetTheSamePropertyOrParameter
        => multipleAttributesTargetTheSamePropertyOrParameter ??= BuildError(
            MappaDiagnosticsKind.MultipleAttributesTargetTheSamePropertyOrParameter,
            DiagnosticsResources.MultipleAttributesTargetTheSamePropertyOrParameter);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotDetectSuitableMethodToInvoke"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotDetectSuitableMethodToInvoke
        => cannotDetectSuitableMethodToInvoke ??= BuildError(
            MappaDiagnosticsKind.CannotDetectSuitableMethodToInvoke,
            DiagnosticsResources.CannotDetectSuitableMethodToInvoke);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotDetectType"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotDetectType
        => cannotDetectType ??= BuildError(
            MappaDiagnosticsKind.CannotDetectType,
            DiagnosticsResources.CannotDetectType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotFindFieldOrProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotFindFieldOrProperty
        => cannotFindFieldOrProperty ??= BuildError(
            MappaDiagnosticsKind.CannotFindFieldOrProperty,
            DiagnosticsResources.CannotFindFieldOrProperty);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotUseMappaAssignFromContextAttributeWithoutContextParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotUseMappaAssignFromContextAttributeWithoutContextParameter
        => cannotUseMappaAssignFromContextAttributeWithoutContextParameter ??= BuildError(
            MappaDiagnosticsKind.CannotUseMappaAssignFromContextAttributeWithoutContextParameter,
            DiagnosticsResources.CannotUseMappaAssignFromContextAttributeWithoutContextParameter);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.UserDefinedCultureIsMissingCultureName"/>.
    /// </summary>
    internal static DiagnosticDescriptor UserDefinedCultureIsMissingCultureName
        => userDefinedCultureIsMissingCultureName ??= BuildWarning(
            MappaDiagnosticsKind.UserDefinedCultureIsMissingCultureName,
            DiagnosticsResources.UserDefinedCultureIsMissingCultureName);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ParseExactDoesNotAcceptOnlyFormat"/>.
    /// </summary>
    internal static DiagnosticDescriptor ParseExactDoesNotAcceptOnlyFormat
        => parseExactDoesNotAcceptOnlyFormat ??= BuildWarning(
            MappaDiagnosticsKind.ParseExactDoesNotAcceptOnlyFormat,
            DiagnosticsResources.ParseExactDoesNotAcceptOnlyFormat);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PropertySetterIsNotAccessible"/>.
    /// </summary>
    internal static DiagnosticDescriptor PropertySetterIsNotAccessible
        => propertySetterIsNotAccessible ??= BuildWarning(
            MappaDiagnosticsKind.PropertySetterIsNotAccessible,
            DiagnosticsResources.PropertySetterIsNotAccessible);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TooManyUsePropertyAttributesForTheSameTargetProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor TooManyUsePropertyAttributesForTheSameTargetProperty
        => tooManyUsePropertyAttributesForTheSameTargetProperty ??= BuildError(
            MappaDiagnosticsKind.TooManyUsePropertyAttributesForTheSameTargetProperty,
            DiagnosticsResources.TooManyUsePropertyAttributesForTheSameTargetProperty);

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
            DiagnosticSeverity.Warning,
            true);
}