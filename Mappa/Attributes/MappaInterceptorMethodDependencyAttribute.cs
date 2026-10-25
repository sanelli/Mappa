// <copyright file="MappaInterceptorMethodDependencyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Marks a method that may be used as a mapping dependency on the
/// <see cref="MappaInterceptorAttribute"/> generation path only.
/// <para>
/// This attribute has no effect on the normal <see cref="MappaAttribute"/> generation path.
/// On the interceptor path, unmarked methods are not used as dependencies unless the class also
/// has <see cref="MappaAttribute"/> (in which case other eligible map methods may be used) or
/// they are marked with this attribute.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaInterceptorMethodDependencyAttribute
    : Attribute;