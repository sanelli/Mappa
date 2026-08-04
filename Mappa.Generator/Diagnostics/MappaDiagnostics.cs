// <copyright file="MappaDiagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Diagnostics;

/// <summary>
/// Diagnostics reported by the Mappa generator.
/// </summary>
internal static class MappaDiagnostics
{
    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> has an invalid number of
    /// parameters.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodHasInvalidNumberOfParameters(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodHasInvalidNumberOfParameters,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> require a second parameter
    /// of type <see cref="MappaContext"/>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodHasInvalidMappaContextParameter(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodHasInvalidMappaContextParameter,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> returns <c>void</c>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodIsVoid(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodIsVoid,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that the method described by syntax
    /// <paramref name="methodDeclarationSyntax"/> returns either <see cref="Task"/>,
    /// or <see cref="Task{T}"/>, or <see cref="ValueTask"/>, or <see cref="ValueTask{T}"/>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodReturnsTaskType(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodReturnsTaskType,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that a method cannot be generated because a
    /// mapping between two types has already been defined.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic DuplicatedMapping(MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.DuplicatedMapping,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString(),
            methodDeclarationSyntax.ParameterList.Parameters.First().Type?.ToFullString() ?? "unknown",
            methodDeclarationSyntax.ReturnType.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that a mapping cannot be identifier.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="location">The location of the mapping.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotIdentifyStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotIdentifyStrategy,
            location,
            sourceType.ToDisplayString(),
            targetType.ToDisplayString());

    /// <summary>
    /// Diagnostic to report the fact that multiple attributes are targeting the same property.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="propertyOrParameterName">The name of the property or constructor parameter for which multiple mapping attributes have been defined.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MultipleAttributesTargetTheSamePropertyOrParameter(MethodDeclarationSyntax methodDeclarationSyntax, string propertyOrParameterName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MultipleAttributesTargetTheSamePropertyOrParameter,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString(),
            propertyOrParameterName);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable method to invoke.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="propertyOrParameterName">The name of the property or constructor parameter for which multiple mapping attributes have been defined.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="typeName">The type on which the method is being looked for.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotDetectSuitableMethodToInvokeForParameter(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string propertyOrParameterName,
        string methodName,
        string typeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotDetectSuitableMethodToInvokeForParameter,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            typeName,
            propertyOrParameterName);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable type for the method to invoke.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="type">The type that cannot be found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotDetectType(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string type)
        => CannotDetectType(methodDeclarationSyntax.GetLocation(), type);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable type for the method to invoke.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="type">The type that cannot be found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotDetectType(
        Location? location,
        string type)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotDetectType,
            location,
            type);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable field or property.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="fieldName">The type that cannot be found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotFindFieldOrProperty(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string fieldName)
        => CannotFindFieldOrProperty(methodDeclarationSyntax.GetLocation(), fieldName);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable field or property.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="fieldName">The field or property that cannot be found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotFindFieldOrProperty(
        Location? location,
        string fieldName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotFindFieldOrProperty,
            location,
            fieldName);

    /// <summary>
    /// Diagnostic to report the fact that it is not use the <see cref="MappaAssignFromContextAttribute"/>
    /// because the method does not have a <see cref="MappaContext"/> parameter.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="fieldName">The type that cannot be found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotUseMappaAssignFromContextAttributeWithoutContextParameter(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string fieldName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotUseMappaAssignFromContextAttributeWithoutContextParameter,
            methodDeclarationSyntax.GetLocation(),
            fieldName);

    /// <summary>
    /// Diagnostic to report the fact that user defined settings are using the
    /// <see cref="CultureInfoSetting.UserDefined"/> culture
    /// but the culture name is not properly defined.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic UserDefinedCultureIsMissingCultureName(
        MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.UserDefinedCultureIsMissingCultureName,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report the fact that on mappa settings a format is specified
    /// but since a ParseExact(string,string) does not exist for the type
    /// <paramref name="typeName"/>, the format will be ignored.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="typeName">The name of the type to which we are mapping to.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ParseExactDoesNotAcceptOnlyFormat(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string typeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ParseExactDoesNotAcceptOnlyFormat,
            methodDeclarationSyntax?.GetLocation(),
            typeName);

    /// <summary>
    /// Diagnostic to report the fact that <paramref name="property"/>
    /// is not accessible and therefore cannot be mapped.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="typeSymbol">The type which the property belong to.</param>
    /// <param name="property">The property that cannot be accessed.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic PropertySetterIsNotAccessible(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        ITypeSymbol typeSymbol,
        IPropertySymbol property)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.PropertySetterIsNotAccessible,
            methodDeclarationSyntax?.GetLocation(),
            $"{typeSymbol.ToDisplayString()}.{property.Name}");

    /// <summary>
    /// Diagnostic to report the fact that <paramref name="methodDeclarationSyntax"/>
    /// has multiple attributes of type <see cref="MappaUsePropertyAttribute"/>
    /// targeting the same <paramref name="property"/>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="dependency">The name of the method.</param>
    /// <param name="property">The property (or parameter) that cannot be accessed.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic TooManyUsePropertyAttributesForTheSameTargetProperty(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string dependency,
        string property)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.TooManyUsePropertyAttributesForTheSameTargetProperty,
            methodDeclarationSyntax?.GetLocation(),
            dependency,
            property);

    /// <summary>
    /// Diagnostic to report that a dependency (identified via <see cref="MappaDependencyAttribute"/>
    /// or <see cref="MappaStaticDependencyAttribute"/>) does not provide any viable method
    /// that could be used for mapping.
    /// </summary>
    /// <param name="syntaxNode">The syntax element.</param>
    /// <param name="dependency">The dependency.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic DependencyDoesNotProvideAnyViableMethod(
        SyntaxNode? syntaxNode,
        string dependency)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod,
            syntaxNode?.GetLocation(),
            dependency);

    /// <summary>
    /// Diagnostic to report that a non-required property cannot be mapped.
    /// </summary>
    /// <param name="syntaxNode">The syntax element.</param>
    /// <param name="parentType">The type the property lives on.</param>
    /// <param name="property">The property that cannot be mapped.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotMapNonRequiredProperty(
        SyntaxNode? syntaxNode,
        ITypeSymbol parentType,
        IPropertySymbol property)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotMapNonRequiredProperty,
            syntaxNode?.GetLocation(),
            parentType.ToDisplayString(),
            property.Name);

    /// <summary>
    /// Diagnostic to report that an explicit target type does not derive or implement
    /// the method target type.
    /// </summary>
    /// <param name="explicitTargetType">The explicit target type.</param>
    /// <param name="mapMethodTargetType">The map method target type.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ExplicitTargetTypeDoesNotDeriveMapMethodTargetType(
        ITypeSymbol explicitTargetType,
        ITypeSymbol mapMethodTargetType,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ExplicitTargetTypeDoesNotDeriveMapMethodTargetType,
            location,
            explicitTargetType.ToDisplayString(),
            mapMethodTargetType.ToDisplayString());

    /// <summary>
    /// Diagnostic to report that the method to invoke is not correctly
    /// identified.
    /// </summary>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MethodToInvokeUndefined(Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MethodToInvokeUndefined,
            location);

    /// <summary>
    /// Diagnostic to report the fact that it is not possible identify a suitable method to invoke.
    /// </summary>
    /// <param name="typeName">The type on which the method is being looked for.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotIdentifySuitableMethodToInvoke(
        string typeName,
        string methodName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotIdentifySuitableMethodToInvoke,
            location,
            methodName,
            typeName);

    /// <summary>
    /// Diagnostic to report the fact that the type must be an exception.
    /// </summary>
    /// <param name="typeName">The type that must be an exception.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic TypeMustBeAnException(
        string typeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.TypeMustBeAnException,
            location,
            typeName);

    /// <summary>
    /// Diagnostic to report the fact that the type must be a concrete type.
    /// </summary>
    /// <param name="typeName">The type that must be a concrete type.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic TypeMustBeConcrete(
        string typeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.TypeMustBeConcrete,
            location,
            typeName);

    /// <summary>
    /// Diagnostic to report the fact that the type must have a constructor with no parameters
    /// or a constructor with a single string parameter.
    /// </summary>
    /// <param name="typeName">The type that must be a concrete type.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter(
        string typeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.TypeMustHaveAConstructorWithNoParametersOrAConstructorWithOneStringParameter,
            location,
            typeName);

    /// <summary>
    /// Diagnostic to report the fact that the <see cref="MappaTypeMappingDefaultAttribute"/>
    /// is <see cref="MappaTypeMappingDefaultBehavior.Undefined"/>.
    /// </summary>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaTypeDefaultBehaviorUndefined(
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaTypeDefaultBehaviorUndefined,
            location);

    /// <summary>
    /// Diagnostic to report the fact that the <see cref="MappaTypeMappingDefaultAttribute"/>
    /// specify a type that will not be used.
    /// </summary>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaTypeMappingDefaultAttributeUnusedType(
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaTypeMappingDefaultAttributeUnusedType,
            location);

    /// <summary>
    /// Diagnostic to report the fact that multiple <see cref="MappaTypeMappingDefaultAttribute"/>
    /// have the same source type.
    /// </summary>
    /// <param name="typeName">The name of the source type.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaTypeMappingAttributeHaveTheSameSourceType(
        string typeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaTypeMappingAttributeHaveTheSameSourceType,
            location,
            typeName);

    /// <summary>
    /// Diagnostic to report the fact <see cref="MappaTypeMappingDefaultAttribute"/>
    /// have the same source type of the method being mapped.
    /// </summary>
    /// <param name="typeName">The name of the source type.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaTypeMappingAttributeMapsSourceType(
        string typeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaTypeMappingAttributeMapsSourceType,
            location,
            typeName);

    /// <summary>
    /// Diagnostic to report the fact that <see cref="MappaTypeMappingDefaultAttribute"/>
    /// source type does not implement nor is derived from the source type of the method
    /// being mapped.
    /// </summary>
    /// <param name="attributeTypeName">The name of the source type in the attribute.</param>
    /// <param name="mapMethodTypeName">The name of the source type in the method.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType(
        string attributeTypeName,
        string mapMethodTypeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaTypeMappingAttributeSourceTypeNotDeriveOrImplementMapMethodSourceType,
            location,
            attributeTypeName,
            mapMethodTypeName);

    /// <summary>
    /// Diagnostic to report the fact that <see cref="MappaTypeMappingDefaultAttribute"/>
    /// target type does not implement nor is derived from the target type of the method
    /// being mapped.
    /// </summary>
    /// <param name="attributeTypeName">The name of the target type in the attribute.</param>
    /// <param name="mapMethodTypeName">The name of the target type in the method.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType(
        string attributeTypeName,
        string mapMethodTypeName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaTypeMappingAttributeTargetTypeNotDeriveOrImplementMapMethodTargetType,
            location,
            attributeTypeName,
            mapMethodTypeName);

    /// <summary>
    /// Diagnostic to report the fact that a field or a property needs to be static to be using in the expected context.
    /// </summary>
    /// <param name="fieldOrPropertyName">The name of the field or property.</param>
    /// <param name="location">The location of the diagnostic.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic FieldOrPropertyMustBeStatic(
        string fieldOrPropertyName,
        Location? location)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.FieldOrPropertyMustBeStatic,
            location,
            fieldOrPropertyName);

    /// <summary>
    /// Diagnostic to report that a <see cref="MappaUsePropertyAttribute"/> source property
    /// will not be used because another mapping attribute targets the same member.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="targetName">The target property or constructor parameter name.</param>
    /// <param name="sourcePropertyName">The source property name from MappaUseProperty.</param>
    /// <param name="conflictingAttributeName">The name of the conflicting attribute.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaUsePropertySourcePropertyWillNotBeUsed(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string targetName,
        string sourcePropertyName,
        string conflictingAttributeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaUsePropertySourcePropertyWillNotBeUsed,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            targetName,
            sourcePropertyName,
            conflictingAttributeName);

    /// <summary>
    /// Diagnostic to report that a <see cref="MappaUsePropertyAttribute"/> source property
    /// will not be used because the invoked method does not require it.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="targetName">The target property or constructor parameter name.</param>
    /// <param name="sourcePropertyName">The source property name from MappaUseProperty.</param>
    /// <param name="invokeMethodName">The name of the invoked method.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaUsePropertyNotUsedByInvokeMethod(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string targetName,
        string sourcePropertyName,
        string invokeMethodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaUsePropertyNotUsedByInvokeMethod,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            targetName,
            sourcePropertyName,
            invokeMethodName);

    /// <summary>
    /// Diagnostic to report that a mapping attribute targets a property or constructor
    /// parameter that does not exist on the target type.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="attributeName">The name of the mapping attribute.</param>
    /// <param name="targetName">The target property or constructor parameter name.</param>
    /// <param name="targetTypeName">The display name of the target type.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappingAttributeTargetPropertyOrParameterDoesNotExist(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string attributeName,
        string targetName,
        string targetTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            attributeName,
            targetName,
            targetTypeName);

    /// <summary>
    /// Diagnostic to report the fact that <paramref name="methodDeclarationSyntax"/>
    /// has multiple attributes of type <see cref="MappaIgnoreTargetPropertyAttribute"/>
    /// targeting the same <paramref name="property"/>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="property">The property that is targeted by multiple ignore attributes.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string property)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.TooManyMappaIgnoreTargetPropertyAttributesForTheSameTargetProperty,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            property);

    /// <summary>
    /// Diagnostic to report the fact that the target member for <see cref="MappaAssignToContextAttribute"/>
    /// does not exist or is not accessible.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="contextKey">The context key.</param>
    /// <param name="targetMemberName">The target member name.</param>
    /// <param name="targetTypeName">The display name of the target type.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string contextKey,
        string targetMemberName,
        string targetTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            contextKey,
            targetMemberName,
            targetTypeName);

    /// <summary>
    /// Diagnostic to report the fact that <see cref="MappaAssignToContextAttribute"/>
    /// is ignored because the method does not have a <see cref="MappaContext"/> parameter.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="contextKey">The context key.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic CannotUseMappaAssignToContextAttributeWithoutContextParameter(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string contextKey)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotUseMappaAssignToContextAttributeWithoutContextParameter,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            contextKey);

    /// <summary>
    /// Diagnostic to report the fact that multiple <see cref="MappaAssignToContextAttribute"/>
    /// attributes on the same method use the same context key.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="contextKey">The duplicated context key.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MultipleMappaAssignToContextAttributesUseTheSameContextKey(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string contextKey)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MultipleMappaAssignToContextAttributesUseTheSameContextKey,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            contextKey);

    /// <summary>
    /// Diagnostic to report that a <see cref="MappaSettingsAttribute"/> style property
    /// has an integer value that is not a valid enum combination.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="propertyName">The name of the style property.</param>
    /// <param name="value">The invalid style value.</param>
    /// <param name="enumTypeName">The display name of the enum type.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic InvalidMappaSettingsStyleValue(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string propertyName,
        int value,
        string enumTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.InvalidMappaSettingsStyleValue,
            methodDeclarationSyntax?.GetLocation(),
            propertyName,
            value,
            enumTypeName);

    /// <summary>
    /// Diagnostic to report that not all source enum members can be mapped to the target enum by name.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="sourceEnumTypeName">The display name of the source enum type.</param>
    /// <param name="targetEnumTypeName">The display name of the target enum type.</param>
    /// <param name="unmappedMemberNames">The formatted list of unmapped source enum member names.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic NotAllSourceEnumMembersCanBeMapped(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string sourceEnumTypeName,
        string targetEnumTypeName,
        string unmappedMemberNames)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
            methodDeclarationSyntax?.GetLocation(),
            sourceEnumTypeName,
            targetEnumTypeName,
            unmappedMemberNames);

    /// <summary>
    /// Diagnostic to report that an enum member is missing a Description attribute.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="enumTypeName">The display name of the enum type.</param>
    /// <param name="missingMemberNames">The formatted list of member names missing a Description attribute.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMemberMissingDescription(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string enumTypeName,
        string missingMemberNames)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMemberMissingDescription,
            methodDeclarationSyntax?.GetLocation(),
            enumTypeName,
            missingMemberNames);

    /// <summary>
    /// Diagnostic to report that enum mapping is ambiguous.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="details">The ambiguity details.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic AmbiguousEnumMap(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string details)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.AmbiguousEnumMap,
            methodDeclarationSyntax?.GetLocation(),
            details);

    /// <summary>
    /// Diagnostic to report that invoke method resolution is ambiguous.
    /// </summary>
    /// <param name="location">The location of the diagnostic.</param>
    /// <param name="details">The ambiguity details.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic AmbiguousInvokeMethodResolution(
        Location? location,
        string details)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.AmbiguousInvokeMethodResolution,
            location,
            details);

    /// <summary>
    /// Diagnostic to report that a mapping attribute source property path is shorter than the target property path.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="sourcePropertyPath">The source property path.</param>
    /// <param name="targetPropertyPath">The target property path.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string attributeName,
        string sourcePropertyPath,
        string targetPropertyPath)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappingAttributeSourcePropertyPathIsShorterThanTargetPropertyPath,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            attributeName,
            sourcePropertyPath,
            targetPropertyPath);

    /// <summary>
    /// Diagnostic to report that a mapping attribute source property path segment does not exist.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="sourcePropertyPath">The source property path.</param>
    /// <param name="missingSegment">The missing segment.</param>
    /// <param name="sourceTypeName">The source type display name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappingAttributeSourcePropertyPathSegmentDoesNotExist(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string attributeName,
        string sourcePropertyPath,
        string missingSegment,
        string sourceTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappingAttributeSourcePropertyPathSegmentDoesNotExist,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            attributeName,
            sourcePropertyPath,
            missingSegment,
            sourceTypeName);

    /// <summary>
    /// Diagnostic to report that a before-map or after-map hook method cannot be resolved.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="mapMethodName">The mapping method name.</param>
    /// <param name="hookKind">The hook kind.</param>
    /// <param name="hookMethodName">The hook method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic HookMethodNotFound(
        Location? location,
        string mapMethodName,
        string hookKind,
        string hookMethodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.HookMethodNotFound,
            location,
            mapMethodName,
            hookKind,
            hookMethodName);

    /// <summary>
    /// Diagnostic to report that the same hook method is registered at class and method scope.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="mapMethodName">The mapping method name.</param>
    /// <param name="hookKind">The hook kind.</param>
    /// <param name="hookMethodName">The resolved hook method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic DuplicateMapHookRegistration(
        Location? location,
        string mapMethodName,
        string hookKind,
        string hookMethodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.DuplicateMapHookRegistration,
            location,
            mapMethodName,
            hookKind,
            hookMethodName);

    /// <summary>
    /// Diagnostic to report that an enum mapping configuration attribute references an enum
    /// which is not part of the current mapping.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="attributeName">The name of the offending attribute.</param>
    /// <param name="enumTypeName">The enum type referenced by the attribute.</param>
    /// <param name="sourceTypeName">The source type of the mapping.</param>
    /// <param name="targetTypeName">The target type of the mapping.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMapAttributeEnumTypeMismatch(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string attributeName,
        string enumTypeName,
        string sourceTypeName,
        string targetTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMapAttributeEnumTypeMismatch,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            attributeName,
            enumTypeName,
            sourceTypeName,
            targetTypeName);

    /// <summary>
    /// Diagnostic to report that two or more enum member mappings conflict with each other.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="enumTypeName">The enum type being configured.</param>
    /// <param name="details">The clash details.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMapMemberMappingClash(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string enumTypeName,
        string details)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMapMemberMappingClash,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            enumTypeName,
            details);

    /// <summary>
    /// Diagnostic to report that an ignored enum member is also configured by an explicit member mapping.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="enumTypeName">The enum type being configured.</param>
    /// <param name="enumMemberName">The conflicting enum member name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMapIgnoreConflictsWithMemberMapping(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string enumTypeName,
        string enumMemberName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMapIgnoreConflictsWithMemberMapping,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            enumTypeName,
            enumMemberName);

    /// <summary>
    /// Diagnostic to report that a default enum mapping behaviour requires a default value.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="enumTypeName">The enum type being configured.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMapDefaultBehaviorRequiresDefaultValue(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string enumTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMapDefaultBehaviorRequiresDefaultValue,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            enumTypeName);

    /// <summary>
    /// Diagnostic to report that the default value provided for an enum mapping
    /// is not compatible with the target type of the mapping.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="enumTypeName">The enum type being configured.</param>
    /// <param name="targetTypeName">The target type of the mapping.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMapDefaultValueConstructorMismatch(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string enumTypeName,
        string targetTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMapDefaultValueConstructorMismatch,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            enumTypeName,
            targetTypeName);

    /// <summary>
    /// Diagnostic to report that the default value provided for an enum mapping will not be used.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="enumTypeName">The enum type being configured.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic EnumMapDefaultAttributeUnusedDefaultValue(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string enumTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.EnumMapDefaultAttributeUnusedDefaultValue,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            enumTypeName);

    /// <summary>
    /// Diagnostic to report that too many default enum mapping attributes have been applied
    /// to a map method whose source or return type is an enum.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="numberOfAttributes">The number of attributes found.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic TooManyEnumMapDefaultAttributesOnDirectEnumMap(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        int numberOfAttributes)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.TooManyEnumMapDefaultAttributesOnDirectEnumMap,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            numberOfAttributes);

    /// <summary>
    /// Diagnostic to report that multiple default enum mapping attributes target the same enum.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="enumTypeName">The duplicated enum type.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic DuplicateEnumMapDefaultAttribute(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName,
        string enumTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.DuplicateEnumMapDefaultAttribute,
            methodDeclarationSyntax?.GetLocation(),
            methodName,
            enumTypeName);

    /// <summary>
    /// Diagnostic to report that a queryable projection map method declares before or after map hooks.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionMethodHasBeforeOrAfterMapHooks(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionMethodHasBeforeOrAfterMapHooks,
            methodDeclarationSyntax?.GetLocation(),
            methodName);

    /// <summary>
    /// Diagnostic to report that a queryable projection map method declares a <see cref="MappaContext"/> parameter.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionMethodHasMappaContextParameter(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionMethodHasMappaContextParameter,
            methodDeclarationSyntax?.GetLocation(),
            methodName);

    /// <summary>
    /// Diagnostic to report that a queryable projection element mapping contains an unsupported construct.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="constructDescription">The unsupported construct description.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionMappingNotSupported(
        Location? location,
        string methodName,
        string constructDescription)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionMappingNotSupported,
            location,
            methodName,
            constructDescription);

    /// <summary>
    /// Diagnostic to report that a queryable projection invokes a method that cannot be inlined.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="invokedMethodName">The invoked method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionInvokeMethodNotInlinable(
        Location? location,
        string methodName,
        string invokedMethodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionInvokeMethodNotInlinable,
            location,
            methodName,
            invokedMethodName);

    /// <summary>
    /// Diagnostic to report that a queryable projection involves a nested IQueryable property.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="propertyName">The nested property name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionNestedQueryableNotSupported(
        Location? location,
        string methodName,
        string propertyName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionNestedQueryableNotSupported,
            location,
            methodName,
            propertyName);

    /// <summary>
    /// Diagnostic to report that a queryable projection uses an enum mapping strategy that may not translate.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionEnumStrategyNotSupported(
        Location? location,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionEnumStrategyNotSupported,
            location,
            methodName);

    /// <summary>
    /// Diagnostic to report that an IQueryable source is mapped to a concrete collection.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic IQueryableMappedAsCollection(
        Location? location,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.IQueryableMappedAsCollection,
            location,
            methodName);

    /// <summary>
    /// Diagnostic to report that multiple object factories target the same type.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="targetTypeName">The duplicated target type.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic DuplicateObjectFactoryForTargetType(
        Location? location,
        string methodName,
        string targetTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.DuplicateObjectFactoryForTargetType,
            location,
            methodName,
            targetTypeName);

    /// <summary>
    /// Diagnostic to report that an object factory method cannot be identified.
    /// </summary>
    /// <param name="location">The diagnostic location.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <param name="targetTypeName">The factory target type.</param>
    /// <param name="factoryMethodName">The factory method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ObjectFactoryMethodNotFound(
        Location? location,
        string methodName,
        string targetTypeName,
        string factoryMethodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ObjectFactoryMethodNotFound,
            location,
            methodName,
            targetTypeName,
            factoryMethodName);

    /// <summary>
    /// Diagnostic to report that a queryable projection map method declares object factories.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionMethodHasObjectFactory(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionMethodHasObjectFactory,
            methodDeclarationSyntax?.GetLocation(),
            methodName);

    /// <summary>
    /// Diagnostic to report that a property required by <see cref="MappaMustMapTargetPropertyAttribute"/> cannot be mapped.
    /// </summary>
    /// <param name="syntaxNode">The syntax element.</param>
    /// <param name="parentType">The type the property lives on.</param>
    /// <param name="property">The property that cannot be mapped.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MustMapTargetPropertyWasNotMapped(
        SyntaxNode? syntaxNode,
        ITypeSymbol parentType,
        IPropertySymbol property)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MustMapTargetPropertyWasNotMapped,
            syntaxNode?.GetLocation(),
            parentType.ToDisplayString(),
            property.Name);

    /// <summary>
    /// Diagnostic to report that <see cref="MappaMustMapTargetPropertyAttribute"/> lists a required property.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="propertyName">The required property name listed in the attribute.</param>
    /// <param name="targetTypeName">The display name of the target type.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaMustMapTargetPropertyListsRequiredProperty(
        MethodDeclarationSyntax methodDeclarationSyntax,
        string methodName,
        string propertyName,
        string targetTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaMustMapTargetPropertyListsRequiredProperty,
            methodDeclarationSyntax.GetLocation(),
            methodName,
            propertyName,
            targetTypeName);

    /// <summary>
    /// Diagnostic to report that inaccessible-member attributes require <c>UnsafeAccessor</c> support.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic UnsafeAccessorNotSupported(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.UnsafeAccessorNotSupported,
            methodDeclarationSyntax?.GetLocation(),
            methodName);

    /// <summary>
    /// Diagnostic to report that both allow flags on the target inaccessible-member attribute are <c>false</c>.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic AllowInaccessibleTargetMembersDisabledAll(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.AllowInaccessibleTargetMembersDisabledAll,
            methodDeclarationSyntax?.GetLocation(),
            methodName);

    /// <summary>
    /// Diagnostic to report that a queryable projection map method declares inaccessible-member attributes.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <param name="methodName">The mapping method name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ProjectionMethodHasAllowInaccessibleMembers(
        MethodDeclarationSyntax? methodDeclarationSyntax,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ProjectionMethodHasAllowInaccessibleMembers,
            methodDeclarationSyntax?.GetLocation(),
            methodName);

    /// <summary>
    /// Diagnostic to report that <see cref="Mappa.Attributes.MappaDependencyInjectionAttribute"/>
    /// is applied to a non-<c>partial</c> class.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class declaration syntax.</param>
    /// <param name="className">The class name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaDependencyInjectionClassIsNotPartial(
        ClassDeclarationSyntax? classDeclarationSyntax,
        string className)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaDependencyInjectionClassIsNotPartial,
            classDeclarationSyntax?.GetLocation(),
            className);

    /// <summary>
    /// Diagnostic to report that both <see cref="Mappa.Attributes.MappaAttribute"/> and
    /// <see cref="Mappa.Attributes.MappaDependencyInjectionAttribute"/> are applied to the same class.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class declaration syntax.</param>
    /// <param name="className">The class name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaAndMappaDependencyInjectionBothApplied(
        ClassDeclarationSyntax? classDeclarationSyntax,
        string className)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaAndMappaDependencyInjectionBothApplied,
            classDeclarationSyntax?.GetLocation(),
            className);

    /// <summary>
    /// Diagnostic to report that a mapper has no eligible interfaces for DI registration.
    /// </summary>
    /// <param name="classDeclarationSyntax">The registrar class declaration syntax.</param>
    /// <param name="mapperTypeName">The mapper type name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaDependencyInjectionMapperHasNoEligibleInterfaces(
        ClassDeclarationSyntax? classDeclarationSyntax,
        string mapperTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaDependencyInjectionMapperHasNoEligibleInterfaces,
            classDeclarationSyntax?.GetLocation(),
            mapperTypeName);

    /// <summary>
    /// Diagnostic to report that a static mapper cannot be registered with dependency injection.
    /// </summary>
    /// <param name="classDeclarationSyntax">The registrar class declaration syntax.</param>
    /// <param name="mapperTypeName">The static mapper type name.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappaDependencyInjectionStaticMapperSkipped(
        ClassDeclarationSyntax? classDeclarationSyntax,
        string mapperTypeName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappaDependencyInjectionStaticMapperSkipped,
            classDeclarationSyntax?.GetLocation(),
            mapperTypeName);

    /// <summary>
    /// Diagnostic to report that reference handling is requested on a root map method
    /// that does not declare a <see cref="MappaContext"/> parameter.
    /// </summary>
    /// <param name="methodDeclarationSyntax">The method declaration syntax.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ReferenceHandlingRootMapWithoutMappaContext(
        MethodDeclarationSyntax methodDeclarationSyntax)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ReferenceHandlingRootMapWithoutMappaContext,
            methodDeclarationSyntax.GetLocation(),
            methodDeclarationSyntax.Identifier.ToFullString());

    /// <summary>
    /// Diagnostic to report that an invoked map method does not declare a
    /// <see cref="MappaContext"/> parameter while reference handling is active.
    /// </summary>
    /// <param name="location">The location of the mapping that invokes the method.</param>
    /// <param name="methodName">The name of the invoked map method.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic ReferenceHandlingNestedMapWithoutMappaContext(
        Location? location,
        string methodName)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.ReferenceHandlingNestedMapWithoutMappaContext,
            location,
            methodName);

    /// <summary>
    /// Diagnostic to report that strategy discovery exceeded the maximum compile-time depth.
    /// </summary>
    /// <param name="location">The location associated with the mapping.</param>
    /// <param name="sourceType">The source type being mapped.</param>
    /// <param name="targetType">The target type being mapped.</param>
    /// <param name="maxCompileTimeDepth">The effective maximum compile-time depth.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MaxCompileTimeDepthReached(
        Location? location,
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        short maxCompileTimeDepth)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MaxCompileTimeDepthReached,
            location,
            sourceType.ToDisplayString(),
            targetType.ToDisplayString(),
            maxCompileTimeDepth);

    /// <summary>
    /// Diagnostic to report that strategy discovery detected a compile-time mapping cycle.
    /// </summary>
    /// <param name="location">The location associated with the mapping.</param>
    /// <param name="sourceType">The source type being mapped.</param>
    /// <param name="targetType">The target type being mapped.</param>
    /// <returns>The diagnostic.</returns>
    internal static Diagnostic MappingCycleDetected(
        Location? location,
        ITypeSymbol sourceType,
        ITypeSymbol targetType)
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.MappingCycleDetected,
            location,
            sourceType.ToDisplayString(),
            targetType.ToDisplayString());
}