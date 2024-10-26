// <copyright file="AttributeDataExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
    private static readonly string MappaAssignFromContextAttribute = typeof(MappaAssignFromContextAttribute).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(MappaAssignFromContextAttribute)}");

    /// <summary>
    /// Gets the <see cref="MappaInvokeMethodAttribute"/>s applied to the method.
    /// </summary>
    /// <param name="methodSymbol">The method.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaInvokeMethodAttribute"/> applied to the method.</returns>
    internal static MappaInvokeMethodAttribute[] GetInvokeMethodAttributes(this IMethodSymbol methodSymbol, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaInvokeMethodAttributeFullName);
        List<MappaInvokeMethodAttribute> results = new();
        foreach (var constructorArguments in methodSymbol
                     .GetAttributes()
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
    /// Gets the <see cref="MappaAssignFromContextAttribute"/>s applied to the method.
    /// </summary>
    /// <param name="methodSymbol">The method.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaAssignFromContextAttribute"/> applied to the method.</returns>
    internal static MappaAssignFromContextAttribute[] GetMappaAssignFromContextAttributes(this IMethodSymbol methodSymbol, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaAssignFromContextAttribute);
        List<MappaAssignFromContextAttribute> results = new();
        foreach (var constructorArguments in methodSymbol
                     .GetAttributes()
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
    /// <param name="methodSymbol">The method.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The <see cref="MappaIgnoreAttribute"/> applied to the method, or <c>null</c> if it does not exist.</returns>
    internal static MappaIgnoreAttribute? GetMappaIgnoreAttribute(this IMethodSymbol methodSymbol, Compilation compilation)
    {
        var mappaInvokeMethodAttributeSymbol = compilation.GetTypeByMetadataName(MappaIgnoreAttributeFullName);
        var exists = methodSymbol
            .GetAttributes()
            .Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappaInvokeMethodAttributeSymbol));
        return exists ? new() : null;
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