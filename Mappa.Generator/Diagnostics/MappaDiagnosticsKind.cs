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

    /// <summary>
    /// A before-map or after-map hook method cannot be resolved.
    /// </summary>
    HookMethodNotFound,

    /// <summary>
    /// The same hook method is registered at class and method scope.
    /// </summary>
    DuplicateMapHookRegistration,

    /// <summary>
    /// An enum mapping configuration attribute references an enum which is not part of the current mapping.
    /// </summary>
    EnumMapAttributeEnumTypeMismatch,

    /// <summary>
    /// Two or more <see cref="MappaMapEnumMemberAttribute{TEnum}"/> declarations conflict with each other.
    /// </summary>
    EnumMapMemberMappingClash,

    /// <summary>
    /// A <see cref="MappaMapEnumIgnoreAttribute{TEnum}"/> excludes a member which is
    /// also configured by a <see cref="MappaMapEnumMemberAttribute{TEnum}"/>.
    /// </summary>
    EnumMapIgnoreConflictsWithMemberMapping,

    /// <summary>
    /// A <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> uses
    /// <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/> without providing a default value.
    /// </summary>
    EnumMapDefaultBehaviorRequiresDefaultValue,

    /// <summary>
    /// A <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> provides a default value
    /// which is not compatible with the target type of the mapping.
    /// </summary>
    EnumMapDefaultValueConstructorMismatch,

    /// <summary>
    /// A <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> provides a default value
    /// which is not used because the behaviour is <see cref="MappaMapEnumDefaultBehavior.Throw"/>.
    /// </summary>
    EnumMapDefaultAttributeUnusedDefaultValue,

    /// <summary>
    /// More than one <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> is applied to a
    /// map method whose source or return type is an enum.
    /// </summary>
    TooManyEnumMapDefaultAttributesOnDirectEnumMap,

    /// <summary>
    /// Multiple <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> declarations target the same enum.
    /// </summary>
    DuplicateEnumMapDefaultAttribute,

    /// <summary>
    /// A queryable projection map method declares <see cref="MappaBeforeMapAttribute"/>
    /// or <see cref="MappaAfterMapAttribute"/> hooks.
    /// </summary>
    ProjectionMethodHasBeforeOrAfterMapHooks,

    /// <summary>
    /// A queryable projection map method declares a <see cref="MappaContext"/> parameter.
    /// </summary>
    ProjectionMethodHasMappaContextParameter,

    /// <summary>
    /// A queryable projection element mapping contains an unsupported construct.
    /// </summary>
    ProjectionMappingNotSupported,

    /// <summary>
    /// A queryable projection element mapping invokes a method that cannot be inlined into an expression.
    /// </summary>
    ProjectionInvokeMethodNotInlinable,

    /// <summary>
    /// A queryable projection element mapping involves a nested <see cref="System.Linq.IQueryable{T}"/> property.
    /// </summary>
    ProjectionNestedQueryableNotSupported,

    /// <summary>
    /// A queryable projection uses an enum mapping strategy that may not translate in LINQ providers.
    /// </summary>
    ProjectionEnumStrategyNotSupported,

    /// <summary>
    /// An <see cref="System.Linq.IQueryable{T}"/> source is mapped to a concrete collection instead of a queryable projection.
    /// </summary>
    IQueryableMappedAsCollection,
}