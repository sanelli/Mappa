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
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotDetectType,
            methodDeclarationSyntax.GetLocation(),
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
        => Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotFindFieldOrProperty,
            methodDeclarationSyntax.GetLocation(),
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
}