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
    private static DiagnosticDescriptor? dependencyDoesNotProvideAnyViableMethod;
    private static DiagnosticDescriptor? cannotMapNonRequiredProperty;
    private static DiagnosticDescriptor? explicitTargetTypeDoesNotDeriveMapMethodTargetType;
    private static DiagnosticDescriptor? methodToInvokeUndefined;
    private static DiagnosticDescriptor? cannotIdentifySuitableMethodToInvoke;
    private static DiagnosticDescriptor? typeMustBeAnException;
    private static DiagnosticDescriptor? typeMustBeConcrete;
    private static DiagnosticDescriptor? typeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter;
    private static DiagnosticDescriptor? mappaTypeDefaultBehaviorUndefined;
    private static DiagnosticDescriptor? mappaTypeMappingDefaultAttributeUnused;
    private static DiagnosticDescriptor? mappaTypeMappingAttributeHaveTheSameSourceType;
    private static DiagnosticDescriptor? mappaTypeMappingAttributeMapsSourceType;
    private static DiagnosticDescriptor? mappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType;
    private static DiagnosticDescriptor? mappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType;
    private static DiagnosticDescriptor? fieldOrPropertyMustBeStatic;
    private static DiagnosticDescriptor? mappaUsePropertySourcePropertyWillNotBeUsed;
    private static DiagnosticDescriptor? mappaUsePropertyNotUsedByInvokeMethod;
    private static DiagnosticDescriptor? mappingAttributeTargetPropertyOrParameterDoesNotExist;
    private static DiagnosticDescriptor? tooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty;
    private static DiagnosticDescriptor? mappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible;
    private static DiagnosticDescriptor? cannotUseMappaAssignToContextAttributeWithoutContextParameter;
    private static DiagnosticDescriptor? multipleMappaAssignToContextAttributesUseTheSameContextKey;
    private static DiagnosticDescriptor? invalidMappaSettingsStyleValue;
    private static DiagnosticDescriptor? notAllSourceEnumMembersCanBeMapped;
    private static DiagnosticDescriptor? enumMemberMissingDescription;
    private static DiagnosticDescriptor? ambiguousEnumMap;
    private static DiagnosticDescriptor? ambiguousInvokeMethodResolution;
    private static DiagnosticDescriptor? mappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath;
    private static DiagnosticDescriptor? mappingAttributeSourcePropertyPathSegmentDoesNotExist;
    private static DiagnosticDescriptor? hookMethodNotFound;
    private static DiagnosticDescriptor? duplicateMapHookRegistration;
    private static DiagnosticDescriptor? enumMapAttributeEnumTypeMismatch;
    private static DiagnosticDescriptor? enumMapMemberMappingClash;
    private static DiagnosticDescriptor? enumMapIgnoreConflictsWithMemberMapping;
    private static DiagnosticDescriptor? enumMapDefaultBehaviorRequiresDefaultValue;
    private static DiagnosticDescriptor? enumMapDefaultValueConstructorMismatch;
    private static DiagnosticDescriptor? enumMapDefaultAttributeUnusedDefaultValue;
    private static DiagnosticDescriptor? tooManyEnumMapDefaultAttributesOnDirectEnumMap;
    private static DiagnosticDescriptor? duplicateEnumMapDefaultAttribute;
    private static DiagnosticDescriptor? projectionMethodHasBeforeOrAfterMapHooks;
    private static DiagnosticDescriptor? projectionMethodHasMappaContextParameter;
    private static DiagnosticDescriptor? projectionMappingNotSupported;
    private static DiagnosticDescriptor? projectionInvokeMethodNotInlinable;
    private static DiagnosticDescriptor? projectionNestedQueryableNotSupported;
    private static DiagnosticDescriptor? projectionEnumStrategyNotSupported;
    private static DiagnosticDescriptor? iQueryableMappedAsCollection;
    private static DiagnosticDescriptor? duplicateObjectFactoryForTargetType;
    private static DiagnosticDescriptor? objectFactoryMethodNotFound;
    private static DiagnosticDescriptor? projectionMethodHasObjectFactory;
    private static DiagnosticDescriptor? mustMapTargetPropertyWasNotMapped;
    private static DiagnosticDescriptor? mappaMustMapTargetPropertyListsRequiredProperty;
    private static DiagnosticDescriptor? unsafeAccessorNotSupported;
    private static DiagnosticDescriptor? allowInaccessibleTargetMembersDisabledAll;
    private static DiagnosticDescriptor? projectionMethodHasAllowInaccessibleMembers;
    private static DiagnosticDescriptor? mappaDependencyInjectionClassIsNotPartial;
    private static DiagnosticDescriptor? mappaAndMappaDependencyInjectionBothApplied;
    private static DiagnosticDescriptor? mappaDependencyInjectionMapperHasNoEligibleInterfaces;
    private static DiagnosticDescriptor? mappaDependencyInjectionStaticMapperSkipped;
    private static DiagnosticDescriptor? referenceHandlingRootMapWithoutMappaContext;
    private static DiagnosticDescriptor? referenceHandlingNestedMapWithoutMappaContext;
    private static DiagnosticDescriptor? maxCompileTimeDepthReached;
    private static DiagnosticDescriptor? mappingCycleDetected;
    private static DiagnosticDescriptor? mappingCycleAutoBroken;
    private static DiagnosticDescriptor? preferredInterfaceCannotBeUsed;
    private static DiagnosticDescriptor? preferredInterfaceImplementingTypeNotConcrete;
    private static DiagnosticDescriptor? preferredInterfaceNotImplemented;
    private static DiagnosticDescriptor? preferredInterfaceAttributeNotNeeded;
    private static DiagnosticDescriptor? duplicatePreferredInterfaceAttribute;
    private static DiagnosticDescriptor? preferredInterfaceNotClosedGenericInterface;
    private static DiagnosticDescriptor? preferredInterfaceImplementingTypeInvalid;
    private static DiagnosticDescriptor? projectionMethodMapReplacedByStructuralStrategy;
    private static DiagnosticDescriptor? mappaUseConstructorNoEmptyConstructor;
    private static DiagnosticDescriptor? mappaUseConstructorNoMatchingParameterizedConstructor;
    private static DiagnosticDescriptor? mappaUseConstructorInvalidConfiguration;
    private static DiagnosticDescriptor? mappaUseConstructorCannotMap;
    private static DiagnosticDescriptor? duplicateMappaUseConstructorForTargetType;
    private static DiagnosticDescriptor? mappaUseConstructorAttributeNotUsed;
    private static DiagnosticDescriptor? protobufPolymorphismMixedWithTypeMapping;
    private static DiagnosticDescriptor? protobufPolymorphismSetupMissing;
    private static DiagnosticDescriptor? protobufPolymorphismMultipleSetups;
    private static DiagnosticDescriptor? protobufPolymorphismSetupWrongArity;
    private static DiagnosticDescriptor? protobufPolymorphismCasePropertyNotFound;
    private static DiagnosticDescriptor? protobufPolymorphismCasePropertyNotOneofCaseEnum;
    private static DiagnosticDescriptor? protobufPolymorphismCaseNameUnknown;
    private static DiagnosticDescriptor? protobufPolymorphismCaseNameNone;
    private static DiagnosticDescriptor? protobufPolymorphismScalarArm;
    private static DiagnosticDescriptor? protobufPolymorphismDuplicateArm;
    private static DiagnosticDescriptor? protobufPolymorphismDefaultRequiresProtobufTarget;
    private static DiagnosticDescriptor? protobufPolymorphismMapToCaseMissing;
    private static DiagnosticDescriptor? protobufPolymorphismDefaultBehaviorUndefined;
    private static DiagnosticDescriptor? protobufPolymorphismDefaultArgumentUnused;
    private static DiagnosticDescriptor? protobufPolymorphismClrTypeNotCompatible;
    private static DiagnosticDescriptor? protobufPolymorphismArmPropertyNotWritable;
    private static DiagnosticDescriptor? protobufPolymorphismArmFormMismatch;
    private static DiagnosticDescriptor? invalidTransformerSignature;
    private static DiagnosticDescriptor? applyTransformerMethodNotFound;
    private static DiagnosticDescriptor? applyTransformerSourceMemberNotFound;
    private static DiagnosticDescriptor? applyTransformerClassTypeAndFieldNameConflict;
    private static DiagnosticDescriptor? projectionMethodHasApplyTransformer;
    private static DiagnosticDescriptor? nullSubstituteModeMissingOrAmbiguous;
    private static DiagnosticDescriptor? nullSubstituteInvokeMethodTypeAndSourceConflict;
    private static DiagnosticDescriptor? tooManyNullSubstituteAttributesForTheSameTargetProperty;
    private static DiagnosticDescriptor? nullSubstituteIgnoredBecauseAssignFromConstant;
    private static DiagnosticDescriptor? projectionMethodHasNullSubstituteInvokeMethod;
    private static DiagnosticDescriptor? mappaInterceptorClassIsNotPartial;
    private static DiagnosticDescriptor? mappaInterceptorClassIsStatic;
    private static DiagnosticDescriptor? mappaInterceptorWithArgumentMustBeObjectCreation;
    private static DiagnosticDescriptor? mappaInterceptorMultipleSettings;
    private static DiagnosticDescriptor? mappaInterceptorDuplicateStaticDependencyType;
    private static DiagnosticDescriptor? mappaInterceptorDuplicateNonMultipleAttribute;
    private static DiagnosticDescriptor? mappaInterceptorUnsupportedFeature;
    private static DiagnosticDescriptor? mappaInterceptorQueryableNotSupported;
    private static DiagnosticDescriptor? mappaInterceptorInvalidMethodAttributeType;

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
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotDetectSuitableMethodToInvokeForParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotDetectSuitableMethodToInvokeForParameter
        => cannotDetectSuitableMethodToInvoke ??= BuildError(
            MappaDiagnosticsKind.CannotDetectSuitableMethodToInvokeForParameter,
            DiagnosticsResources.CannotDetectSuitableMethodToInvokeForParameter);

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

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TooManyUsePropertyAttributesForTheSameTargetProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor DependencyDoesNotProvideAnyViableMethod
        => dependencyDoesNotProvideAnyViableMethod ??= BuildWarning(
            MappaDiagnosticsKind.DependencyDoesNotProvideAnyViableMethod,
            DiagnosticsResources.DependencyDoesNotProvideAnyViableMethod);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotMapNonRequiredProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotMapNonRequiredProperty
        => cannotMapNonRequiredProperty ??= BuildWarning(
            MappaDiagnosticsKind.CannotMapNonRequiredProperty,
            DiagnosticsResources.CannotMapNonRequiredProperty);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType"/>.
    /// </summary>
    internal static DiagnosticDescriptor ExplicitTargetTypeDoesNotDeriveMapMethodTargetType
        => explicitTargetTypeDoesNotDeriveMapMethodTargetType ??= BuildError(
            MappaDiagnosticsKind.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType,
            DiagnosticsResources.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MethodToInvokeUndefined"/>.
    /// </summary>
    internal static DiagnosticDescriptor MethodToInvokeUndefined
        => methodToInvokeUndefined ??= BuildError(
            MappaDiagnosticsKind.MethodToInvokeUndefined,
            DiagnosticsResources.MethodToInvokeUndefined);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotIdentifySuitableMethodToInvoke"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotIdentifySuitableMethodToInvoke
        => cannotIdentifySuitableMethodToInvoke ??= BuildError(
            MappaDiagnosticsKind.CannotIdentifySuitableMethodToInvoke,
            DiagnosticsResources.CannotIdentifySuitableMethodToInvoke);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TypeMustBeAnException"/>.
    /// </summary>
    internal static DiagnosticDescriptor TypeMustBeAnException
        => typeMustBeAnException ??= BuildError(
            MappaDiagnosticsKind.TypeMustBeAnException,
            DiagnosticsResources.TypeMustBeAnException);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TypeMustBeConcrete"/>.
    /// </summary>
    internal static DiagnosticDescriptor TypeMustBeConcrete
        => typeMustBeConcrete ??= BuildError(
            MappaDiagnosticsKind.TypeMustBeConcrete,
            DiagnosticsResources.TypeMustBeConcrete);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter
        => typeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter ??= BuildError(
            MappaDiagnosticsKind.TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter,
            DiagnosticsResources.TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaTypeDefaultBehaviorUndefined"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaTypeDefaultBehaviorUndefined
        => mappaTypeDefaultBehaviorUndefined ??= BuildError(
            MappaDiagnosticsKind.MappaTypeDefaultBehaviorUndefined,
            DiagnosticsResources.MappaTypeDefaultBehaviorUndefined);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaTypeMappingDefaultAttributeUnusedType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaTypeMappingDefaultAttributeUnusedType
        => mappaTypeMappingDefaultAttributeUnused ??= BuildWarning(
            MappaDiagnosticsKind.MappaTypeMappingDefaultAttributeUnusedType,
            DiagnosticsResources.MappaTypeMappingDefaultAttributeUnusedType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaTypeMappingAttributeHaveTheSameSourceType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaTypeMappingAttributeHaveTheSameSourceType
        => mappaTypeMappingAttributeHaveTheSameSourceType ??= BuildError(
            MappaDiagnosticsKind.MappaTypeMappingAttributeHaveTheSameSourceType,
            DiagnosticsResources.MappaTypeMappingAttributeHaveTheSameSourceType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaTypeMappingAttributeMapsSourceType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaTypeMappingAttributeMapsSourceType
        => mappaTypeMappingAttributeMapsSourceType ??= BuildError(
            MappaDiagnosticsKind.MappaTypeMappingAttributeMapsSourceType,
            DiagnosticsResources.MappaTypeMappingAttributeMapsSourceType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType
        => mappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType ??= BuildError(
            MappaDiagnosticsKind.MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType,
            DiagnosticsResources.MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType
        => mappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType ??= BuildError(
            MappaDiagnosticsKind.MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType,
            DiagnosticsResources.MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.FieldOrPropertyMustBeStatic"/>.
    /// </summary>
    internal static DiagnosticDescriptor FieldOrPropertyMustBeStatic
        => fieldOrPropertyMustBeStatic ??= BuildError(
            MappaDiagnosticsKind.FieldOrPropertyMustBeStatic,
            DiagnosticsResources.FieldOrPropertyMustBeStatic);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUsePropertySourcePropertyWillNotBeUsed"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUsePropertySourcePropertyWillNotBeUsed
        => mappaUsePropertySourcePropertyWillNotBeUsed ??= BuildWarning(
            MappaDiagnosticsKind.MappaUsePropertySourcePropertyWillNotBeUsed,
            DiagnosticsResources.MappaUsePropertySourcePropertyWillNotBeUsed);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUsePropertyNotUsedByInvokeMethod"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUsePropertyNotUsedByInvokeMethod
        => mappaUsePropertyNotUsedByInvokeMethod ??= BuildWarning(
            MappaDiagnosticsKind.MappaUsePropertyNotUsedByInvokeMethod,
            DiagnosticsResources.MappaUsePropertyNotUsedByInvokeMethod);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappingAttributeTargetPropertyOrParameterDoesNotExist"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappingAttributeTargetPropertyOrParameterDoesNotExist
        => mappingAttributeTargetPropertyOrParameterDoesNotExist ??= BuildWarning(
            MappaDiagnosticsKind.MappingAttributeTargetPropertyOrParameterDoesNotExist,
            DiagnosticsResources.MappingAttributeTargetPropertyOrParameterDoesNotExist);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty
        => tooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty ??= BuildError(
            MappaDiagnosticsKind.TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty,
            DiagnosticsResources.TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible
        => mappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible ??= BuildError(
            MappaDiagnosticsKind.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible,
            DiagnosticsResources.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.CannotUseMappaAssignToContextAttributeWithoutContextParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor CannotUseMappaAssignToContextAttributeWithoutContextParameter
        => cannotUseMappaAssignToContextAttributeWithoutContextParameter ??= BuildWarning(
            MappaDiagnosticsKind.CannotUseMappaAssignToContextAttributeWithoutContextParameter,
            DiagnosticsResources.CannotUseMappaAssignToContextAttributeWithoutContextParameter);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MultipleMappaAssignToContextAttributesUseTheSameContextKey"/>.
    /// </summary>
    internal static DiagnosticDescriptor MultipleMappaAssignToContextAttributesUseTheSameContextKey
        => multipleMappaAssignToContextAttributesUseTheSameContextKey ??= BuildError(
            MappaDiagnosticsKind.MultipleMappaAssignToContextAttributesUseTheSameContextKey,
            DiagnosticsResources.MultipleMappaAssignToContextAttributesUseTheSameContextKey);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.InvalidMappaSettingsStyleValue"/>.
    /// </summary>
    internal static DiagnosticDescriptor InvalidMappaSettingsStyleValue
        => invalidMappaSettingsStyleValue ??= BuildWarning(
            MappaDiagnosticsKind.InvalidMappaSettingsStyleValue,
            DiagnosticsResources.InvalidMappaSettingsStyleValue);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.NotAllSourceEnumMembersCanBeMapped"/>.
    /// </summary>
    internal static DiagnosticDescriptor NotAllSourceEnumMembersCanBeMapped
        => notAllSourceEnumMembersCanBeMapped ??= BuildWarning(
            MappaDiagnosticsKind.NotAllSourceEnumMembersCanBeMapped,
            DiagnosticsResources.NotAllSourceEnumMembersCanBeMapped);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMemberMissingDescription"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMemberMissingDescription
        => enumMemberMissingDescription ??= BuildError(
            MappaDiagnosticsKind.EnumMemberMissingDescription,
            DiagnosticsResources.EnumMemberMissingDescription);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.AmbiguousEnumMap"/>.
    /// </summary>
    internal static DiagnosticDescriptor AmbiguousEnumMap
        => ambiguousEnumMap ??= BuildError(
            MappaDiagnosticsKind.AmbiguousEnumMap,
            DiagnosticsResources.AmbiguousEnumMap);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.AmbiguousInvokeMethodResolution"/>.
    /// </summary>
    internal static DiagnosticDescriptor AmbiguousInvokeMethodResolution
        => ambiguousInvokeMethodResolution ??= BuildError(
            MappaDiagnosticsKind.AmbiguousInvokeMethodResolution,
            DiagnosticsResources.AmbiguousInvokeMethodResolution);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath
        => mappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath ??= BuildError(
            MappaDiagnosticsKind.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath,
            DiagnosticsResources.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappingAttributeSourcePropertyPathSegmentDoesNotExist"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappingAttributeSourcePropertyPathSegmentDoesNotExist
        => mappingAttributeSourcePropertyPathSegmentDoesNotExist ??= BuildError(
            MappaDiagnosticsKind.MappingAttributeSourcePropertyPathSegmentDoesNotExist,
            DiagnosticsResources.MappingAttributeSourcePropertyPathSegmentDoesNotExist);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.HookMethodNotFound"/>.
    /// </summary>
    internal static DiagnosticDescriptor HookMethodNotFound
        => hookMethodNotFound ??= BuildWarning(
            MappaDiagnosticsKind.HookMethodNotFound,
            DiagnosticsResources.HookMethodNotFound);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicateMapHookRegistration"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicateMapHookRegistration
        => duplicateMapHookRegistration ??= BuildWarning(
            MappaDiagnosticsKind.DuplicateMapHookRegistration,
            DiagnosticsResources.DuplicateMapHookRegistration);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMapAttributeEnumTypeMismatch"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMapAttributeEnumTypeMismatch
        => enumMapAttributeEnumTypeMismatch ??= BuildError(
            MappaDiagnosticsKind.EnumMapAttributeEnumTypeMismatch,
            DiagnosticsResources.EnumMapAttributeEnumTypeMismatch);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMapMemberMappingClash"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMapMemberMappingClash
        => enumMapMemberMappingClash ??= BuildError(
            MappaDiagnosticsKind.EnumMapMemberMappingClash,
            DiagnosticsResources.EnumMapMemberMappingClash);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMapIgnoreConflictsWithMemberMapping"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMapIgnoreConflictsWithMemberMapping
        => enumMapIgnoreConflictsWithMemberMapping ??= BuildError(
            MappaDiagnosticsKind.EnumMapIgnoreConflictsWithMemberMapping,
            DiagnosticsResources.EnumMapIgnoreConflictsWithMemberMapping);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMapDefaultBehaviorRequiresDefaultValue"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMapDefaultBehaviorRequiresDefaultValue
        => enumMapDefaultBehaviorRequiresDefaultValue ??= BuildError(
            MappaDiagnosticsKind.EnumMapDefaultBehaviorRequiresDefaultValue,
            DiagnosticsResources.EnumMapDefaultBehaviorRequiresDefaultValue);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMapDefaultValueConstructorMismatch"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMapDefaultValueConstructorMismatch
        => enumMapDefaultValueConstructorMismatch ??= BuildError(
            MappaDiagnosticsKind.EnumMapDefaultValueConstructorMismatch,
            DiagnosticsResources.EnumMapDefaultValueConstructorMismatch);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.EnumMapDefaultAttributeUnusedDefaultValue"/>.
    /// </summary>
    internal static DiagnosticDescriptor EnumMapDefaultAttributeUnusedDefaultValue
        => enumMapDefaultAttributeUnusedDefaultValue ??= BuildWarning(
            MappaDiagnosticsKind.EnumMapDefaultAttributeUnusedDefaultValue,
            DiagnosticsResources.EnumMapDefaultAttributeUnusedDefaultValue);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TooManyEnumMapDefaultAttributesOnDirectEnumMap"/>.
    /// </summary>
    internal static DiagnosticDescriptor TooManyEnumMapDefaultAttributesOnDirectEnumMap
        => tooManyEnumMapDefaultAttributesOnDirectEnumMap ??= BuildError(
            MappaDiagnosticsKind.TooManyEnumMapDefaultAttributesOnDirectEnumMap,
            DiagnosticsResources.TooManyEnumMapDefaultAttributesOnDirectEnumMap);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicateEnumMapDefaultAttribute"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicateEnumMapDefaultAttribute
        => duplicateEnumMapDefaultAttribute ??= BuildError(
            MappaDiagnosticsKind.DuplicateEnumMapDefaultAttribute,
            DiagnosticsResources.DuplicateEnumMapDefaultAttribute);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodHasBeforeOrAfterMapHooks"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodHasBeforeOrAfterMapHooks
        => projectionMethodHasBeforeOrAfterMapHooks ??= BuildError(
            MappaDiagnosticsKind.ProjectionMethodHasBeforeOrAfterMapHooks,
            DiagnosticsResources.ProjectionMethodHasBeforeOrAfterMapHooks);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodHasMappaContextParameter"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodHasMappaContextParameter
        => projectionMethodHasMappaContextParameter ??= BuildError(
            MappaDiagnosticsKind.ProjectionMethodHasMappaContextParameter,
            DiagnosticsResources.ProjectionMethodHasMappaContextParameter);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMappingNotSupported"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMappingNotSupported
        => projectionMappingNotSupported ??= BuildError(
            MappaDiagnosticsKind.ProjectionMappingNotSupported,
            DiagnosticsResources.ProjectionMappingNotSupported);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionInvokeMethodNotInlinable"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionInvokeMethodNotInlinable
        => projectionInvokeMethodNotInlinable ??= BuildError(
            MappaDiagnosticsKind.ProjectionInvokeMethodNotInlinable,
            DiagnosticsResources.ProjectionInvokeMethodNotInlinable);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionNestedQueryableNotSupported"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionNestedQueryableNotSupported
        => projectionNestedQueryableNotSupported ??= BuildError(
            MappaDiagnosticsKind.ProjectionNestedQueryableNotSupported,
            DiagnosticsResources.ProjectionNestedQueryableNotSupported);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionEnumStrategyNotSupported"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionEnumStrategyNotSupported
        => projectionEnumStrategyNotSupported ??= BuildWarning(
            MappaDiagnosticsKind.ProjectionEnumStrategyNotSupported,
            DiagnosticsResources.ProjectionEnumStrategyNotSupported);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.IQueryableMappedAsCollection"/>.
    /// </summary>
    internal static DiagnosticDescriptor IQueryableMappedAsCollection
        => iQueryableMappedAsCollection ??= BuildWarning(
            MappaDiagnosticsKind.IQueryableMappedAsCollection,
            DiagnosticsResources.IQueryableMappedAsCollection);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicateObjectFactoryForTargetType"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicateObjectFactoryForTargetType
        => duplicateObjectFactoryForTargetType ??= BuildError(
            MappaDiagnosticsKind.DuplicateObjectFactoryForTargetType,
            DiagnosticsResources.DuplicateObjectFactoryForTargetType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ObjectFactoryMethodNotFound"/>.
    /// </summary>
    internal static DiagnosticDescriptor ObjectFactoryMethodNotFound
        => objectFactoryMethodNotFound ??= BuildWarning(
            MappaDiagnosticsKind.ObjectFactoryMethodNotFound,
            DiagnosticsResources.ObjectFactoryMethodNotFound);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodHasObjectFactory"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodHasObjectFactory
        => projectionMethodHasObjectFactory ??= BuildError(
            MappaDiagnosticsKind.ProjectionMethodHasObjectFactory,
            DiagnosticsResources.ProjectionMethodHasObjectFactory);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MustMapTargetPropertyWasNotMapped"/>.
    /// </summary>
    internal static DiagnosticDescriptor MustMapTargetPropertyWasNotMapped
        => mustMapTargetPropertyWasNotMapped ??= BuildError(
            MappaDiagnosticsKind.MustMapTargetPropertyWasNotMapped,
            DiagnosticsResources.MustMapTargetPropertyWasNotMapped);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaMustMapTargetPropertyListsRequiredProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaMustMapTargetPropertyListsRequiredProperty
        => mappaMustMapTargetPropertyListsRequiredProperty ??= BuildWarning(
            MappaDiagnosticsKind.MappaMustMapTargetPropertyListsRequiredProperty,
            DiagnosticsResources.MappaMustMapTargetPropertyListsRequiredProperty);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.UnsafeAccessorNotSupported"/>.
    /// </summary>
    internal static DiagnosticDescriptor UnsafeAccessorNotSupported
        => unsafeAccessorNotSupported ??= BuildError(
            MappaDiagnosticsKind.UnsafeAccessorNotSupported,
            DiagnosticsResources.UnsafeAccessorNotSupported);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.AllowInaccessibleTargetMembersDisabledAll"/>.
    /// </summary>
    internal static DiagnosticDescriptor AllowInaccessibleTargetMembersDisabledAll
        => allowInaccessibleTargetMembersDisabledAll ??= BuildError(
            MappaDiagnosticsKind.AllowInaccessibleTargetMembersDisabledAll,
            DiagnosticsResources.AllowInaccessibleTargetMembersDisabledAll);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodHasAllowInaccessibleMembers"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodHasAllowInaccessibleMembers
        => projectionMethodHasAllowInaccessibleMembers ??= BuildError(
            MappaDiagnosticsKind.ProjectionMethodHasAllowInaccessibleMembers,
            DiagnosticsResources.ProjectionMethodHasAllowInaccessibleMembers);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaDependencyInjectionClassIsNotPartial"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaDependencyInjectionClassIsNotPartial
        => mappaDependencyInjectionClassIsNotPartial ??= BuildWarning(
            MappaDiagnosticsKind.MappaDependencyInjectionClassIsNotPartial,
            DiagnosticsResources.MappaDependencyInjectionClassIsNotPartial);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaAndMappaDependencyInjectionBothApplied"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaAndMappaDependencyInjectionBothApplied
        => mappaAndMappaDependencyInjectionBothApplied ??= BuildError(
            MappaDiagnosticsKind.MappaAndMappaDependencyInjectionBothApplied,
            DiagnosticsResources.MappaAndMappaDependencyInjectionBothApplied);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaDependencyInjectionMapperHasNoEligibleInterfaces"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaDependencyInjectionMapperHasNoEligibleInterfaces
        => mappaDependencyInjectionMapperHasNoEligibleInterfaces ??= BuildWarning(
            MappaDiagnosticsKind.MappaDependencyInjectionMapperHasNoEligibleInterfaces,
            DiagnosticsResources.MappaDependencyInjectionMapperHasNoEligibleInterfaces);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaDependencyInjectionStaticMapperSkipped"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaDependencyInjectionStaticMapperSkipped
        => mappaDependencyInjectionStaticMapperSkipped ??= BuildWarning(
            MappaDiagnosticsKind.MappaDependencyInjectionStaticMapperSkipped,
            DiagnosticsResources.MappaDependencyInjectionStaticMapperSkipped);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ReferenceHandlingRootMapWithoutMappaContext"/>.
    /// </summary>
    internal static DiagnosticDescriptor ReferenceHandlingRootMapWithoutMappaContext
        => referenceHandlingRootMapWithoutMappaContext ??= BuildWarning(
            MappaDiagnosticsKind.ReferenceHandlingRootMapWithoutMappaContext,
            DiagnosticsResources.ReferenceHandlingRootMapWithoutMappaContext);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ReferenceHandlingNestedMapWithoutMappaContext"/>.
    /// </summary>
    internal static DiagnosticDescriptor ReferenceHandlingNestedMapWithoutMappaContext
        => referenceHandlingNestedMapWithoutMappaContext ??= BuildWarning(
            MappaDiagnosticsKind.ReferenceHandlingNestedMapWithoutMappaContext,
            DiagnosticsResources.ReferenceHandlingNestedMapWithoutMappaContext);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MaxCompileTimeDepthReached"/>.
    /// </summary>
    internal static DiagnosticDescriptor MaxCompileTimeDepthReached
        => maxCompileTimeDepthReached ??= BuildError(
            MappaDiagnosticsKind.MaxCompileTimeDepthReached,
            DiagnosticsResources.MaxCompileTimeDepthReached);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappingCycleDetected"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappingCycleDetected
        => mappingCycleDetected ??= BuildError(
            MappaDiagnosticsKind.MappingCycleDetected,
            DiagnosticsResources.MappingCycleDetected);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappingCycleAutoBroken"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappingCycleAutoBroken
        => mappingCycleAutoBroken ??= BuildWarning(
            MappaDiagnosticsKind.MappingCycleAutoBroken,
            DiagnosticsResources.MappingCycleAutoBroken);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PreferredInterfaceCannotBeUsed"/>.
    /// </summary>
    internal static DiagnosticDescriptor PreferredInterfaceCannotBeUsed
        => preferredInterfaceCannotBeUsed ??= BuildError(
            MappaDiagnosticsKind.PreferredInterfaceCannotBeUsed,
            DiagnosticsResources.PreferredInterfaceCannotBeUsed);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PreferredInterfaceImplementingTypeNotConcrete"/>.
    /// </summary>
    internal static DiagnosticDescriptor PreferredInterfaceImplementingTypeNotConcrete
        => preferredInterfaceImplementingTypeNotConcrete ??= BuildError(
            MappaDiagnosticsKind.PreferredInterfaceImplementingTypeNotConcrete,
            DiagnosticsResources.PreferredInterfaceImplementingTypeNotConcrete);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PreferredInterfaceNotImplemented"/>.
    /// </summary>
    internal static DiagnosticDescriptor PreferredInterfaceNotImplemented
        => preferredInterfaceNotImplemented ??= BuildWarning(
            MappaDiagnosticsKind.PreferredInterfaceNotImplemented,
            DiagnosticsResources.PreferredInterfaceNotImplemented);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PreferredInterfaceAttributeNotNeeded"/>.
    /// </summary>
    internal static DiagnosticDescriptor PreferredInterfaceAttributeNotNeeded
        => preferredInterfaceAttributeNotNeeded ??= BuildWarning(
            MappaDiagnosticsKind.PreferredInterfaceAttributeNotNeeded,
            DiagnosticsResources.PreferredInterfaceAttributeNotNeeded);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicatePreferredInterfaceAttribute"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicatePreferredInterfaceAttribute
        => duplicatePreferredInterfaceAttribute ??= BuildError(
            MappaDiagnosticsKind.DuplicatePreferredInterfaceAttribute,
            DiagnosticsResources.DuplicatePreferredInterfaceAttribute);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PreferredInterfaceNotClosedGenericInterface"/>.
    /// </summary>
    internal static DiagnosticDescriptor PreferredInterfaceNotClosedGenericInterface
        => preferredInterfaceNotClosedGenericInterface ??= BuildError(
            MappaDiagnosticsKind.PreferredInterfaceNotClosedGenericInterface,
            DiagnosticsResources.PreferredInterfaceNotClosedGenericInterface);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.PreferredInterfaceImplementingTypeInvalid"/>.
    /// </summary>
    internal static DiagnosticDescriptor PreferredInterfaceImplementingTypeInvalid
        => preferredInterfaceImplementingTypeInvalid ??= BuildError(
            MappaDiagnosticsKind.PreferredInterfaceImplementingTypeInvalid,
            DiagnosticsResources.PreferredInterfaceImplementingTypeInvalid);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodMapReplacedByStructuralStrategy"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodMapReplacedByStructuralStrategy
        => projectionMethodMapReplacedByStructuralStrategy ??= BuildWarning(
            MappaDiagnosticsKind.ProjectionMethodMapReplacedByStructuralStrategy,
            DiagnosticsResources.ProjectionMethodMapReplacedByStructuralStrategy);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUseConstructorNoEmptyConstructor"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUseConstructorNoEmptyConstructor
        => mappaUseConstructorNoEmptyConstructor ??= BuildError(
            MappaDiagnosticsKind.MappaUseConstructorNoEmptyConstructor,
            DiagnosticsResources.MappaUseConstructorNoEmptyConstructor);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUseConstructorNoMatchingParameterizedConstructor"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUseConstructorNoMatchingParameterizedConstructor
        => mappaUseConstructorNoMatchingParameterizedConstructor ??= BuildError(
            MappaDiagnosticsKind.MappaUseConstructorNoMatchingParameterizedConstructor,
            DiagnosticsResources.MappaUseConstructorNoMatchingParameterizedConstructor);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUseConstructorInvalidConfiguration"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUseConstructorInvalidConfiguration
        => mappaUseConstructorInvalidConfiguration ??= BuildError(
            MappaDiagnosticsKind.MappaUseConstructorInvalidConfiguration,
            DiagnosticsResources.MappaUseConstructorInvalidConfiguration);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUseConstructorCannotMap"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUseConstructorCannotMap
        => mappaUseConstructorCannotMap ??= BuildError(
            MappaDiagnosticsKind.MappaUseConstructorCannotMap,
            DiagnosticsResources.MappaUseConstructorCannotMap);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.DuplicateMappaUseConstructorForTargetType"/>.
    /// </summary>
    internal static DiagnosticDescriptor DuplicateMappaUseConstructorForTargetType
        => duplicateMappaUseConstructorForTargetType ??= BuildError(
            MappaDiagnosticsKind.DuplicateMappaUseConstructorForTargetType,
            DiagnosticsResources.DuplicateMappaUseConstructorForTargetType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaUseConstructorAttributeNotUsed"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaUseConstructorAttributeNotUsed
        => mappaUseConstructorAttributeNotUsed ??= BuildWarning(
            MappaDiagnosticsKind.MappaUseConstructorAttributeNotUsed,
            DiagnosticsResources.MappaUseConstructorAttributeNotUsed);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismMixedWithTypeMapping"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismMixedWithTypeMapping
        => protobufPolymorphismMixedWithTypeMapping ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismMixedWithTypeMapping,
            DiagnosticsResources.ProtobufPolymorphismMixedWithTypeMapping);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismSetupMissing"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismSetupMissing
        => protobufPolymorphismSetupMissing ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismSetupMissing,
            DiagnosticsResources.ProtobufPolymorphismSetupMissing);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismMultipleSetups"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismMultipleSetups
        => protobufPolymorphismMultipleSetups ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismMultipleSetups,
            DiagnosticsResources.ProtobufPolymorphismMultipleSetups);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismSetupWrongArity"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismSetupWrongArity
        => protobufPolymorphismSetupWrongArity ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismSetupWrongArity,
            DiagnosticsResources.ProtobufPolymorphismSetupWrongArity);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismCasePropertyNotFound"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismCasePropertyNotFound
        => protobufPolymorphismCasePropertyNotFound ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismCasePropertyNotFound,
            DiagnosticsResources.ProtobufPolymorphismCasePropertyNotFound);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismCasePropertyNotOneofCaseEnum"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismCasePropertyNotOneofCaseEnum
        => protobufPolymorphismCasePropertyNotOneofCaseEnum ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismCasePropertyNotOneofCaseEnum,
            DiagnosticsResources.ProtobufPolymorphismCasePropertyNotOneofCaseEnum);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismCaseNameUnknown"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismCaseNameUnknown
        => protobufPolymorphismCaseNameUnknown ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismCaseNameUnknown,
            DiagnosticsResources.ProtobufPolymorphismCaseNameUnknown);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismCaseNameNone"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismCaseNameNone
        => protobufPolymorphismCaseNameNone ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismCaseNameNone,
            DiagnosticsResources.ProtobufPolymorphismCaseNameNone);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismScalarArm"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismScalarArm
        => protobufPolymorphismScalarArm ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismScalarArm,
            DiagnosticsResources.ProtobufPolymorphismScalarArm);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismDuplicateArm"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismDuplicateArm
        => protobufPolymorphismDuplicateArm ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismDuplicateArm,
            DiagnosticsResources.ProtobufPolymorphismDuplicateArm);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismDefaultRequiresProtobufTarget"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismDefaultRequiresProtobufTarget
        => protobufPolymorphismDefaultRequiresProtobufTarget ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismDefaultRequiresProtobufTarget,
            DiagnosticsResources.ProtobufPolymorphismDefaultRequiresProtobufTarget);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismMapToCaseMissing"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismMapToCaseMissing
        => protobufPolymorphismMapToCaseMissing ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismMapToCaseMissing,
            DiagnosticsResources.ProtobufPolymorphismMapToCaseMissing);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismDefaultBehaviorUndefined"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismDefaultBehaviorUndefined
        => protobufPolymorphismDefaultBehaviorUndefined ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismDefaultBehaviorUndefined,
            DiagnosticsResources.ProtobufPolymorphismDefaultBehaviorUndefined);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismDefaultArgumentUnused"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismDefaultArgumentUnused
        => protobufPolymorphismDefaultArgumentUnused ??= BuildWarning(
            MappaDiagnosticsKind.ProtobufPolymorphismDefaultArgumentUnused,
            DiagnosticsResources.ProtobufPolymorphismDefaultArgumentUnused);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismClrTypeNotCompatible"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismClrTypeNotCompatible
        => protobufPolymorphismClrTypeNotCompatible ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismClrTypeNotCompatible,
            DiagnosticsResources.ProtobufPolymorphismClrTypeNotCompatible);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismArmPropertyNotWritable"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismArmPropertyNotWritable
        => protobufPolymorphismArmPropertyNotWritable ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismArmPropertyNotWritable,
            DiagnosticsResources.ProtobufPolymorphismArmPropertyNotWritable);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProtobufPolymorphismArmFormMismatch"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProtobufPolymorphismArmFormMismatch
        => protobufPolymorphismArmFormMismatch ??= BuildError(
            MappaDiagnosticsKind.ProtobufPolymorphismArmFormMismatch,
            DiagnosticsResources.ProtobufPolymorphismArmFormMismatch);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.InvalidTransformerSignature"/>.
    /// </summary>
    internal static DiagnosticDescriptor InvalidTransformerSignature
        => invalidTransformerSignature ??= BuildWarning(
            MappaDiagnosticsKind.InvalidTransformerSignature,
            DiagnosticsResources.InvalidTransformerSignature);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ApplyTransformerMethodNotFound"/>.
    /// </summary>
    internal static DiagnosticDescriptor ApplyTransformerMethodNotFound
        => applyTransformerMethodNotFound ??= BuildWarning(
            MappaDiagnosticsKind.ApplyTransformerMethodNotFound,
            DiagnosticsResources.ApplyTransformerMethodNotFound);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ApplyTransformerSourceMemberNotFound"/>.
    /// </summary>
    internal static DiagnosticDescriptor ApplyTransformerSourceMemberNotFound
        => applyTransformerSourceMemberNotFound ??= BuildError(
            MappaDiagnosticsKind.ApplyTransformerSourceMemberNotFound,
            DiagnosticsResources.ApplyTransformerSourceMemberNotFound);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ApplyTransformerClassTypeAndFieldNameConflict"/>.
    /// </summary>
    internal static DiagnosticDescriptor ApplyTransformerClassTypeAndFieldNameConflict
        => applyTransformerClassTypeAndFieldNameConflict ??= BuildWarning(
            MappaDiagnosticsKind.ApplyTransformerClassTypeAndFieldNameConflict,
            DiagnosticsResources.ApplyTransformerClassTypeAndFieldNameConflict);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodHasApplyTransformer"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodHasApplyTransformer
        => projectionMethodHasApplyTransformer ??= BuildError(
            MappaDiagnosticsKind.ProjectionMethodHasApplyTransformer,
            DiagnosticsResources.ProjectionMethodHasApplyTransformer);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.NullSubstituteModeMissingOrAmbiguous"/>.
    /// </summary>
    internal static DiagnosticDescriptor NullSubstituteModeMissingOrAmbiguous
        => nullSubstituteModeMissingOrAmbiguous ??= BuildError(
            MappaDiagnosticsKind.NullSubstituteModeMissingOrAmbiguous,
            DiagnosticsResources.NullSubstituteModeMissingOrAmbiguous);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.NullSubstituteInvokeMethodTypeAndSourceConflict"/>.
    /// </summary>
    internal static DiagnosticDescriptor NullSubstituteInvokeMethodTypeAndSourceConflict
        => nullSubstituteInvokeMethodTypeAndSourceConflict ??= BuildError(
            MappaDiagnosticsKind.NullSubstituteInvokeMethodTypeAndSourceConflict,
            DiagnosticsResources.NullSubstituteInvokeMethodTypeAndSourceConflict);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.TooManyNullSubstituteAttributesForTheSameTargetProperty"/>.
    /// </summary>
    internal static DiagnosticDescriptor TooManyNullSubstituteAttributesForTheSameTargetProperty
        => tooManyNullSubstituteAttributesForTheSameTargetProperty ??= BuildError(
            MappaDiagnosticsKind.TooManyNullSubstituteAttributesForTheSameTargetProperty,
            DiagnosticsResources.TooManyNullSubstituteAttributesForTheSameTargetProperty);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.NullSubstituteIgnoredBecauseAssignFromConstant"/>.
    /// </summary>
    internal static DiagnosticDescriptor NullSubstituteIgnoredBecauseAssignFromConstant
        => nullSubstituteIgnoredBecauseAssignFromConstant ??= BuildWarning(
            MappaDiagnosticsKind.NullSubstituteIgnoredBecauseAssignFromConstant,
            DiagnosticsResources.NullSubstituteIgnoredBecauseAssignFromConstant);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.ProjectionMethodHasNullSubstituteInvokeMethod"/>.
    /// </summary>
    internal static DiagnosticDescriptor ProjectionMethodHasNullSubstituteInvokeMethod
        => projectionMethodHasNullSubstituteInvokeMethod ??= BuildError(
            MappaDiagnosticsKind.ProjectionMethodHasNullSubstituteInvokeMethod,
            DiagnosticsResources.ProjectionMethodHasNullSubstituteInvokeMethod);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorClassIsNotPartial"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorClassIsNotPartial
        => mappaInterceptorClassIsNotPartial ??= BuildWarning(
            MappaDiagnosticsKind.MappaInterceptorClassIsNotPartial,
            DiagnosticsResources.MappaInterceptorClassIsNotPartial);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorClassIsStatic"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorClassIsStatic
        => mappaInterceptorClassIsStatic ??= BuildError(
            MappaDiagnosticsKind.MappaInterceptorClassIsStatic,
            DiagnosticsResources.MappaInterceptorClassIsStatic);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorWithArgumentMustBeObjectCreation"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorWithArgumentMustBeObjectCreation
        => mappaInterceptorWithArgumentMustBeObjectCreation ??= BuildError(
            MappaDiagnosticsKind.MappaInterceptorWithArgumentMustBeObjectCreation,
            DiagnosticsResources.MappaInterceptorWithArgumentMustBeObjectCreation);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorMultipleSettings"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorMultipleSettings
        => mappaInterceptorMultipleSettings ??= BuildWarning(
            MappaDiagnosticsKind.MappaInterceptorMultipleSettings,
            DiagnosticsResources.MappaInterceptorMultipleSettings);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorDuplicateStaticDependencyType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorDuplicateStaticDependencyType
        => mappaInterceptorDuplicateStaticDependencyType ??= BuildWarning(
            MappaDiagnosticsKind.MappaInterceptorDuplicateStaticDependencyType,
            DiagnosticsResources.MappaInterceptorDuplicateStaticDependencyType);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorDuplicateNonMultipleAttribute"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorDuplicateNonMultipleAttribute
        => mappaInterceptorDuplicateNonMultipleAttribute ??= BuildError(
            MappaDiagnosticsKind.MappaInterceptorDuplicateNonMultipleAttribute,
            DiagnosticsResources.MappaInterceptorDuplicateNonMultipleAttribute);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorUnsupportedFeature"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorUnsupportedFeature
        => mappaInterceptorUnsupportedFeature ??= BuildError(
            MappaDiagnosticsKind.MappaInterceptorUnsupportedFeature,
            DiagnosticsResources.MappaInterceptorUnsupportedFeature);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorQueryableNotSupported"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorQueryableNotSupported
        => mappaInterceptorQueryableNotSupported ??= BuildError(
            MappaDiagnosticsKind.MappaInterceptorQueryableNotSupported,
            DiagnosticsResources.MappaInterceptorQueryableNotSupported);

    /// <summary>
    /// Gets a descriptor for diagnostic <see cref="MappaDiagnosticsKind.MappaInterceptorInvalidMethodAttributeType"/>.
    /// </summary>
    internal static DiagnosticDescriptor MappaInterceptorInvalidMethodAttributeType
        => mappaInterceptorInvalidMethodAttributeType ??= BuildError(
            MappaDiagnosticsKind.MappaInterceptorInvalidMethodAttributeType,
            DiagnosticsResources.MappaInterceptorInvalidMethodAttributeType);

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