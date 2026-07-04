// <copyright file="AttributeDataExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using Mappa.Attributes;
using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeData"/>.
/// </summary>
internal static class AttributeDataExtensions
{
    private static readonly string MappaInvokeMethodAttributeFullName = typeof(MappaInvokeMethodAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaInvokeMethodAttribute)}");
    private static readonly string MappaIgnoreAttributeFullName = typeof(MappaIgnoreAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaIgnoreAttribute)}");
    private static readonly string MappaAssignFromContextAttributeFullName = typeof(MappaAssignFromContextAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaAssignFromContextAttribute)}");
    private static readonly string MappaAssignToContextAttributeFullName = typeof(MappaAssignToContextAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaAssignToContextAttribute)}");
    private static readonly string MappaSettingsAttributeFullName = typeof(MappaSettingsAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaSettingsAttribute)}");
    private static readonly string MappaUsePropertyAttributeFullName = typeof(MappaUsePropertyAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaUsePropertyAttribute)}");
    private static readonly string MappaDependencyAttributeFullName = typeof(MappaDependencyAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaDependencyAttribute)}");
    private static readonly string MappaStaticDependencyAttributeFullName = typeof(MappaStaticDependencyAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaStaticDependencyAttribute)}");
    private static readonly string MappaAssignFromConstantAttributeFullName = typeof(MappaAssignFromConstantAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaAssignFromConstantAttribute)}");
    private static readonly string MappaIgnoreTargetPropertyAttributeFullName = typeof(MappaIgnoreTargetPropertyAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaIgnoreTargetPropertyAttribute)}");
    private static readonly string MappaTypeMappingAttributeFullName = typeof(MappaTypeMappingAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaTypeMappingAttribute)}");
    private static readonly string MappaTypeMappingDefaultAttributeFullName = typeof(MappaTypeMappingDefaultAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaTypeMappingDefaultAttribute)}");

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

        return [..results];
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

        return [..results];
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

                case nameof(MappaSettingsAttribute.PolymorphicMapMethodWithMatchingDefaultAttribute) when namedArgument.Value.Value is int value:
                    attribute.PolymorphicMapMethodWithMatchingDefaultAttribute = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.CaseInsensitivePropertyMap) when namedArgument.Value.Value is int value:
                    attribute.CaseInsensitivePropertyMap = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.IgnoreUnderscoreForPropertyMap) when namedArgument.Value.Value is int value:
                    attribute.IgnoreUnderscoreForPropertyMap = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.CaseInsensitiveStringToEnumMap) when namedArgument.Value.Value is int value:
                    attribute.CaseInsensitiveStringToEnumMap = (BooleanSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.EnumToEnumMapSetting) when namedArgument.Value.Value is int value:
                    attribute.EnumToEnumMapSetting = (EnumToEnumMapSetting)value;
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

        return [..results];
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

        return [..results];
    }

    [DebuggerDisplay("FullName = {FullName}")]
    private sealed class FakeType(string fullName) : Type
    {
        public override Module Module => throw new NotImplementedException();

        public override string? Namespace => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        public override Assembly Assembly => throw new NotImplementedException();

        public override string? AssemblyQualifiedName => throw new NotImplementedException();

        public override Type? BaseType => throw new NotImplementedException();

        public override string? FullName => fullName;

        public override Guid GUID => throw new NotImplementedException();

        public override Type UnderlyingSystemType => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit)
            => throw new NotImplementedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            => throw new NotImplementedException();

        public override bool IsDefined(Type attributeType, bool inherit)
            => throw new NotImplementedException();

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override Type? GetElementType()
            => throw new NotImplementedException();

        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            => throw new NotImplementedException();

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
            => throw new NotImplementedException();

        public override Type GetInterface(string name, bool ignoreCase)
            => throw new NotImplementedException();

        public override Type[] GetInterfaces()
            => throw new NotImplementedException();

        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => throw new NotImplementedException();

        protected override TypeAttributes GetAttributeFlagsImpl()
            => throw new NotImplementedException();

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => throw new NotImplementedException();

        protected override bool IsArrayImpl()
            => throw new NotImplementedException();

        protected override bool IsByRefImpl()
            => throw new NotImplementedException();

        protected override bool IsCOMObjectImpl()
            => throw new NotImplementedException();

        protected override bool IsPointerImpl()
            => throw new NotImplementedException();

        protected override bool IsPrimitiveImpl()
            => throw new NotImplementedException();

        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            => throw new NotImplementedException();

        protected override bool HasElementTypeImpl()
            => throw new NotImplementedException();
    }
}