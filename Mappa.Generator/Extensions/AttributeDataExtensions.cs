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
    /// <summary>
    /// Metadata name for <c>Mappa.Attributes.MappaAttribute</c>.
    /// </summary>
    internal const string MappaAttributeFullName = "Mappa.Attributes.MappaAttribute";

    /// <summary>
    /// Metadata name for <c>Mappa.Attributes.MappaDependencyInjectionAttribute</c>.
    /// </summary>
    internal const string MappaDependencyInjectionAttributeFullName = "Mappa.Attributes.MappaDependencyInjectionAttribute";

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
    private const string MappaTypeMappingAttributeOfTFullName = "Mappa.Attributes.MappaTypeMappingAttribute`2";
    private const string MappaTypeMappingDefaultAttributeFullName = "Mappa.Attributes.MappaTypeMappingDefaultAttribute";
    private const string MappaTypeMappingDefaultAttributeOfTFullName = "Mappa.Attributes.MappaTypeMappingDefaultAttribute`1";
    private const string MappaMapEnumMemberAttributeFullName = "Mappa.Attributes.MappaMapEnumMemberAttribute`1";
    private const string MappaMapEnumMemberToEnumAttributeFullName = "Mappa.Attributes.MappaMapEnumMemberAttribute`2";
    private const string MappaMapEnumIgnoreAttributeFullName = "Mappa.Attributes.MappaMapEnumIgnoreAttribute`1";
    private const string MappaMapEnumDefaultAttributeFullName = "Mappa.Attributes.MappaMapEnumDefaultAttribute`1";

    private static readonly Dictionary<string, Action<MappaSettingsAttribute, TypedConstant>> MappaSettingsNamedArgumentApplicators =
        new(StringComparer.Ordinal)
        {
            [nameof(MappaSettingsAttribute.DateTimeFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.DateTimeFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.DateTimeOffsetFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.DateTimeOffsetFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.DateOnlyFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.DateOnlyFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.TimeOnlyFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.TimeOnlyFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.DateTimeStyle)] = static (attribute, constant) =>
                attribute.DateTimeStyle = ReadDateTimeStyles(constant),
            [nameof(MappaSettingsAttribute.DateTimeOffsetStyle)] = static (attribute, constant) =>
                attribute.DateTimeOffsetStyle = ReadDateTimeStyles(constant),
            [nameof(MappaSettingsAttribute.DateOnlyStyle)] = static (attribute, constant) =>
                attribute.DateOnlyStyle = ReadDateTimeStyles(constant),
            [nameof(MappaSettingsAttribute.TimeOnlyStyle)] = static (attribute, constant) =>
                attribute.TimeOnlyStyle = ReadDateTimeStyles(constant),
            [nameof(MappaSettingsAttribute.GlobalDateTimeStyle)] = static (attribute, constant) =>
                attribute.GlobalDateTimeStyle = ReadDateTimeStyles(constant),
            [nameof(MappaSettingsAttribute.TimeSpanFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.TimeSpanFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.GuidFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.GuidFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.ByteFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.ByteFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.SByteFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.SByteFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.ShortFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.ShortFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.UShortFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.UShortFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.IntFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.IntFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.UIntFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.UIntFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.LongFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.LongFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.ULongFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.ULongFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.DecimalFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.DecimalFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.FloatFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.FloatFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.DoubleFormat)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.DoubleFormat = value;
                }
            },
            [nameof(MappaSettingsAttribute.ByteStyle)] = static (attribute, constant) =>
                attribute.ByteStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.SByteStyle)] = static (attribute, constant) =>
                attribute.SByteStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.ShortStyle)] = static (attribute, constant) =>
                attribute.ShortStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.UShortStyle)] = static (attribute, constant) =>
                attribute.UShortStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.IntStyle)] = static (attribute, constant) =>
                attribute.IntStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.UIntStyle)] = static (attribute, constant) =>
                attribute.UIntStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.LongStyle)] = static (attribute, constant) =>
                attribute.LongStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.ULongStyle)] = static (attribute, constant) =>
                attribute.ULongStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.DecimalStyle)] = static (attribute, constant) =>
                attribute.DecimalStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.FloatStyle)] = static (attribute, constant) =>
                attribute.FloatStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.DoubleStyle)] = static (attribute, constant) =>
                attribute.DoubleStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.GlobalNumberStyle)] = static (attribute, constant) =>
                attribute.GlobalNumberStyle = ReadNumberStyles(constant),
            [nameof(MappaSettingsAttribute.CultureName)] = static (attribute, constant) =>
            {
                if (constant.Value is string value)
                {
                    attribute.CultureName = value;
                }
            },
            [nameof(MappaSettingsAttribute.CultureInfoSetting)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.CultureInfoSetting = (CultureInfoSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.ProtobufOptional)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.ProtobufOptional = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.PragmaWarning)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.PragmaWarning = (PragmaWarningSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.FastCollections)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.FastCollections = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.ContainerCapacityConstructors)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.ContainerCapacityConstructors = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.PreventEnumerableCount)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.PreventEnumerableCount = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.PolymorphicMapMethodWithMatchingDefaultAttribute = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.CompatibleMapMethod)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.CompatibleMapMethod = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.CaseInsensitivePropertyMap)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.CaseInsensitivePropertyMap = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.IgnoreUnderscoreForPropertyMap)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.IgnoreUnderscoreForPropertyMap = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.CaseInsensitiveEnumMap)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.CaseInsensitiveEnumMap = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.EnumStringMapSetting)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.EnumStringMapSetting = (EnumStringMapSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.EnumToEnumMapSetting)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.EnumToEnumMapSetting = (EnumToEnumMapSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.IdentityMapDeepCopy)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.IdentityMapDeepCopy = (IdentityMapDeepCopySetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.EnumerableConcreteType)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.EnumerableConcreteType = (EnumerableConcreteTypeSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.DictionaryAssignment)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.DictionaryAssignment = (DictionaryAssignmentSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.ReferenceReusing)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.ReferenceReusing = (BooleanSetting)value;
                }
            },
            [nameof(MappaSettingsAttribute.MaxRuntimeDepth)] = static (attribute, constant) =>
                attribute.MaxRuntimeDepth = ReadDepth(constant),
            [nameof(MappaSettingsAttribute.MaxCompileTimeDepth)] = static (attribute, constant) =>
                attribute.MaxCompileTimeDepth = ReadDepth(constant),
            [nameof(MappaSettingsAttribute.BreakCompileTimeCycles)] = static (attribute, constant) =>
            {
                if (constant.Value is int value)
                {
                    attribute.BreakCompileTimeCycles = (BooleanSetting)value;
                }
            },
        };

    private static readonly Dictionary<string, Action<MappaDependencyInjectionNamedArgumentValues, TypedConstant>>
        MappaDependencyInjectionNamedArgumentApplicators =
            new(StringComparer.Ordinal)
            {
                [nameof(MappaDependencyInjectionAttribute.ExtensionMethod)] = static (values, constant) =>
                {
                    if (constant.Value is bool extensionMethodValue)
                    {
                        values.ExtensionMethod = extensionMethodValue;
                    }
                },
                [nameof(MappaDependencyInjectionAttribute.MethodName)] = static (values, constant) =>
                {
                    if (constant.Value is string methodNameValue)
                    {
                        values.MethodName = methodNameValue;
                    }
                },
                [nameof(MappaDependencyInjectionAttribute.Accessibility)] = static (values, constant) =>
                {
                    if (TryReadEnum(constant, out MappaDependencyInjectionMethodAccessibility accessibilityValue))
                    {
                        values.Accessibility = accessibilityValue;
                    }
                },
                [nameof(MappaDependencyInjectionAttribute.ServiceLifetime)] = static (values, constant) =>
                {
                    if (TryReadEnum(constant, out MappaDependencyInjectionServiceLifetime serviceLifetimeValue))
                    {
                        values.ServiceLifetime = serviceLifetimeValue;
                    }
                },
                [nameof(MappaDependencyInjectionAttribute.InjectInterfaces)] = static (values, constant) =>
                {
                    if (TryReadEnum(constant, out MappaDependencyInjectionInjectInterfaces injectInterfacesValue))
                    {
                        values.InjectInterfaces = injectInterfacesValue;
                    }
                },
                [nameof(MappaDependencyInjectionAttribute.IgnoreType)] = static (values, constant) =>
                    values.IgnoreTypes = ReadNamedTypeArray(constant),
                [nameof(MappaDependencyInjectionAttribute.InjectFromAssemblies)] = static (values, constant) =>
                    values.InjectFromAssemblies = ReadNamedTypeArray(constant),
            };

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
            if (TryParseEnumMapMemberAttribute(
                    attributeData,
                    enumMemberAttributeSymbol,
                    enumMemberToEnumAttributeSymbol) is { } enumMapMember)
            {
                results.Add(enumMapMember);
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
            if (TryParseEnumMapDefaultAttribute(attributeData, enumDefaultAttributeSymbol) is { } enumMapDefault)
            {
                results.Add(enumMapDefault);
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
        var mappaTypeMappingDefaultAttributeSymbol = compilation.GetTypeByMetadataName(MappaTypeMappingDefaultAttributeFullName);
        var mappaTypeMappingDefaultAttributeOfTSymbol = compilation.GetTypeByMetadataName(MappaTypeMappingDefaultAttributeOfTFullName);
        MappaTypeMappingDefaultAttribute? attribute = null;

        foreach (var attributeData in attributes)
        {
            if (attributeData.AttributeClass is not { } attributeClass)
            {
                continue;
            }

            if (IsAttribute(attributeClass, mappaTypeMappingDefaultAttributeOfTSymbol)
                && GetEnumTypeArgument(attributeClass, 1, 0) is { } defaultType)
            {
                attribute = new MappaTypeMappingDefaultAttribute(
                    MappaTypeMappingDefaultBehavior.MapSourceType,
                    new FakeType(defaultType.ToDisplayString()));
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(attributeClass, mappaTypeMappingDefaultAttributeSymbol))
            {
                continue;
            }

            attribute = CreateMappaTypeMappingDefaultFromConstructorArguments(attributeData.ConstructorArguments);
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
        var mappaTypeMappingAttributeOfTSymbol = compilation.GetTypeByMetadataName(MappaTypeMappingAttributeOfTFullName);
        List<MappaTypeMappingAttribute> results = new();

        foreach (var attributeData in attributes)
        {
            if (TryParseTypeMappingAttribute(
                    attributeData,
                    mappaTypeMappingAttributeSymbol,
                    mappaTypeMappingAttributeOfTSymbol) is { } typeMapping)
            {
                results.Add(typeMapping);
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
            if (TryCreateInvokeMethodAttribute(attributeData) is not { } attribute)
            {
                continue;
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
            if (TryCreateMappaObjectFactoryAttributeData(attributeData) is { } factoryAttribute)
            {
                results.Add(factoryAttribute);
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
            ApplyMappaSettingsNamedArgument(attribute, namedArgument);
        }

        return attribute;
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

        var namedArgumentValues = new MappaDependencyInjectionNamedArgumentValues();
        foreach (var namedArgument in attributeData.NamedArguments)
        {
            ApplyMappaDependencyInjectionNamedArgument(namedArgumentValues, namedArgument);
        }

        var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        return new MappaDependencyInjectionAttributeData(
            constructorMethodName,
            namedArgumentValues.MethodName,
            namedArgumentValues.ExtensionMethod,
            namedArgumentValues.Accessibility,
            namedArgumentValues.ServiceLifetime,
            namedArgumentValues.InjectInterfaces,
            namedArgumentValues.IgnoreTypes,
            namedArgumentValues.InjectFromAssemblies,
            location);
    }

    private static void ApplyMappaSettingsNamedArgument(
        MappaSettingsAttribute attribute,
        KeyValuePair<string, TypedConstant> namedArgument)
    {
        if (MappaSettingsNamedArgumentApplicators.TryGetValue(namedArgument.Key, out var apply))
        {
            apply(attribute, namedArgument.Value);
        }
    }

    private static void ApplyMappaDependencyInjectionNamedArgument(
        MappaDependencyInjectionNamedArgumentValues values,
        KeyValuePair<string, TypedConstant> namedArgument)
    {
        if (MappaDependencyInjectionNamedArgumentApplicators.TryGetValue(namedArgument.Key, out var apply))
        {
            apply(values, namedArgument.Value);
        }
    }

    private static EnumMapMemberInfoAttribute? TryParseEnumMapMemberAttribute(
        AttributeData attributeData,
        INamedTypeSymbol? enumMemberAttributeSymbol,
        INamedTypeSymbol? enumMemberToEnumAttributeSymbol)
    {
        if (attributeData.AttributeClass is not { } attributeClass
            || attributeData.ConstructorArguments.Length != 2)
        {
            return null;
        }

        var constructorArguments = attributeData.ConstructorArguments;
        var singleEnumResult = TryParseSingleEnumMapMemberAttribute(
            attributeClass,
            enumMemberAttributeSymbol,
            constructorArguments);
        if (singleEnumResult is not null)
        {
            return singleEnumResult;
        }

        return TryParseEnumToEnumMapMemberAttribute(
            attributeClass,
            enumMemberToEnumAttributeSymbol,
            constructorArguments);
    }

    private static EnumMapMemberInfoAttribute? TryParseSingleEnumMapMemberAttribute(
        INamedTypeSymbol attributeClass,
        INamedTypeSymbol? enumMemberAttributeSymbol,
        ImmutableArray<TypedConstant> constructorArguments)
    {
        if (!IsAttribute(attributeClass, enumMemberAttributeSymbol)
            || GetEnumTypeArgument(attributeClass, 1, 0) is not { } enumType
            || GetEnumMemberName(constructorArguments[0]) is not { } enumMemberName)
        {
            return null;
        }

        return CreateSingleEnumMapMemberAttribute(enumType, enumMemberName, constructorArguments[1]);
    }

    private static EnumMapMemberInfoAttribute? TryParseEnumToEnumMapMemberAttribute(
        INamedTypeSymbol attributeClass,
        INamedTypeSymbol? enumMemberToEnumAttributeSymbol,
        ImmutableArray<TypedConstant> constructorArguments)
    {
        if (!IsAttribute(attributeClass, enumMemberToEnumAttributeSymbol)
            || GetEnumTypeArgument(attributeClass, 2, 0) is not { } firstEnumType
            || GetEnumTypeArgument(attributeClass, 2, 1) is not { } secondEnumType
            || GetEnumMemberName(constructorArguments[0]) is not { } firstEnumMemberName
            || GetEnumMemberName(constructorArguments[1]) is not { } secondEnumMemberName)
        {
            return null;
        }

        return new EnumMapMemberInfoAttribute(
            firstEnumType,
            firstEnumMemberName,
            null,
            null,
            secondEnumType,
            secondEnumMemberName);
    }

    private static EnumMapMemberInfoAttribute? CreateSingleEnumMapMemberAttribute(
        INamedTypeSymbol enumType,
        string enumMemberName,
        TypedConstant secondArgument)
    {
        return secondArgument.Value switch
        {
            string stringValue => new EnumMapMemberInfoAttribute(enumType, enumMemberName, null, stringValue, null, null),
            int integerValue => new EnumMapMemberInfoAttribute(enumType, enumMemberName, integerValue, null, null, null),
            _ => null,
        };
    }

    private static EnumMapDefaultInfoAttribute? TryParseEnumMapDefaultAttribute(
        AttributeData attributeData,
        INamedTypeSymbol? enumDefaultAttributeSymbol)
    {
        if (attributeData.AttributeClass is not { } attributeClass
            || !IsAttribute(attributeClass, enumDefaultAttributeSymbol)
            || GetEnumTypeArgument(attributeClass, 1, 0) is not { } enumType)
        {
            return null;
        }

        var constructorArguments = attributeData.ConstructorArguments;
        if (constructorArguments.Length == 0
            || constructorArguments[0].Value is not int behaviorValue)
        {
            return null;
        }

        var behavior = (MappaMapEnumDefaultBehavior)behaviorValue;
        if (constructorArguments.Length == 1)
        {
            return new EnumMapDefaultInfoAttribute(enumType, behavior, null, null, null);
        }

        return CreateEnumMapDefaultWithExplicitDefault(enumType, behavior, constructorArguments[1]);
    }

    private static EnumMapDefaultInfoAttribute? CreateEnumMapDefaultWithExplicitDefault(
        INamedTypeSymbol enumType,
        MappaMapEnumDefaultBehavior behavior,
        TypedConstant defaultValue)
    {
        if (defaultValue.Type is INamedTypeSymbol { TypeKind: TypeKind.Enum }
            && GetEnumMemberName(defaultValue) is { } enumDefaultMemberName)
        {
            return new EnumMapDefaultInfoAttribute(enumType, behavior, enumDefaultMemberName, null, null);
        }

        return defaultValue.Value switch
        {
            string stringDefaultValue => new EnumMapDefaultInfoAttribute(enumType, behavior, null, null, stringDefaultValue),
            int integerDefaultValue => new EnumMapDefaultInfoAttribute(enumType, behavior, null, integerDefaultValue, null),
            _ => null,
        };
    }

    private static MappaTypeMappingDefaultAttribute? CreateMappaTypeMappingDefaultFromConstructorArguments(
        ImmutableArray<TypedConstant> constructorArguments)
    {
        return constructorArguments.Length switch
        {
            1 => CreateMappaTypeMappingDefaultFromSingleConstructorArgument(constructorArguments[0]),
            2 => CreateMappaTypeMappingDefaultFromTwoConstructorArguments(constructorArguments[0], constructorArguments[1]),
            _ => null,
        };
    }

    private static MappaTypeMappingDefaultAttribute? CreateMappaTypeMappingDefaultFromSingleConstructorArgument(
        TypedConstant argument)
    {
        return argument.Value switch
        {
            string methodName => new MappaTypeMappingDefaultAttribute(methodName),
            int behavior => new MappaTypeMappingDefaultAttribute((MappaTypeMappingDefaultBehavior)behavior),
            _ => null,
        };
    }

    private static MappaTypeMappingDefaultAttribute? CreateMappaTypeMappingDefaultFromTwoConstructorArguments(
        TypedConstant firstArgument,
        TypedConstant secondArgument)
    {
        if (firstArgument.Value is INamedTypeSymbol invokeType && secondArgument.Value is string methodName)
        {
            return new MappaTypeMappingDefaultAttribute(new FakeType(invokeType.ToDisplayString()), methodName);
        }

        if (firstArgument.Value is int behavior && secondArgument.Value is INamedTypeSymbol type)
        {
            return new MappaTypeMappingDefaultAttribute((MappaTypeMappingDefaultBehavior)behavior, new FakeType(type.ToDisplayString()));
        }

        return null;
    }

    private static MappaTypeMappingAttribute? TryParseTypeMappingAttribute(
        AttributeData attributeData,
        INamedTypeSymbol? mappaTypeMappingAttributeSymbol,
        INamedTypeSymbol? mappaTypeMappingAttributeOfTSymbol)
    {
        if (attributeData.AttributeClass is not { } attributeClass)
        {
            return null;
        }

        var genericResult = TryParseGenericTypeMappingAttribute(attributeClass, mappaTypeMappingAttributeOfTSymbol);
        if (genericResult is not null)
        {
            return genericResult;
        }

        return TryParseNonGenericTypeMappingAttribute(attributeClass, mappaTypeMappingAttributeSymbol, attributeData.ConstructorArguments);
    }

    private static MappaTypeMappingAttribute? TryParseGenericTypeMappingAttribute(
        INamedTypeSymbol attributeClass,
        INamedTypeSymbol? mappaTypeMappingAttributeOfTSymbol)
    {
        if (!IsAttribute(attributeClass, mappaTypeMappingAttributeOfTSymbol)
            || GetEnumTypeArgument(attributeClass, 2, 0) is not { } genericTargetType
            || GetEnumTypeArgument(attributeClass, 2, 1) is not { } genericSourceType)
        {
            return null;
        }

        return new MappaTypeMappingAttribute(
            new FakeType(genericTargetType.ToDisplayString()),
            new FakeType(genericSourceType.ToDisplayString()));
    }

    private static MappaTypeMappingAttribute? TryParseNonGenericTypeMappingAttribute(
        INamedTypeSymbol attributeClass,
        INamedTypeSymbol? mappaTypeMappingAttributeSymbol,
        ImmutableArray<TypedConstant> constructorArguments)
    {
        if (!SymbolEqualityComparer.Default.Equals(attributeClass, mappaTypeMappingAttributeSymbol)
            || constructorArguments.Length != 2
            || constructorArguments[0].Value is not INamedTypeSymbol targetType
            || constructorArguments[1].Value is not INamedTypeSymbol sourceType)
        {
            return null;
        }

        return new MappaTypeMappingAttribute(
            new FakeType(targetType.ToDisplayString()),
            new FakeType(sourceType.ToDisplayString()));
    }

    private static MappaInvokeMethodAttribute? TryCreateInvokeMethodAttribute(AttributeData attributeData)
    {
        var constructorArguments = attributeData.ConstructorArguments;
        MappaInvokeMethodAttribute? attribute = constructorArguments.Length switch
        {
            2 when constructorArguments[0].Value is string targetParameterNameTwo
                 && constructorArguments[1].Value is string methodNameTwo
                => new MappaInvokeMethodAttribute(targetParameterNameTwo, methodNameTwo),
            3 when constructorArguments[0].Value is string targetParameterNameThree
                 && constructorArguments[2].Value is string methodNameThree
                => CreateInvokeMethodAttributeWithMiddleArgument(
                    targetParameterNameThree,
                    constructorArguments[1],
                    methodNameThree),
            _ => null,
        };

        if (attribute is null)
        {
            return null;
        }

        ApplyInvokeMethodNamedArguments(attribute, attributeData.NamedArguments);
        return attribute;
    }

    private static MappaInvokeMethodAttribute? CreateInvokeMethodAttributeWithMiddleArgument(
        string targetParameterName,
        TypedConstant middleArgument,
        string methodName)
    {
        return middleArgument.Value switch
        {
            string fieldName => new MappaInvokeMethodAttribute(targetParameterName, fieldName, methodName),
            INamedTypeSymbol classType => new MappaInvokeMethodAttribute(
                targetParameterName,
                new FakeType(classType.ToDisplayString()),
                methodName),
            _ => null,
        };
    }

    private static void ApplyInvokeMethodNamedArguments(
        MappaInvokeMethodAttribute attribute,
        ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
    {
        foreach (var namedArgument in namedArguments)
        {
            if (namedArgument.Key == nameof(MappaInvokeMethodAttribute.SourcePropertyName)
                && namedArgument.Value.Value is string sourcePropertyName)
            {
                attribute.SourcePropertyName = sourcePropertyName;
            }
        }
    }

    private static MappaObjectFactoryAttributeData? TryCreateMappaObjectFactoryAttributeData(AttributeData attributeData)
    {
        var constructorArguments = attributeData.ConstructorArguments;
        var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        if (constructorArguments.Length == 2
            && constructorArguments[0].Value is INamedTypeSymbol targetType
            && constructorArguments[1].Value is string methodName)
        {
            return new MappaObjectFactoryAttributeData(targetType, methodName, null, null, location);
        }

        if (constructorArguments.Length != 3
            || constructorArguments[0].Value is not INamedTypeSymbol factoryTargetType
            || constructorArguments[2].Value is not string factoryMethodName)
        {
            return null;
        }

        return CreateMappaObjectFactoryWithMiddleArgument(
            factoryTargetType,
            factoryMethodName,
            constructorArguments[1],
            location);
    }

    private static MappaObjectFactoryAttributeData? CreateMappaObjectFactoryWithMiddleArgument(
        INamedTypeSymbol factoryTargetType,
        string factoryMethodName,
        TypedConstant middleArgument,
        Location? location)
    {
        return middleArgument.Value switch
        {
            string fieldName => new MappaObjectFactoryAttributeData(
                factoryTargetType,
                factoryMethodName,
                null,
                fieldName,
                location),
            INamedTypeSymbol classType => new MappaObjectFactoryAttributeData(
                factoryTargetType,
                factoryMethodName,
                new FakeType(classType.ToDisplayString()),
                null,
                location),
            _ => null,
        };
    }

    private static MapHookAttributeData? TryCreateMapHookAttributeData(AttributeData attributeData)
    {
        var constructorArguments = attributeData.ConstructorArguments;
        var location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        if (constructorArguments.Length == 1
            && constructorArguments[0].Value is string mapperMethodName)
        {
            return new MapHookAttributeData(mapperMethodName, null, null, location);
        }

        if (constructorArguments.Length != 2
            || constructorArguments[1].Value is not string locatedMethodName)
        {
            return null;
        }

        return CreateMapHookWithLocationArgument(locatedMethodName, constructorArguments[0], location);
    }

    private static MapHookAttributeData? CreateMapHookWithLocationArgument(
        string locatedMethodName,
        TypedConstant locationArgument,
        Location? location)
    {
        return locationArgument.Value switch
        {
            string fieldName => new MapHookAttributeData(locatedMethodName, null, fieldName, location),
            INamedTypeSymbol classType => new MapHookAttributeData(
                locatedMethodName,
                new FakeType(classType.ToDisplayString()),
                null,
                location),
            _ => null,
        };
    }

    private static DateTimeStyles ReadDateTimeStyles(TypedConstant typedConstant)
    {
        return typedConstant.Value switch
        {
            null => MappaSettingsAttribute.UndefinedDateTimeStyle,
            int intValue => (DateTimeStyles)intValue,
            DateTimeStyles dateTimeStyles => dateTimeStyles,
            _ => MappaSettingsAttribute.UndefinedDateTimeStyle,
        };
    }

    private static NumberStyles ReadNumberStyles(TypedConstant typedConstant)
    {
        return typedConstant.Value switch
        {
            null => MappaSettingsAttribute.UndefinedNumberStyle,
            int intValue => (NumberStyles)intValue,
            NumberStyles numberStyles => numberStyles,
            _ => MappaSettingsAttribute.UndefinedNumberStyle,
        };
    }

    private static short ReadDepth(TypedConstant typedConstant)
    {
        return typedConstant.Value switch
        {
            null => MappaSettingsAttribute.UndefinedDepth,
            short shortValue => shortValue,
            int intValue => (short)intValue,
            long longValue => (short)longValue,
            _ => MappaSettingsAttribute.UndefinedDepth,
        };
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
            if (TryCreateMapHookAttributeData(attributeData) is { } mapHook)
            {
                results.Add(mapHook);
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

    private sealed class MappaDependencyInjectionNamedArgumentValues
    {
        internal bool ExtensionMethod { get; set; } = true;

        internal string? MethodName { get; set; }

        internal MappaDependencyInjectionMethodAccessibility Accessibility { get; set; } =
            MappaDependencyInjectionMethodAccessibility.Public;

        internal MappaDependencyInjectionServiceLifetime ServiceLifetime { get; set; } =
            MappaDependencyInjectionServiceLifetime.Singleton;

        internal MappaDependencyInjectionInjectInterfaces InjectInterfaces { get; set; } =
            MappaDependencyInjectionInjectInterfaces.ClassOnly;

        internal ImmutableArray<INamedTypeSymbol> IgnoreTypes { get; set; } =
            ImmutableArray<INamedTypeSymbol>.Empty;

        internal ImmutableArray<INamedTypeSymbol> InjectFromAssemblies { get; set; } =
            ImmutableArray<INamedTypeSymbol>.Empty;
    }
}