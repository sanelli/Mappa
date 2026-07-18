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
    CannotDetectSuitableMethodToInvokeForParameter,

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

    /// <summary>
    /// Mappa settings define a format but not a
    /// culture therefore ParseExact cannot be used
    /// and format is being ignored.
    /// </summary>
    ParseExactDoesNotAcceptOnlyFormat,

    /// <summary>
    /// The property setter is not accessible.
    /// </summary>
    PropertySetterIsNotAccessible,

    /// <summary>
    /// A method has multiple <see cref="MappaUsePropertyAttribute"/>s for the same property.
    /// </summary>
    TooManyUsePropertyAttributesForTheSameTargetProperty,

    /// <summary>
    /// A dependency is not providing any viable method for mapping.
    /// </summary>
    DependencyDoesNotProvideAnyViableMethod,

    /// <summary>
    /// A property that is not marked as required cannot be mapped.
    /// </summary>
    CannotMapNonRequiredProperty,

    /// <summary>
    /// An explicit target type does not implement or derive from the map method target type.
    /// </summary>
    ExplicitTargetTypeDoesNotDeriveMapMethodTargetType,

    /// <summary>
    /// The method to invoke is undefined.
    /// </summary>
    MethodToInvokeUndefined,

    /// <summary>
    /// The method to invoke cannot be identified: it does not exist or does not have the correct
    /// number of parameters.
    /// </summary>
    CannotIdentifySuitableMethodToInvoke,

    /// <summary>
    /// The type must be an exception.
    /// </summary>
    TypeMustBeAnException,

    /// <summary>
    /// The type must be a concrete type (i.e. non-abstract).
    /// </summary>
    TypeMustBeConcrete,

    /// <summary>
    /// The type must have a constructor with no parameters or a constructor with one string parameter.
    /// </summary>
    TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter,

    /// <summary>
    /// The <see cref="MappaTypeMappingDefaultAttribute"/> is undefined.
    /// </summary>
    MappaTypeDefaultBehaviorUndefined,

    /// <summary>
    /// The type specified in <see cref="MappaTypeMappingDefaultAttribute"/> will not be used.
    /// </summary>
    MappaTypeMappingDefaultAttributeUnusedType,

    /// <summary>
    /// Two or more <see cref="MappaTypeMappingAttribute"/> target the same type.
    /// </summary>
    MappaTypeMappingAttributeHaveTheSameSourceType,

    /// <summary>
    /// The <see cref="MappaTypeMappingAttribute"/> has the same source type as the method being mapped.
    /// </summary>
    MappaTypeMappingAttributeMapsSourceType,

    /// <summary>
    /// The <see cref="MappaTypeMappingAttribute"/> source type does not derive nor implement the source type of the method being mapped.
    /// </summary>
    MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType,

    /// <summary>
    /// The <see cref="MappaTypeMappingAttribute"/> target type does not derive nor implement the target type of the method being mapped.
    /// </summary>
    MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType,

    /// <summary>
    /// The field of property must be static in order to be corretly used.
    /// </summary>
    FieldOrPropertyMustBeStatic,

    /// <summary>
    /// A <see cref="MappaUsePropertyAttribute"/> source property will not be used because
    /// another mapping attribute targets the same property or constructor parameter.
    /// </summary>
    MappaUsePropertySourcePropertyWillNotBeUsed,

    /// <summary>
    /// A <see cref="MappaUsePropertyAttribute"/> source property will not be used because
    /// the method invoked via <see cref="MappaInvokeMethodAttribute"/> does not require it.
    /// </summary>
    MappaUsePropertyNotUsedByInvokeMethod,

    /// <summary>
    /// A mapping attribute targets a property or constructor parameter that does not exist on the target type.
    /// </summary>
    MappingAttributeTargetPropertyOrParameterDoesNotExist,

    /// <summary>
    /// A method has multiple <see cref="MappaIgnoreTargetPropertyAttribute"/>s for the same property.
    /// </summary>
    TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty,

    /// <summary>
    /// The target property or field for <see cref="MappaAssignToContextAttribute"/> does not exist or is not accessible.
    /// </summary>
    MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible,

    /// <summary>
    /// The <see cref="MappaAssignToContextAttribute"/> cannot be used
    /// because the <see cref="MappaContext"/> parameter is missing.
    /// </summary>
    CannotUseMappaAssignToContextAttributeWithoutContextParameter,

    /// <summary>
    /// Multiple <see cref="MappaAssignToContextAttribute"/> attributes on the same method use the same context key.
    /// </summary>
    MultipleMappaAssignToContextAttributesUseTheSameContextKey,

    /// <summary>
    /// A <see cref="MappaSettingsAttribute"/> style property has an integer value that is not a valid enum combination.
    /// </summary>
    InvalidMappaSettingsStyleValue,

    /// <summary>
    /// Not all source enum members can be mapped to the target enum by member name.
    /// </summary>
    NotAllSourceEnumMembersCanBeMapped,

    /// <summary>
    /// An enum member is missing a <see cref="System.ComponentModel.DescriptionAttribute"/> when Description mapping is enabled.
    /// </summary>
    EnumMemberMissingDescription,

    /// <summary>
    /// Enum mapping is ambiguous because multiple members match the same target.
    /// </summary>
    AmbiguousEnumMap,

    /// <summary>
    /// Invoke method resolution is ambiguous because multiple methods match.
    /// </summary>
    AmbiguousInvokeMethodResolution,

    /// <summary>
    /// A mapping attribute source property path is shorter than the target property path.
    /// </summary>
    MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath,

    /// <summary>
    /// A mapping attribute source property path segment does not exist on the source type.
    /// </summary>
    MappingAttributeSourcePropertyPathSegmentDoesNotExist,
}