// <copyright file="FakeType.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Minimal <see cref="Type"/> stub used when constructing attribute instances
/// from Roslyn symbols that expose type arguments as <see cref="Type"/>.
/// </summary>
[DebuggerDisplay("FullName = {FullName}")]
internal sealed class FakeType(string fullName) : Type
{
    /// <inheritdoc/>
    public override Module Module => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string? Namespace => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string Name => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Assembly Assembly => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string? AssemblyQualifiedName => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type? BaseType => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string? FullName => fullName;

    /// <inheritdoc/>
    public override Guid GUID => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type UnderlyingSystemType => throw new NotImplementedException();

    /// <inheritdoc/>
    public override object[] GetCustomAttributes(bool inherit)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override bool IsDefined(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type? GetElementType()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type GetNestedType(string name, BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type GetInterface(string name, bool ignoreCase)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Type[] GetInterfaces()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override TypeAttributes GetAttributeFlagsImpl()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override bool IsArrayImpl()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override bool IsByRefImpl()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override bool IsCOMObjectImpl()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override bool IsPointerImpl()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override bool IsPrimitiveImpl()
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override bool HasElementTypeImpl()
        => throw new NotImplementedException();
}