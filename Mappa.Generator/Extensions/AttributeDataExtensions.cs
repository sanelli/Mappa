// <copyright file="AttributeDataExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Globalization;

using Mappa.Attributes;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeData"/>.
/// </summary>
internal static class AttributeDataExtensions
{
    private const string MappaAfterMapAttributeFullName = "Mappa.Attributes.MappaAfterMapAttribute";
    private const string MappaBeforeMapAttributeFullName = "Mappa.Attributes.MappaBeforeMapAttribute";
    private const string MappaObjectFactoryAttributeFullName = "Mappa.Attributes.MappaObjectFactoryAttribute";
    private const string MappaInvokeMethodAttributeFullName = "Mappa.Attributes.MappaInvokeMethodAttribute";
    private const string MappaIgnoreAttributeFullName = "Mappa.Attributes.MappaIgnoreAttribute";
    private const string MappaAssignFromContextAttributeFullName = "Mappa.Attributes.MappaAssignFromContextAttribute";
    private const string MappaAssignToContextAttributeFullName = "Mappa.Attributes.MappaAssignToContextAttribute";
    private const string MappaSettingsAttributeFullName = "Mappa.Attributes.MappaSettingsAttribute";
    private const string MappaUsePropertyAttributeFullName = "Mappa.Attributes.MappaUsePropertyAttribute";
    private const string MappaDependencyAttributeFullName = "Mappa.Attributes.MappaDependencyAttribute";
    private const string MappaStaticDependencyAttributeFullName = "Mappa.Attributes.MappaStaticDependencyAttribute";
    private const string MappaAssignFromConstantAttributeFullName = "Mappa.Attributes.MappaAssignFromConstantAttribute";
    private const string MappaIgnoreTargetPropertyAttributeFullName = "Mappa.Attributes.MappaIgnoreTargetPropertyAttribute";
    private const string MappaMustMapTargetPropertyAttributeFullName = "Mappa.Attributes.MappaMustMapTargetPropertyAttribute";
    private const string MappaAllowInaccessibleSourceMembersAttributeFullName = "Mappa.Attributes.MappaAllowInaccessibleSourceMembersAttribute";
    private const string MappaAllowInaccessibleTargetMembersAttributeFullName = "Mappa.Attributes.MappaAllowInaccessibleTargetMembersAttribute";
    private const string MappaTypeMappingAttributeFullName = "Mappa.Attributes.MappaTypeMappingAttribute";
    private const string MappaTypeMappingDefaultAttributeFullName = "Mappa.Attributes.MappaTypeMappingDefaultAttribute";
    private const string MappaMapEnumMemberAttributeFullName = "Mappa.Attributes.MappaMapEnumMemberAttribute`1";
    private const string MappaMapEnumMemberToEnumAttributeFullName = "Mappa.Attributes.MappaMapEnumMemberAttribute`2";
    private const string MappaMapEnumIgnoreAttributeFullName = "Mappa.Attributes.MappaMapEnumIgnoreAttribute`1";
    private const string MappaMapEnumDefaultAttributeFullName = "Mappa.Attributes.MappaMapEnumDefaultAttribute`1";
    private const string MappaAttributeFullName = "Mappa.Attributes.MappaAttribute";
    private const string MappaDependencyInjectionAttributeFullName = "Mappa.Attributes.MappaDependencyInjectionAttribute";

    /// <summary>
    /// Gets the <see cref="MappaMapEnumMemberAttribute{TEnum}"/> and
    /// <see cref="MappaMapEnumMemberAttribute{TEnum, TOtherEnum}"/> declarations applied to the method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed declarations in declaration order.</returns>
    internal static EnumMapMemberInfoAttribute[] GetEnumMapMemberAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var enumMemberAttributeSymbol = compilation.GetTypeByMetadataName(MappaMapEnumMemberAttributeFullName);
        var enumMemberToEnumAttributeSymbol = compilation.GetTypeByMetadataName(MappaMapEnumMemberToEnumAttributeFullName);
        var results = new List<EnumMapMemberInfoAttribute>();

        foreach (var attributeData in attributes)
        {
            if (attributeData.AttributeClass is not { } attributeClass
                || attributeData.ConstructorArguments.Length != 2)
            {
                continue;
            }

            var constructorArguments = attributeData.ConstructorArguments;
            if (IsAttribute(attributeClass, enumMemberAttributeSymbol)
                && GetEnumTypeArgument(attributeClass, 1, 0) is { } enumType
                && GetEnumMemberName(constructorArguments[0]) is { } enumMemberName)
            {
                switch (constructorArguments[1].Value)
                {
                    case string stringValue:
                        results.Add(new EnumMapMemberInfoAttribute(enumType, enumMemberName, null, stringValue, null, null));
                        break;

                    case int integerValue:
                        results.Add(new EnumMapMemberInfoAttribute(enumType, enumMemberName, integerValue, null, null, null));
                        break;
                }

                continue;
            }

            if (IsAttribute(attributeClass, enumMemberToEnumAttributeSymbol)
                && GetEnumTypeArgument(attributeClass, 2, 0) is { } firstEnumType
                && GetEnumTypeArgument(attributeClass, 2, 1) is { } secondEnumType
                && GetEnumMemberName(constructorArguments[0]) is { } firstEnumMemberName
                && GetEnumMemberName(constructorArguments[1]) is { } secondEnumMemberName)
            {
                results.Add(new EnumMapMemberInfoAttribute(
                    firstEnumType,
                    firstEnumMemberName,
                    null,
                    null,
                    secondEnumType,
                    secondEnumMemberName));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaMapEnumIgnoreAttribute{TEnum}"/> declarations applied to the method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed declarations in declaration order.</returns>
    internal static EnumMapIgnoreInfoAttribute[] GetEnumMapIgnoreAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var enumIgnoreAttributeSymbol = compilation.GetTypeByMetadataName(MappaMapEnumIgnoreAttributeFullName);
        var results = new List<EnumMapIgnoreInfoAttribute>();

        foreach (var attributeData in attributes)
        {
            if (attributeData.AttributeClass is not { } attributeClass
                || !IsAttribute(attributeClass, enumIgnoreAttributeSymbol)
                || attributeData.ConstructorArguments.Length != 1
                || GetEnumTypeArgument(attributeClass, 1, 0) is not { } enumType
                || GetEnumMemberName(attributeData.ConstructorArguments[0]) is not { } enumMemberName)
            {
                continue;
            }

            results.Add(new EnumMapIgnoreInfoAttribute(enumType, enumMemberName));
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaMapEnumDefaultAttribute{TEnum}"/> declarations applied to the method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed declarations in declaration order.</returns>
    internal static EnumMapDefaultInfoAttribute[] GetEnumMapDefaultAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var enumDefaultAttributeSymbol = compilation.GetTypeByMetadataName(MappaMapEnumDefaultAttributeFullName);
        var results = new List<EnumMapDefaultInfoAttribute>();

        foreach (var attributeData in attributes)
        {
            if (attributeData.AttributeClass is not { } attributeClass
                || !IsAttribute(attributeClass, enumDefaultAttributeSymbol)
                || GetEnumTypeArgument(attributeClass, 1, 0) is not { } enumType)
            {
                continue;
            }

            var constructorArguments = attributeData.ConstructorArguments;
            if (constructorArguments.Length == 0
                || constructorArguments[0].Value is not int behaviorValue)
            {
                continue;
            }

            var behavior = (MappaMapEnumDefaultBehavior)behaviorValue;
            if (constructorArguments.Length == 1)
            {
                results.Add(new EnumMapDefaultInfoAttribute(enumType, behavior, null, null, null));
                continue;
            }

            var defaultValue = constructorArguments[1];
            if (defaultValue.Type is INamedTypeSymbol { TypeKind: TypeKind.Enum })
            {
                if (GetEnumMemberName(defaultValue) is { } enumDefaultMemberName)
                {
                    results.Add(new EnumMapDefaultInfoAttribute(enumType, behavior, enumDefaultMemberName, null, null));
                }

                continue;
            }

            switch (defaultValue.Value)
            {
                case string stringDefaultValue:
                    results.Add(new EnumMapDefaultInfoAttribute(enumType, behavior, null, null, stringDefaultValue));
                    break;

                case int integerDefaultValue:
                    results.Add(new EnumMapDefaultInfoAttribute(enumType, behavior, null, integerDefaultValue, null));
                    break;
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaTypeMappingDefaultAttribute"/> applied to the method (if any).
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaTypeMappingDefaultAttribute"/> (if any).</returns>
    internal static MappaTypeMappingDefaultAttribute? GetMappaTypeMappingDefaultAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaTypeMappingAttributeSymbol = compilation.GetTypeByMetadataName(MappaTypeMappingDefaultAttributeFullName);
        MappaTypeMappingDefaultAttribute? attribute = null;

        foreach (var constructorArguments in attributes
                     .Where(attributeData => SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, mappaTypeMappingAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            attribute = constructorArguments.Length switch
            {
                1 when constructorArguments[0].Value is string methodName
                    => new MappaTypeMappingDefaultAttribute(methodName),
                1 when constructorArguments[0].Value is int behavior
                    => new MappaTypeMappingDefaultAttribute((MappaTypeMappingDefaultBehavior)behavior),
                2 when constructorArguments[0].Value is INamedTypeSymbol invokeType && constructorArguments[1].Value is string methodName
                    => new MappaTypeMappingDefaultAttribute(new FakeType(invokeType.ToDisplayString()), methodName),
                2 when constructorArguments[0].Value is int behavior && constructorArguments[1].Value is INamedTypeSymbol type
                    => new MappaTypeMappingDefaultAttribute((MappaTypeMappingDefaultBehavior)behavior, new FakeType(type.ToDisplayString())),
                _ => null,
            };
        }

        return attribute;
    }

    /// <summary>
    /// Gets the <see cref="MappaTypeMappingAttribute"/>s applied to the method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaTypeMappingAttribute"/>s.</returns>
    internal static MappaTypeMappingAttribute[] GetTypeMappingAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaTypeMappingAttributeSymbol = compilation.GetTypeByMetadataName(MappaTypeMappingAttributeFullName);
        List<MappaTypeMappingAttribute> results = new();

        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaTypeMappingAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments[0].Value is INamedTypeSymbol targetType &&
                constructorArguments[1].Value is INamedTypeSymbol sourceType)
            {
                results.Add(new MappaTypeMappingAttribute(new FakeType(targetType.ToDisplayString()), new FakeType(sourceType.ToDisplayString())));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaInvokeMethodAttribute"/>s applied to the method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaInvokeMethodAttribute"/>s.</returns>
    internal static MappaInvokeMethodAttribute[] GetInvokeMethodAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaInvokeMethodAttributeFullName);
        List<MappaInvokeMethodAttribute> results = new();
        foreach (var attributeData in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol)))
        {
            MappaInvokeMethodAttribute? attribute = null;
            var constructorArguments = attributeData.ConstructorArguments;
            switch (constructorArguments.Length)
            {
                case 2: // (targetPropertyName, methodName)
                    {
                        if (constructorArguments[0].Value is string targetParameterName &&
                            constructorArguments[1].Value is string methodName)
                        {
                            attribute = new MappaInvokeMethodAttribute(targetParameterName, methodName);
                        }
                    }

                    break;

                case 3: // (targetPropertyName, classType, methodName) or (targetPropertyName, fieldName, methodName)
                    {
                        if (constructorArguments[0].Value is string targetParameterName &&
                            constructorArguments[2].Value is string methodName)
                        {
                            attribute = constructorArguments[1].Value switch
                            {
                                string fieldName => new MappaInvokeMethodAttribute(targetParameterName, fieldName, methodName),
                                INamedTypeSymbol classType => new MappaInvokeMethodAttribute(targetParameterName, new FakeType(classType.ToDisplayString()), methodName),
                                _ => null,
                            };
                        }
                    }

                    break;
            }

            if (attribute is null)
            {
                continue;
            }

            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (namedArgument.Key == nameof(MappaInvokeMethodAttribute.SourcePropertyName) &&
                    namedArgument.Value.Value is string sourcePropertyName)
                {
                    attribute.SourcePropertyName = sourcePropertyName;
                }
            }

            results.Add(attribute);
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaBeforeMapAttribute"/>s applied to a mapper class or mapping method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed attributes in declaration order.</returns>
    internal static MapHookAttributeData[] GetMappaBeforeMapAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
        => GetMapHookAttributes(
            attributes,
            compilation,
            MappaBeforeMapAttributeFullName);

    /// <summary>
    /// Gets the <see cref="MappaAfterMapAttribute"/>s applied to a mapper class or mapping method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed attributes in declaration order.</returns>
    internal static MapHookAttributeData[] GetMappaAfterMapAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
        => GetMapHookAttributes(
            attributes,
            compilation,
            MappaAfterMapAttributeFullName);

    /// <summary>
    /// Gets the <see cref="MappaObjectFactoryAttribute"/>s applied to a mapper class or mapping method.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed attributes in declaration order.</returns>
    internal static MappaObjectFactoryAttributeData[] GetMappaObjectFactoryAttributes(
        this ImmutableArray<AttributeData> attributes,
        Compilation compilation)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(MappaObjectFactoryAttributeFullName);
        var results = new List<MappaObjectFactoryAttributeData>();

        foreach (var attributeData in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)))
        {
            var constructorArguments = attributeData.ConstructorArguments;
            var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
            if (constructorArguments.Length == 2 &&
                constructorArguments[0].Value is INamedTypeSymbol targetType &&
                constructorArguments[1].Value is string methodName)
            {
                results.Add(new MappaObjectFactoryAttributeData(
                    targetType,
                    methodName,
                    null,
                    null,
                    location));
            }
            else if (constructorArguments.Length == 3 &&
                     constructorArguments[0].Value is INamedTypeSymbol factoryTargetType &&
                     constructorArguments[2].Value is string factoryMethodName)
            {
                switch (constructorArguments[1].Value)
                {
                    case string fieldName:
                        results.Add(new MappaObjectFactoryAttributeData(
                            factoryTargetType,
                            factoryMethodName,
                            null,
                            fieldName,
                            location));
                        break;
                    case INamedTypeSymbol classType:
                        results.Add(new MappaObjectFactoryAttributeData(
                            factoryTargetType,
                            factoryMethodName,
                            new FakeType(classType.ToDisplayString()),
                            null,
                            location));
                        break;
                }
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaAssignFromContextAttribute"/>s applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAssignFromContextAttribute"/> applied.</returns>
    internal static MappaAssignFromContextAttribute[] GetMappaAssignFromContextAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaAssignFromContextAttributeSymbol = compilation.GetTypeByMetadataName(MappaAssignFromContextAttributeFullName);
        List<MappaAssignFromContextAttribute> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaAssignFromContextAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments.Length == 2 &&
                constructorArguments[0].Value is string targetParameterName &&
                constructorArguments[1].Value is string itemName)
            {
                results.Add(new MappaAssignFromContextAttribute(targetParameterName, itemName));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaAssignToContextAttribute"/>s applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAssignToContextAttribute"/> applied.</returns>
    internal static MappaAssignToContextAttribute[] GetMappaAssignToContextAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaAssignToContextAttributeSymbol = compilation.GetTypeByMetadataName(MappaAssignToContextAttributeFullName);
        List<MappaAssignToContextAttribute> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaAssignToContextAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments.Length == 2 &&
                constructorArguments[0].Value is string contextKey &&
                constructorArguments[1].Value is string targetPropertyName)
            {
                results.Add(new MappaAssignToContextAttribute(contextKey, targetPropertyName));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaAssignFromConstantAttribute"/>s applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAssignFromConstantAttribute"/> applied.</returns>
    internal static MappaAssignFromConstantAttribute[] GetMappaAssignFromConstantAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaAssignFromConstantAttributeSymbol = compilation.GetTypeByMetadataName(MappaAssignFromConstantAttributeFullName);
        List<MappaAssignFromConstantAttribute> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaAssignFromConstantAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments.Length == 2
                && constructorArguments[0].Value is string targetParameterName)
            {
                results.Add(new MappaAssignFromConstantAttribute(targetParameterName, constructorArguments[1]));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Determines whether the <see cref="MappaDependencyAttribute"/> is applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the attribute is applied; otherwise, <c>false</c>.</returns>
    internal static bool HasMappaDependencyAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaDependencyAttributeSymbol = compilation.GetTypeByMetadataName(MappaDependencyAttributeFullName);
        return attributes.Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaDependencyAttributeSymbol));
    }

    /// <summary>
    /// Gets the <see cref="MappaIgnoreAttribute"/> applied to the method, if any.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaIgnoreAttribute"/> applied, or <c>null</c> if it does not exist.</returns>
    internal static MappaIgnoreAttribute? GetMappaIgnoreAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaIgnoreAttributeFullName);
        var exists = attributes
            .Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol));
        return exists ? new() : null;
    }

    /// <summary>
    /// Gets the <see cref="MappaSettingsAttribute"/> applied to the method, if any.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaSettingsAttribute"/> applied, or <c>null</c> if it does not exist.</returns>
    internal static MappaSettingsAttribute? GetMappaSettingsAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaSettingsAttributeFullName);
        var attributeData = attributes
            .SingleOrDefault(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol));
        if (attributeData is null)
        {
            return null;
        }

        var attribute = new MappaSettingsAttribute();
        foreach (var namedArgument in attributeData.NamedArguments)
        {
            switch (namedArgument.Key)
            {
                case nameof(MappaSettingsAttribute.DateTimeFormat) when namedArgument.Value.Value is string value:
                    attribute.DateTimeFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.DateTimeOffsetFormat) when namedArgument.Value.Value is string value:
                    attribute.DateTimeOffsetFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.DateOnlyFormat) when namedArgument.Value.Value is string value:
                    attribute.DateOnlyFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.TimeOnlyFormat) when namedArgument.Value.Value is string value:
                    attribute.TimeOnlyFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.DateTimeStyle):
                    attribute.DateTimeStyle = ReadDateTimeStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.DateTimeOffsetStyle):
                    attribute.DateTimeOffsetStyle = ReadDateTimeStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.DateOnlyStyle):
                    attribute.DateOnlyStyle = ReadDateTimeStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.TimeOnlyStyle):
                    attribute.TimeOnlyStyle = ReadDateTimeStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.GlobalDateTimeStyle):
                    attribute.GlobalDateTimeStyle = ReadDateTimeStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.TimeSpanFormat) when namedArgument.Value.Value is string value:
                    attribute.TimeSpanFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.GuidFormat) when namedArgument.Value.Value is string value:
                    attribute.GuidFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.ByteFormat) when namedArgument.Value.Value is string value:
                    attribute.ByteFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.SByteFormat) when namedArgument.Value.Value is string value:
                    attribute.SByteFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.ShortFormat) when namedArgument.Value.Value is string value:
                    attribute.ShortFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.UShortFormat) when namedArgument.Value.Value is string value:
                    attribute.UShortFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.IntFormat) when namedArgument.Value.Value is string value:
                    attribute.IntFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.UIntFormat) when namedArgument.Value.Value is string value:
                    attribute.UIntFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.LongFormat) when namedArgument.Value.Value is string value:
                    attribute.LongFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.ULongFormat) when namedArgument.Value.Value is string value:
                    attribute.ULongFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.DecimalFormat) when namedArgument.Value.Value is string value:
                    attribute.DecimalFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.FloatFormat) when namedArgument.Value.Value is string value:
                    attribute.FloatFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.DoubleFormat) when namedArgument.Value.Value is string value:
                    attribute.DoubleFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.ByteStyle):
                    attribute.ByteStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.SByteStyle):
                    attribute.SByteStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.ShortStyle):
                    attribute.ShortStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.UShortStyle):
                    attribute.UShortStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.IntStyle):
                    attribute.IntStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.UIntStyle):
                    attribute.UIntStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.LongStyle):
                    attribute.LongStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.ULongStyle):
                    attribute.ULongStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.DecimalStyle):
                    attribute.DecimalStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.FloatStyle):
                    attribute.FloatStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.DoubleStyle):
                    attribute.DoubleStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.GlobalNumberStyle):
                    attribute.GlobalNumberStyle = ReadNumberStyles(namedArgument.Value);
                    break;

                case nameof(MappaSettingsAttribute.CultureName) when namedArgument.Value.Value is string value:
                    attribute.CultureName = value;
                    break;

                case nameof(MappaSettingsAttribute.CultureInfoSetting) when namedArgument.Value.Value is int value:
                    attribute.CultureInfoSetting = (CultureInfoSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.ProtobufOptional) when namedArgument.Value.Value is int value:
                    attribute.ProtobufOptional = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.PragmaWarning) when namedArgument.Value.Value is int value:
                    attribute.PragmaWarning = (PragmaWarningSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.FastCollections) when namedArgument.Value.Value is int value:
                    attribute.FastCollections = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.ContainerCapacityConstructors) when namedArgument.Value.Value is int value:
                    attribute.ContainerCapacityConstructors = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.PreventEnumerableCount) when namedArgument.Value.Value is int value:
                    attribute.PreventEnumerableCount = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute) when namedArgument.Value.Value is int value:
                    attribute.PolymorphicMapMethodWithMatchingDefaultAttribute = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.CompatibleMapMethod) when namedArgument.Value.Value is int value:
                    attribute.CompatibleMapMethod = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.CaseInsensitivePropertyMap) when namedArgument.Value.Value is int value:
                    attribute.CaseInsensitivePropertyMap = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.IgnoreUnderscoreForPropertyMap) when namedArgument.Value.Value is int value:
                    attribute.IgnoreUnderscoreForPropertyMap = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.CaseInsensitiveEnumMap) when namedArgument.Value.Value is int value:
                    attribute.CaseInsensitiveEnumMap = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.EnumStringMapSetting) when namedArgument.Value.Value is int value:
                    attribute.EnumStringMapSetting = (EnumStringMapSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.EnumToEnumMapSetting) when namedArgument.Value.Value is int value:
                    attribute.EnumToEnumMapSetting = (EnumToEnumMapSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.IdentityMapDeepCopy) when namedArgument.Value.Value is int value:
                    attribute.IdentityMapDeepCopy = (IdentityMapDeepCopySetting)value;
                    break;

                case nameof(MappaSettingsAttribute.EnumerableConcreteType) when namedArgument.Value.Value is int value:
                    attribute.EnumerableConcreteType = (EnumerableConcreteTypeSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.DictionaryAssignment) when namedArgument.Value.Value is int value:
                    attribute.DictionaryAssignment = (DictionaryAssignmentSetting)value;
                    break;
            }
        }

        return attribute;

        static DateTimeStyles ReadDateTimeStyles(TypedConstant typedConstant)
        {
            return typedConstant.Value switch
            {
                null => MappaSettingsAttribute.UndefinedDateTimeStyle,
                int intValue => (DateTimeStyles)intValue,
                DateTimeStyles dateTimeStyles => dateTimeStyles,
                _ => MappaSettingsAttribute.UndefinedDateTimeStyle,
            };
        }

        static NumberStyles ReadNumberStyles(TypedConstant typedConstant)
        {
            return typedConstant.Value switch
            {
                null => MappaSettingsAttribute.UndefinedNumberStyle,
                int intValue => (NumberStyles)intValue,
                NumberStyles numberStyles => numberStyles,
                _ => MappaSettingsAttribute.UndefinedNumberStyle,
            };
        }
    }

    /// <summary>
    /// Gets the <see cref="MappaUsePropertyAttribute"/>s applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaUsePropertyAttribute"/> applied.</returns>
    internal static MappaUsePropertyAttribute[] GetMappaUsePropertyAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaUsePropertyAttributeSymbol = compilation.GetTypeByMetadataName(MappaUsePropertyAttributeFullName);
        List<MappaUsePropertyAttribute> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaUsePropertyAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments.Length == 2 &&
                constructorArguments[0].Value is string targetParameterName &&
                constructorArguments[1].Value is string sourcePropertyName)
            {
                results.Add(new MappaUsePropertyAttribute(targetParameterName, sourcePropertyName));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaIgnoreTargetPropertyAttribute"/>s applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaIgnoreTargetPropertyAttribute"/> applied.</returns>
    internal static MappaIgnoreTargetPropertyAttribute[] GetMappaIgnoreTargetPropertyAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaIgnoreTargetPropertyAttributeSymbol = compilation.GetTypeByMetadataName(MappaIgnoreTargetPropertyAttributeFullName);
        List<MappaIgnoreTargetPropertyAttribute> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaIgnoreTargetPropertyAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments.Length == 1 &&
                constructorArguments[0].Value is string targetPropertyName)
            {
                results.Add(new MappaIgnoreTargetPropertyAttribute(targetPropertyName));
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaMustMapTargetPropertyAttribute"/> applied, if any.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaMustMapTargetPropertyAttribute"/> applied, or <c>null</c>.</returns>
    internal static MappaMustMapTargetPropertyAttribute? GetMappaMustMapTargetPropertyAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaMustMapTargetPropertyAttributeSymbol = compilation.GetTypeByMetadataName(MappaMustMapTargetPropertyAttributeFullName);
        var attributeData = attributes
            .FirstOrDefault(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaMustMapTargetPropertyAttributeSymbol));
        if (attributeData is null)
        {
            return null;
        }

        var constructorArguments = attributeData.ConstructorArguments;
        if (constructorArguments.Length == 0
            || constructorArguments[0].Kind != TypedConstantKind.Array)
        {
            return new MappaMustMapTargetPropertyAttribute();
        }

        List<string> targetPropertyNames = new();
        foreach (var value in constructorArguments[0].Values)
        {
            if (value.Value is string { Length: > 0 } name)
            {
                targetPropertyNames.Add(name);
            }
        }

        return new MappaMustMapTargetPropertyAttribute([.. targetPropertyNames]);
    }

    /// <summary>
    /// Gets the <see cref="MappaAllowInaccessibleSourceMembersAttribute"/> applied, if any.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAllowInaccessibleSourceMembersAttribute"/> applied, or <c>null</c>.</returns>
    internal static MappaAllowInaccessibleSourceMembersAttribute? GetMappaAllowInaccessibleSourceMembersAttribute(
        this ImmutableArray<AttributeData> attributes,
        Compilation compilation)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(MappaAllowInaccessibleSourceMembersAttributeFullName);
        var attributeData = attributes
            .FirstOrDefault(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol));
        if (attributeData is null)
        {
            return null;
        }

        return new MappaAllowInaccessibleSourceMembersAttribute(ParseMemberNames(attributeData));
    }

    /// <summary>
    /// Gets the <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> applied, if any.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAllowInaccessibleTargetMembersAttribute"/> applied, or <c>null</c>.</returns>
    internal static MappaAllowInaccessibleTargetMembersAttribute? GetMappaAllowInaccessibleTargetMembersAttribute(
        this ImmutableArray<AttributeData> attributes,
        Compilation compilation)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(MappaAllowInaccessibleTargetMembersAttributeFullName);
        var attributeData = attributes
            .FirstOrDefault(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol));
        if (attributeData is null)
        {
            return null;
        }

        var attribute = new MappaAllowInaccessibleTargetMembersAttribute(ParseMemberNames(attributeData));
        foreach (var namedArgument in attributeData.NamedArguments)
        {
            if (namedArgument.Key == nameof(MappaAllowInaccessibleTargetMembersAttribute.AllowProperties)
                && namedArgument.Value.Value is bool allowProperties)
            {
                attribute.AllowProperties = allowProperties;
            }
            else if (namedArgument.Key == nameof(MappaAllowInaccessibleTargetMembersAttribute.AllowConstructors)
                     && namedArgument.Value.Value is bool allowConstructors)
            {
                attribute.AllowConstructors = allowConstructors;
            }
        }

        return attribute;
    }

    /// <summary>
    /// Gets the <see cref="INamedTypeSymbol"/> representing the static dependencies
    /// for this class that have been applied via the <see cref="MappaStaticDependencyAttribute"/>.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaUsePropertyAttribute"/> applied.</returns>
    internal static INamedTypeSymbol[] GetMappaStaticDependencies(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaStaticDependencyAttributeSymbols = compilation.GetTypeByMetadataName(MappaStaticDependencyAttributeFullName);
        List<INamedTypeSymbol> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaStaticDependencyAttributeSymbols))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
            if (constructorArguments.Length == 1 &&
                constructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
            {
                results.Add(namedTypeSymbol);
            }
        }

        return [.. results];
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="attributes"/> contains <see cref="MappaAttribute"/>.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> when <see cref="MappaAttribute"/> is present; otherwise <c>false</c>.</returns>
    internal static bool HasMappaAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(MappaAttributeFullName);
        return attributes
            .Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol));
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="attributes"/> contains
    /// <see cref="MappaDependencyInjectionAttribute"/>.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>
    /// <c>true</c> when <see cref="MappaDependencyInjectionAttribute"/> is present; otherwise <c>false</c>.
    /// </returns>
    internal static bool HasMappaDependencyInjectionAttribute(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(MappaDependencyInjectionAttributeFullName);
        return attributes
            .Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol));
    }

    /// <summary>
    /// Gets the parsed <see cref="MappaDependencyInjectionAttribute"/> data, if any.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The parsed attribute data, or <c>null</c> if the attribute is not present.</returns>
    internal static MappaDependencyInjectionAttributeData? GetMappaDependencyInjectionAttributeData(
        this ImmutableArray<AttributeData> attributes,
        Compilation compilation)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(MappaDependencyInjectionAttributeFullName);
        var attributeData = attributes
            .FirstOrDefault(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol));
        if (attributeData is null)
        {
            return null;
        }

        string? constructorMethodName = null;
        if (attributeData.ConstructorArguments.Length == 1
            && attributeData.ConstructorArguments[0].Value is string constructorName)
        {
            constructorMethodName = constructorName;
        }

        var extensionMethod = true;
        string? methodName = null;
        var accessibility = MappaDependencyInjectionMethodAccessibility.Public;
        var serviceLifetime = MappaDependencyInjectionServiceLifetime.Singleton;
        var injectInterfaces = MappaDependencyInjectionInjectInterfaces.ClassOnly;
        var ignoreTypes = ImmutableArray<INamedTypeSymbol>.Empty;

        foreach (var namedArgument in attributeData.NamedArguments)
        {
            switch (namedArgument.Key)
            {
                case nameof(MappaDependencyInjectionAttribute.ExtensionMethod)
                    when namedArgument.Value.Value is bool extensionMethodValue:
                    extensionMethod = extensionMethodValue;
                    break;

                case nameof(MappaDependencyInjectionAttribute.MethodName)
                    when namedArgument.Value.Value is string methodNameValue:
                    methodName = methodNameValue;
                    break;

                case nameof(MappaDependencyInjectionAttribute.Accessibility)
                    when TryReadEnum(namedArgument.Value, out MappaDependencyInjectionMethodAccessibility accessibilityValue):
                    accessibility = accessibilityValue;
                    break;

                case nameof(MappaDependencyInjectionAttribute.ServiceLifetime)
                    when TryReadEnum(namedArgument.Value, out MappaDependencyInjectionServiceLifetime serviceLifetimeValue):
                    serviceLifetime = serviceLifetimeValue;
                    break;

                case nameof(MappaDependencyInjectionAttribute.InjectInterfaces)
                    when TryReadEnum(namedArgument.Value, out MappaDependencyInjectionInjectInterfaces injectInterfacesValue):
                    injectInterfaces = injectInterfacesValue;
                    break;

                case nameof(MappaDependencyInjectionAttribute.IgnoreType):
                    ignoreTypes = ReadNamedTypeArray(namedArgument.Value);
                    break;
            }
        }

        var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        return new MappaDependencyInjectionAttributeData(
            constructorMethodName,
            methodName,
            extensionMethod,
            accessibility,
            serviceLifetime,
            injectInterfaces,
            ignoreTypes,
            location);
    }

    private static MapHookAttributeData[] GetMapHookAttributes(
        ImmutableArray<AttributeData> attributes,
        Compilation compilation,
        string attributeFullName)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(attributeFullName);
        var results = new List<MapHookAttributeData>();

        foreach (var attributeData in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)))
        {
            var constructorArguments = attributeData.ConstructorArguments;
            var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
            if (constructorArguments.Length == 1 &&
                constructorArguments[0].Value is string mapperMethodName)
            {
                results.Add(new MapHookAttributeData(
                    mapperMethodName,
                    null,
                    null,
                    location));
            }
            else if (constructorArguments.Length == 2 &&
                     constructorArguments[1].Value is string locatedMethodName)
            {
                switch (constructorArguments[0].Value)
                {
                    case string fieldName:
                        results.Add(new MapHookAttributeData(
                            locatedMethodName,
                            null,
                            fieldName,
                            location));
                        break;
                    case INamedTypeSymbol classType:
                        results.Add(new MapHookAttributeData(
                            locatedMethodName,
                            new FakeType(classType.ToDisplayString()),
                            null,
                            location));
                        break;
                }
            }
        }

        return [.. results];
    }

    private static bool IsAttribute(INamedTypeSymbol attributeClass, INamedTypeSymbol? attributeSymbol)
        => SymbolEqualityComparer.Default.Equals(attributeClass.OriginalDefinition, attributeSymbol);

    private static INamedTypeSymbol? GetEnumTypeArgument(
        INamedTypeSymbol attributeClass,
        int expectedNumberOfTypeArguments,
        int index)
        => attributeClass.TypeArguments.Length == expectedNumberOfTypeArguments
            ? attributeClass.TypeArguments[index] as INamedTypeSymbol
            : null;

    private static string? GetEnumMemberName(TypedConstant typedConstant)
    {
        if (typedConstant.Type is not INamedTypeSymbol { TypeKind: TypeKind.Enum } enumType
            || typedConstant.Value is null)
        {
            return null;
        }

        var value = Convert.ToDecimal(typedConstant.Value, CultureInfo.InvariantCulture);
        foreach (var (memberName, memberValue) in enumType.GetEnumValues())
        {
            if (Convert.ToDecimal(memberValue, CultureInfo.InvariantCulture) == value)
            {
                return memberName;
            }
        }

        return null;
    }

    private static string[] ParseMemberNames(AttributeData attributeData)
    {
        var constructorArguments = attributeData.ConstructorArguments;
        if (constructorArguments.Length == 0)
        {
            return [];
        }

        if (constructorArguments.Length == 1 && constructorArguments[0].Kind == TypedConstantKind.Array)
        {
            return constructorArguments[0].Values
                .Select(value => value.Value)
                .OfType<string>()
                .Where(name => name.Length > 0)
                .ToArray();
        }

        // A single params argument may be represented as a primitive string rather than an array.
        return constructorArguments
            .Select(argument => argument.Value)
            .OfType<string>()
            .Where(name => name.Length > 0)
            .ToArray();
    }

    private static bool TryReadEnum<TEnum>(TypedConstant typedConstant, out TEnum value)
        where TEnum : struct
    {
        if (typedConstant.Value is TEnum enumValue)
        {
            value = enumValue;
            return true;
        }

        if (typedConstant.Value is int intValue)
        {
            value = (TEnum)(object)intValue;
            return true;
        }

        value = default;
        return false;
    }

    private static ImmutableArray<INamedTypeSymbol> ReadNamedTypeArray(TypedConstant typedConstant)
    {
        if (typedConstant.Kind != TypedConstantKind.Array)
        {
            return typedConstant.Value is INamedTypeSymbol singleType
                ? [singleType]
                : ImmutableArray<INamedTypeSymbol>.Empty;
        }

        return
        [
            .. typedConstant.Values
                .Select(value => value.Value)
                .OfType<INamedTypeSymbol>(),
        ];
    }
}