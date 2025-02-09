// <copyright file="AttributeDataExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;
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
    private static readonly string MappaSettingsAttributeFullName = typeof(MappaSettingsAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaSettingsAttribute)}");

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
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol))
                     .Select(attributeData => attributeData.ConstructorArguments))
        {
                switch (constructorArguments.Length)
                {
                    case 2: // (targetPropertyName, methodName)
                        {
                            if (constructorArguments[0].Value is string targetParameterName &&
                                constructorArguments[1].Value is string methodName)
                            {
                                results.Add(new MappaInvokeMethodAttribute(targetParameterName, methodName));
                            }
                        }

                        break;

                    case 3: // (targetPropertyName, classType, methodName) or (targetPropertyName, fieldName, methodName)
                        {
                            if (constructorArguments[0].Value is string targetParameterName &&
                                constructorArguments[2].Value is string methodName)
                            {
                                switch (constructorArguments[1].Value)
                                {
                                    case string fieldName:
                                        results.Add(new MappaInvokeMethodAttribute(targetParameterName, fieldName, methodName));
                                        break;
                                    case INamedTypeSymbol classType:
                                        results.Add(new MappaInvokeMethodAttribute(targetParameterName, new FakeType(classType.ToDisplayString()), methodName));
                                        break;
                                }
                            }
                        }

                        break;
                }
        }

        return [.. results];
    }

    /// <summary>
    /// Gets the <see cref="MappaAssignFromContextAttributeFullName"/>s applied.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAssignFromContextAttributeFullName"/> applied.</returns>
    internal static MappaAssignFromContextAttribute[] GetMappaAssignFromContextAttributes(this ImmutableArray<AttributeData> attributes, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaAssignFromContextAttributeFullName);
        List<MappaAssignFromContextAttribute> results = new();
        foreach (var constructorArguments in attributes
                     .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol))
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

                case nameof(MappaSettingsAttribute.TimeSpanFormat) when namedArgument.Value.Value is string value:
                    attribute.TimeSpanFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.GuidFormat) when namedArgument.Value.Value is string value:
                    attribute.GuidFormat = value;
                    break;

                case nameof(MappaSettingsAttribute.CultureName) when namedArgument.Value.Value is string value:
                    attribute.CultureName = value;
                    break;

                case nameof(MappaSettingsAttribute.CultureInfoSetting) when namedArgument.Value.Value is int value:
                    attribute.CultureInfoSetting = (CultureInfoSetting)value;
                    break;

                case nameof(MappaSettingsAttribute.EnableOptional) when namedArgument.Value.Value is EnableSetting value:
                    attribute.EnableOptional = value;
                    break;
            }
        }

        return attribute;
    }

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