// <copyright file="MappaDiagnosticsKind.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Generator.Diagnostics;

/// <summary>
/// The type of diagnostics report by Mappa Generator.
/// </summary>
internal enum MappaDiagnosticsKind
{
    /// <summary>
    /// Generic diagnostic debug.
    /// </summary>
    Debug,

    /// <summary>
    /// The method has an invalid number of parameters.
    /// </summary>
    MethodHasInvalidNumberOfParameters = 1,

    /// <summary>
    /// The method second parameter is not of type MappaContext.
    /// </summary>
    MethodHasInvalidMappaContextParameter,

    /// <summary>
    /// The method returns void.
    /// </summary>
    MethodIsVoid,

    /// <summary>
    /// The method returns any of the task types.
    /// </summary>
    MethodReturnsTaskType,

    /// <summary>
    /// A mapping for the given type already exists in the class.
    /// </summary>
    DuplicatedMapping,

    /// <summary>
    /// A mapping strategy cannot be identifier.
    /// </summary>
    CannotIdentifyStrategy,

    /// <summary>
    /// Multiple attributes target the same property or parameter.
    /// </summary>
    MultipleAttributesTargetTheSamePropertyOrParameter,

    /// <summary>
    /// Cannot identify a suitable method to invoke.
    /// </summary>
    CannotDetectSuitableMethodToInvoke,

    /// <summary>
    /// The type cannot be identified.
    /// </summary>
    CannotDetectType,

    /// <summary>
    /// The field or properties cannot be identified.
    /// </summary>
    CannotFindFieldOrProperty,

    /// <summary>
    /// The <see cref="MappaAssignFromContextAttribute"/> cannot be used
    /// because the <see cref="MappaContext"/> parameter is missing.
    /// </summary>
    CannotUseMappaAssignFromContextAttributeWithoutContextParameter,

    /// User defined settings are using the
    /// <see cref="CultureInfoSetting.UserDefined"/> culture
    /// but the culture name is not properly defined.
    UserDefinedCultureIsMissingCultureName,
}